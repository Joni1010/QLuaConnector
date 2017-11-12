using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NSGraphic
{
    public class Graphic
    {
        /// <summary> Полотно для рисования  </summary>
        private Graphics Canvas;
        /// <summary> Координаты перекрестья </summary>
        public Point CrossLine = new Point();
        /// <summary> Минимальный шаг цены </summary>
        public decimal MinStepPrice = 1;

        private Decimal MinPrice = 0;
        private Decimal MaxPrice = 0;

        public Prices PanelPrices = null;
        public Candles PanelCandels = null;
        public VertLevels PanelVolumes = null;
        public HorLevels PanelHorVolumes = null; //Горизонтальные объемы в панеле
        public HorLevels PanelDiffHorVolumes = null; //Разница гориз. объемов в панеле
        public TimeFrame PanelTimes = null;
        public LevelsOrder Orders = null;

        /// <summary> Кол-во свечек по которым выводим горизонтальный объем</summary>
        public int CountCandleShowHVol = 3;
        /// <summary> Прямоугольник верхней области, с отрезанными нижними областями под панели. </summary>
        private Rectangle RectAllTop;

        public Graphic(decimal minStepPrice = 1)
        {
            this.MinStepPrice = minStepPrice;
            PanelPrices = new Prices(2);
            PanelCandels = new Candles();
            PanelVolumes = new VertLevels(this.MinStepPrice);
            PanelDiffHorVolumes = new HorLevels(this.MinStepPrice);
            PanelHorVolumes = new HorLevels(this.MinStepPrice);
            PanelTimes = new TimeFrame();
            Orders = new LevelsOrder();

            this.InitEvents();
        }

        /// <summary> Расчет мин и макс цены на всем графике </summary>
        private void GetMinMax()
        {
            this.MaxPrice = this.PanelCandels.CollectionCandle.Max(c => c.High);
            this.MinPrice = this.PanelCandels.CollectionCandle.Min(c => c.Low);

            //Ищем макс и мин в заявках
            if (Orders.CollectionOrders.Count() > 0)
            {
                foreach (var ord in Orders.CollectionOrders)
                {
                    if (this.MaxPrice < ord.Price) this.MaxPrice = ord.Price;
                    if (this.MinPrice > ord.Price) this.MinPrice = ord.Price;
                }
            }

            this.MaxPrice += (this.MaxPrice - this.MinPrice) * 10 / 100;
            this.MinPrice -= (this.MaxPrice - this.MinPrice) * 10 / 100;
        }
        /// <summary> Расчет горизонтальных объемов для панелей  </summary>
        private void CalculateHorVolumeForPanel()
        {
            List<MarketObject.Chart> volumes = new List<MarketObject.Chart>();
            List<MarketObject.Chart> horDiffVolumes = new List<MarketObject.Chart>(); //разница гор. объемов
            List<MarketObject.Chart> horVolumes = new List<MarketObject.Chart>(); //гор. объемы
            PanelVolumes.Max = 0;
            PanelVolumes.Min = 0;

            PanelDiffHorVolumes.Min = 100000;
            PanelDiffHorVolumes.Max = -100000;

            PanelHorVolumes.Min = 0;
            PanelHorVolumes.Max = -100000;

            LisForHVol[0] = this.PanelCandels.CollectionCandle.First();

            this.PanelCandels.CollectionCandle.ToList().ForEach((oneCan) =>
            {
                volumes.Add(new MarketObject.Chart() { Price = 0, Volume = oneCan.Volume });
                PanelVolumes.Max = PanelVolumes.Max < oneCan.Volume ? oneCan.Volume : PanelVolumes.Max;
            });

            if (!LisForHVol.IsNull())
            {
                LisForHVol.ForEach((oneCan) =>
                {
                    oneCan.HorVolumes.HVolCollection.CollectionArray.ForEach<MarketObject.ChartVol>((el) =>
                    {
                        long res = 0, all = 0;
                        res += el.VolBuy; all += el.VolBuy;
                        res -= el.VolSell; all += el.VolSell;
                        if (res != 0 || all != 0)
                        {
                            var levelDiff = horDiffVolumes.FirstOrDefault(l => l.Price == el.Price);
                            var level = horVolumes.FirstOrDefault(l => l.Price == el.Price);
                            if (levelDiff != null)
                            {
                                levelDiff.Volume += res;
                                PanelDiffHorVolumes.Max = levelDiff.Volume > PanelDiffHorVolumes.Max ? levelDiff.Volume : PanelDiffHorVolumes.Max;
                                PanelDiffHorVolumes.Min = levelDiff.Volume < PanelDiffHorVolumes.Min ? levelDiff.Volume : PanelDiffHorVolumes.Min;

                                level.Volume += all;
                                PanelHorVolumes.Max = level.Volume > PanelHorVolumes.Max ? level.Volume : PanelHorVolumes.Max;
                            }
                            else
                            {
                                horDiffVolumes.Add(new MarketObject.Chart() { Price = el.Price, Volume = res });
                                PanelDiffHorVolumes.Max = res > PanelDiffHorVolumes.Max ? res : PanelDiffHorVolumes.Max;
                                PanelDiffHorVolumes.Min = res < PanelDiffHorVolumes.Min ? res : PanelDiffHorVolumes.Min;

                                horVolumes.Add(new MarketObject.Chart() { Price = el.Price, Volume = all });
                                PanelHorVolumes.Max = all > PanelHorVolumes.Max ? all : PanelHorVolumes.Max;
                            }
                        }
                    });
                });
            }
            PanelVolumes.Max += (int)(PanelVolumes.Max * 10 / 100);
            PanelVolumes.CollectionLevels = volumes;

            PanelDiffHorVolumes.Max++;
            PanelDiffHorVolumes.Min--;
            PanelHorVolumes.Max++;
            PanelDiffHorVolumes.CollectionLevels = horDiffVolumes;
            PanelHorVolumes.CollectionLevels = horVolumes;
        }

        /// <summary>Отрисовка всего графика </summary>
        /// <param name="graphic">Полотно</param>
        public void Paint(Graphics graphic, Rectangle rectPaint)
        {
            this.Canvas = graphic;

            if (this.PanelCandels.CollectionCandle.IsNull() ||
                this.PanelCandels.CollectionCandle.Count() == 0) return;

            this.GetMinMax();
            //Гор. объемы
            this.CalculateHorVolumeForPanel();

            ///Нижняя временная шкала
            var BottomRectForTimeLine = RectanglePaint.GetBottom(rectPaint, 20);
            RectAllTop = RectanglePaint.DeleteBottom(rectPaint, BottomRectForTimeLine);

            //Получение области для нижних объемов
            var BottomRectForVol = RectanglePaint.GetBottom(RectAllTop, 50);
            RectAllTop = RectanglePaint.DeleteBottom(RectAllTop, BottomRectForVol);

            PanelPrices.CurrentValue = PanelCandels.CollectionCandle.ElementAt(0).Close;
            PanelPrices.WidthBorder = 60;                   //Ширина области рисования
            PanelPrices.RectPaint = RectAllTop;             //Прям. область рисования
            PanelPrices.minStepPrice = this.MinStepPrice;   //Шаг цены

            //Получаем правую область для шкалы с ценами
            var rectForPrices = RectanglePaint.GetRight(RectAllTop, PanelPrices.WidthBorder);
            //Получаем область для свечек из общей верхней области.
            var RectCandle = RectanglePaint.DeleteRight(RectAllTop, rectForPrices);

            //Получаем правую область для шкалы с гор. объемами (РАЗНИЦА)
            var RectForDiffHVolumes = RectanglePaint.GetRight(RectCandle, 70);
            //Получаем область для свечек из общей верхней области.
            RectCandle = RectanglePaint.DeleteRight(RectCandle, RectForDiffHVolumes);

            //Получаем правую область для шкалы с гор. объемами
            var RectForHVolumes = RectanglePaint.GetRight(RectCandle, 70);
            //Получаем область для свечек из общей верхней области.
            RectCandle = RectanglePaint.DeleteRight(RectCandle, RectForHVolumes);

            int BorderBottomIndicator = RectForHVolumes.Width + RectForDiffHVolumes.Width + rectForPrices.Width;

            //BottomRectForVol.Width = RectCandle.Width;

            //Получение прямоугольника для области временных меток
            BottomRectForTimeLine = RectanglePaint.AttachBottom(RectCandle, BottomRectForTimeLine);
            //Получение области для рисования временных линий и меток
            BottomRectForTimeLine = RectanglePaint.AttachBottom(BottomRectForTimeLine, BottomRectForVol);

            //Ширина свечки
            int WidthOneCandle = PanelCandels.GetWidthOne(RectCandle);

            //Настройки для объемов
            PanelVolumes.WidthOneLevel = WidthOneCandle;
            PanelVolumes.PanelValues.WidthBorder = BorderBottomIndicator;// PanelPrices.WidthBorder;
            PanelVolumes.PanelValues.CountBorderPrice = 4;

            PanelDiffHorVolumes.PanelValues.CountBorderPrice = 3;
            PanelHorVolumes.PanelValues.CountBorderPrice = 3;

            PanelTimes.WidthOneCandle = WidthOneCandle;
            PanelTimes.CollectionCandle = PanelCandels.CollectionCandle;

            this.Canvas.Clear(Color.White);
            //Рисуем линии времени
            PanelTimes.PaintTimeFrame(this.Canvas, BottomRectForTimeLine);
            //Рисуем линии цен
            PanelPrices.PaintPrices(this.Canvas, this.MaxPrice, this.MinPrice);

            //Рисуем объемы
            PanelVolumes.PaintLevels(this.Canvas, BottomRectForVol);
            //Рисуем объемы горизонтальные
            PanelHorVolumes.PaintLevels(this.Canvas, RectForHVolumes, this.MaxPrice, this.MinPrice);
            //Рисуем объемы горизонтальные РАЗНИЦА
            PanelDiffHorVolumes.PaintLevels(this.Canvas, RectForDiffHVolumes, this.MaxPrice, this.MinPrice);
            
            //Рисуем область свечей
            PanelCandels.PaintCandles(this.Canvas, RectCandle, this.MaxPrice, this.MinPrice, this.MinStepPrice);
            //Рисуем линию последней цены
            PanelPrices.PaintCurrentValue(this.Canvas, this.MaxPrice, this.MinPrice);
            //Рисуем заявки
            Orders.PaintOrders(this.Canvas, RectAllTop, this.MaxPrice, this.MinPrice, this.MinStepPrice);
        }
        /// <summary> Активная свеча, на которую наведена мышь </summary>
        private Candles.DataCandle ActiveCandle = null;
        /// <summary> Инициализируем внутренние события </summary>
        private void InitEvents()
        {
            //Перед отриосвкой свечей
            PanelCandels.OnBeforePaintCandle += (emptyCandle) =>
            {
                this.ActiveCandle = null;
            };
            //Отрисовка одной свечи
            PanelCandels.OnPaintCandle += (candle) =>
            {
                if (candle.Body.X <= this.CrossLine.X && candle.Body.X + candle.Body.Width >= this.CrossLine.X)
                {
                    this.ActiveCandle = candle;
                }
                if (candle.Index == this.CountCandleShowHVol)
                {
                    Point p1 = new Point() { X = candle.Body.X, Y = candle.PaintRect.Y };
                    Point p2 = new Point() { X = candle.Body.X, Y = candle.PaintRect.Y + candle.PaintRect.Height };
                    GraphicShape.PaintVLine(this.Canvas, candle.PaintRect, candle.Index.ToString(), p1, p2, Color.Blue, 2);
                }
            };
            //Отрисовка свечей завершена
            PanelCandels.OnPaintedCandles += () =>
            {
                if (!this.ActiveCandle.IsNull())
                {
                    this.PaintHorVolByCandle(this.Canvas, ActiveCandle, this.CrossLine);
                    this.PanelCandels.MoveVerticalActiveCandle(this.ActiveCandle);
                }
                this.PaintCrossLines(this.CrossLine, ActiveCandle);
            };
        }

        
        /// <summary> Списко для формирования временной коллекции объемов, для ускорения </summary>
        private List<CandleLib.CandleData> LisForHVol = null;
        /// <summary> Расчет горизонтальных объемов (для ускорения) </summary>
        /// <param name="collection"></param>
        public void CalculationHVol(IEnumerable<CandleLib.CandleData> collection)
        {
            if (collection.IsNull()) return;
            
            this.LisForHVol = new List<CandleLib.CandleData>();
            CandleLib.CandleData mainCandle = new CandleLib.CandleData(DateTime.Now);
            int indx = 0;
            foreach (var el in collection)
            {
                if (indx > 0)
                {
                    mainCandle.High = mainCandle.High < el.High ? el.High : mainCandle.High;
                    mainCandle.Low = mainCandle.Low > el.Low ? el.Low : mainCandle.Low;

                    if (el.HorVolumes.HVolCollection.Count > 0)
                    {
                        foreach (var hv in el.HorVolumes.HVolCollection.CollectionArray)
                        {
                            mainCandle.HorVolumes.AddBuy(hv.Price, hv.VolBuy);
                            mainCandle.HorVolumes.AddSell(hv.Price, hv.VolSell);
                        }
                    }
                }
                indx++;
            }
            this.LisForHVol.Add(new CandleLib.CandleData(DateTime.Now));//Первая свечка изменяемая
            this.LisForHVol.Add(mainCandle);             //Последующие расчитываются один раз
        }
        /// <summary> Устанавливает спискок заявок </summary>
        /// <param name="ordersLine"></param>
        public void SetOrders(IEnumerable<MarketObject.Chart> ordersLine)
        {
            Orders.CollectionOrders = ordersLine;
        }
        /// <summary> Рисует объемы по активной свечке </summary>
        private void PaintHorVolByCandle(Graphics canvas, Candles.DataCandle activeCandle, Point crossLines)
        {
            if (activeCandle.Candle.HorVolumes.HVolCollection.Count == 0) return;
            Rectangle rectPaint = new Rectangle();
            rectPaint.X = activeCandle.TailCoord.High.X + 1;
            rectPaint.Width = 30 * 2;
            rectPaint.Y = activeCandle.TailCoord.High.Y;
            rectPaint.Height = activeCandle.TailCoord.Low.Y - rectPaint.Y;
            
            long MaxVol = activeCandle.Candle.HorVolumes.HVolCollection.CollectionArray.Max(e => e.VolBuy + e.VolSell);
            long MinVol = 0;

            SolidBrush solidBrush = new SolidBrush(Color.LightGray);
            canvas.FillRectangle(solidBrush, rectPaint);
            activeCandle.Candle.HorVolumes.HVolCollection.CollectionArray.ForEach<MarketObject.ChartVol>((hv) =>
            {
                int y = GraphicShape.GetCoordinate(this.RectAllTop.Height, this.MaxPrice, this.MinPrice, hv.Price);
                int x2 = (int)GraphicShape.GetCoordinate(rectPaint.Width, MaxVol, MinVol, hv.VolBuy + hv.VolSell);
                var p1 = new Point(rectPaint.X, y);
                var p2 = new Point(rectPaint.X + rectPaint.Width - x2, y);
                if (hv.VolBuy + hv.VolSell == MaxVol)
                {
                    GraphicShape.PaintText(canvas, MaxVol.ToString(), activeCandle.TailCoord.High.X, activeCandle.TailCoord.High.Y - 11, Color.Blue);
                }
                if (crossLines.Y + 5 > y && crossLines.Y - 5 < y)
                {
                    decimal priceY = GraphicShape.GetValueFromCoordinate(this.RectAllTop.Height, this.MaxPrice, this.MinPrice, crossLines.Y, this.PanelPrices.CountFloat);
                    if (hv.Price == priceY) activeCandle.Description = (hv.VolBuy + hv.VolSell).ToString();
                }
                GraphicShape.PaintLine(canvas, p1, p2, Color.Blue, 2);
            });
        }

        /// <summary>Отрисовка перекрестья</summary>
        /// <param name="coord"></param>
        void PaintCrossLines(Point coord, Candles.DataCandle candle)
        {
            if (candle.IsNull()) return;
            string d = candle.Candle.Time.Day.ToString();
            string m = candle.Candle.Time.Month.ToString();
            string y = candle.Candle.Time.Year.ToString();
            string min = candle.Candle.Time.Minute.ToString();
            string hour = candle.Candle.Time.Hour.ToString();
            string time = (d.Length < 2 ? '0' + d : d) + "." + (m.Length < 2 ? '0' + m : m) + "." + y + " " +
                (hour.Length < 2 ? '0' + hour : hour) + ":" + (min.Length < 2 ? '0' + min : min) + " (" + candle.Description + ")";
            //GraphicShape.PaintLine(this.Canvas, new Point(coord.X, 0), new Point(coord.X, 1000), Color.Black);
            decimal priceY = GraphicShape.GetValueFromCoordinate(this.RectAllTop.Height, this.MaxPrice, this.MinPrice, coord.Y, this.PanelPrices.CountFloat);
            GraphicShape.PaintVLine(this.Canvas, this.RectAllTop, time, new Point(coord.X, 0), new Point(coord.X, this.RectAllTop.Height), Color.Black);
            GraphicShape.PaintHLine(this.Canvas, this.RectAllTop, priceY, this.MaxPrice, this.MinPrice, Color.Black);
        }

        public class LevelsOrder
        {
            public IEnumerable<MarketObject.Chart> CollectionOrders = null;

            public void PaintOrders(Graphics canvas, Rectangle rectPaint, decimal MaxPrice, decimal MinPrice, decimal minStepPrice)
            {
                int count = this.CollectionOrders.Count();
                if (count == 0) return;

                foreach (var ord in this.CollectionOrders)
                {
                    var vol = ord.Volume < 0 ? ord.Volume * -1 : ord.Volume;
                    GraphicShape.PaintHLine(canvas, rectPaint, ord.Price, ord.Price + "(" + vol + ")",
                        MaxPrice, MinPrice, ord.Volume > 0 ? Color.DarkGreen : Color.DarkRed, "left");
                }
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////
        public class BaseLevels
        {
            /// <summary> Область значений шкалы </summary>
            public Prices PanelValues = null;
            /// <summary> Ширина одного уровня </summary> 
            public int WidthOneLevel = 2;
            /// <summary> Коллекция для отрисовки уровней </summary>
            public IEnumerable<MarketObject.Chart> CollectionLevels = null;
            /// <summary> Минимальное значение </summary>
            public decimal Min = -100000000;
            /// <summary> Максимальное значение </summary>
            public decimal Max = 0;
            /// <summary> Минимальный шаг цены</summary>
            public decimal MinStepPrice = 1;


            /// <summary> Кол-во уровней </summary>
            protected int CountLevels = 0;
        }
        /// <summary>
        /// Горизонтальные уровни, обычно расположенные снизу от графика.
        /// </summary>
        public class HorLevels : BaseLevels
        {
            public HorLevels(decimal minStepPrice = 1, int Scale = 0)
            {
                this.PanelValues = new Prices(Scale);
                this.PanelValues.ColorLines = Color.Gray;
            }
            public void PaintLevels(Graphics canvas, Rectangle rectPaint, decimal MaxPrice, decimal MinPrice)
            {
                if (this.CollectionLevels == null) return;
                this.CountLevels = this.CollectionLevels.Count();
                if (this.CountLevels == 0) return;

                foreach (var valLevel in this.CollectionLevels)
                {
                    //*****************************
                    this.PaintOneLevel(canvas, rectPaint, valLevel, MaxPrice, MinPrice);
                }

                GraphicShape.PaintLine(canvas, new Point(rectPaint.X, rectPaint.Y), new Point(rectPaint.X, rectPaint.Y + rectPaint.Height), Color.Black);

                GraphicShape.PaintText(canvas, "min:" + this.Min.ToString(), rectPaint.X, rectPaint.Y + rectPaint.Height - 12, Color.Black);
                GraphicShape.PaintText(canvas, "max:" + this.Max.ToString(), rectPaint.X, rectPaint.Y + 1, Color.Black);
            }

            private void PaintOneLevel(Graphics canvas, Rectangle rectPaint, MarketObject.Chart Value, decimal MaxPrice, decimal MinPrice)
            {
                int y = GraphicShape.GetCoordinate(rectPaint.Height, MaxPrice, MinPrice, Value.Price);
                int x1 = rectPaint.X + GraphicShape.GetCoordinate(rectPaint.Width, this.Max, this.Min, Value.Volume > 0 ? 0 : Value.Volume);
                int x2 = rectPaint.X + GraphicShape.GetCoordinate(rectPaint.Width, this.Max, this.Min, Value.Volume < 0 ? 0 : Value.Volume);

                GraphicShape.PaintLine(canvas, new Point(x1, y), new Point(x2, y), Value.Volume < 0 ? Color.Red : Color.Green, 2);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Вертикальные уровни, обычно расположенные снизу от графика.
        /// </summary>
        public class VertLevels : BaseLevels
        {
            /// <summary> Минимально кол-во отображаемых уровней </summary>
            public int MinCountLevels = 5;
            public VertLevels(decimal minStepPrice = 1, int Scale = 0)
            {
                PanelValues = new Prices(Scale);
                PanelValues.ColorLines = Color.Gray;
                this.MinStepPrice = minStepPrice;
            }

            public void PaintLevels(Graphics canvas, Rectangle rectPaint)
            {
                this.CountLevels = this.CollectionLevels.Count();
                if (this.CountLevels == 0) return;
                if (this.CountLevels < MinCountLevels) this.CountLevels = MinCountLevels;

                int index = 1;
                foreach (var valLevel in this.CollectionLevels)
                {
                    //*****************************
                    this.PaintOneLevel(canvas, rectPaint, valLevel.Volume, index);
                    index++;
                }

                GraphicShape.PaintLine(canvas, new Point(rectPaint.X, rectPaint.Y), new Point(rectPaint.X + rectPaint.Width, rectPaint.Y), Color.Black);

                PanelValues.CurrentValue = this.CollectionLevels.ElementAt(0).Volume;
                PanelValues.RectPaint = rectPaint;
                PanelValues.ColorLines = Color.DarkGray;
                PanelValues.minStepPrice = this.MinStepPrice;
                PanelValues.PaintPrices(canvas, this.Max, this.Min);
                PanelValues.PaintCurrentValue(canvas, this.Max, this.Min);
            }

            private void PaintOneLevel(Graphics canvas, Rectangle rectPaint, decimal Value, int index)
            {
                rectPaint = RectanglePaint.DeleteRight(rectPaint, new Rectangle(0, 0, this.PanelValues.WidthBorder, 0));
                int Y1 = GraphicShape.GetCoordinate(rectPaint.Height, this.Max, this.Min, Value > 0 ? Value : 0);
                int Y2 = GraphicShape.GetCoordinate(rectPaint.Height, this.Max, this.Min, Value < 0 ? Value : 0);

                int bodyX = (int)(rectPaint.Width - this.WidthOneLevel * index);
                int bodyY = rectPaint.Y + Y1;

                int bodyWidth = this.WidthOneLevel - 2; //-2 чтобы свечки не слипались
                int bodyHeight = Y2 - Y1;
                //bodyHeight = bodyHeight - bodyY;
                bodyHeight = bodyHeight == 0 ? bodyHeight + 1 : bodyHeight;

                GraphicShape.PaintRectangle(canvas, bodyX, bodyY, bodyWidth, bodyHeight, Color.Black, Value < 0 ? Color.Red : Color.Green);
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace NSGraphic
{
    ////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Класс отвечающий за свечки </summary>
    public class Candles
    {
        /// <summary> Минимальное кол-во свечек на графике</summary>
        const int MinCountCandle = 5;
        /// <summary> отступ между свечами </summary>
        private int MarginCandle = 2;
        public struct TailCoord
        {
            public Point High;
            public Point Low;
        }
        /// <summary>  Данные по свечке  </summary>
        public class DataCandle
        {
            /// <summary> Прямоугольник отрисовки </summary>
            public Rectangle PaintRect;
            /// <summary> Данные по свечке </summary>
            public CandleLib.CandleData Candle;
            /// <summary> Соординаты хвостов </summary>
            public TailCoord TailCoord;
            /// <summary> Координаты тела свечи </summary>
            public Rectangle Body;
            /// <summary> Индекс свечи </summary>
            public int Index = -1;
            public string Description = "";
        }

        public delegate void EventPointCandle(DataCandle dataCandle);
        /// <summary> Событие рисования свечки </summary>
        public event EventPointCandle OnPaintCandle;
        /// <summary> Перед отрисовкой свечей </summary>
        public event EventPointCandle OnBeforePaintCandle;
        /// <summary> Событие нахождения указателя мыши на вертикале свечки </summary>
        public event EventPointCandle OnMoveVerticalCandle;

        public delegate void EventPaintedCandles();
        /// <summary> Событие после отрисовки свечей </summary>
        public event EventPaintedCandles OnPaintedCandles = null;

        public delegate void EventPointHorVolumes(int Y, decimal Price, long Volume);
        /// <summary> Событие рисования линии горизонтального объема </summary>
        public event EventPointHorVolumes OnPaintHorVolume = null;

        /// <summary> Коллекция свечек для отображения </summary>
        public IEnumerable<CandleLib.CandleData> CollectionCandle = null;

        /// <summary> Кол-во рисуемых свечей (Масштаб) </summary>
        public int CountPaintCandle = 0;
        /// <summary> Ширина одной свечки </summary>
        private int WidthOneCandle = 0;

        public Candles() { }


        /// <summary> Получить ширину одной свечки </summary>
        /// <returns></returns>
        public int GetWidthOne(Rectangle rectPaint)
        {
            int c = CollectionCandle.Count();
            this.CountPaintCandle = this.CountPaintCandle > c ? this.CountPaintCandle : c;

            this.CountPaintCandle = this.CountPaintCandle < MinCountCandle ? MinCountCandle : this.CountPaintCandle;
            this.WidthOneCandle = (int)(rectPaint.Width / this.CountPaintCandle);
            return this.WidthOneCandle;
        }
        /// <summary> Вызываем событие активной свечки </summary>
        /// <param name="candle"></param>
        public void MoveVerticalActiveCandle(Candles.DataCandle candle)
        {
            if(!this.OnMoveVerticalCandle.IsNull())
                this.OnMoveVerticalCandle(candle);
        }


        /// <summary> Отрисовывет свечи</summary>
        /// <param name="canvas"></param>
        /// <param name="rectPaint"></param>
        /// <param name="MaxPrice"></param>
        /// <param name="MinPrice"></param>
        /// <param name="minStepPrice"></param>
        public void PaintCandles(Graphics canvas, Rectangle rectPaint, decimal MaxPrice, decimal MinPrice, decimal minStepPrice)
        {
            if (this.CountPaintCandle == 0) return;

            this.WidthOneCandle = GetWidthOne(rectPaint);

            //Событие перед отрисовкой
            if (!OnBeforePaintCandle.IsNull())
                OnBeforePaintCandle(null);

            List<MarketObject.Chart> HVolume = new List<MarketObject.Chart>();
            int index = 1;
            CollectionCandle.ForEach<CandleLib.CandleData>((candleData) =>
            {
                this.PaintOneCandle(canvas, rectPaint, candleData, index, MaxPrice, MinPrice);
                index++;
            });

            if (!OnPaintedCandles.IsNull())
                OnPaintedCandles();
        }
        /// <summary> Рисует одну свечку </summary>
        /// <param name="canvas"></param>
        /// <param name="rectPaint"></param>
        /// <param name="candleData"></param>
        /// <param name="index"></param>
        /// <param name="maxPrice"></param>
        /// <param name="minPrice"></param>
        private void PaintOneCandle(Graphics canvas, Rectangle rectPaint, CandleLib.CandleData candleData, int index, decimal maxPrice, decimal minPrice)
        {
            int tailY1 = GraphicShape.GetCoordinate(rectPaint.Height, maxPrice, minPrice, candleData.High);
            int tailY2 = GraphicShape.GetCoordinate(rectPaint.Height, maxPrice, minPrice, candleData.Low);
            int tailX1 = (int)((rectPaint.Width - this.WidthOneCandle * index) + this.WidthOneCandle / 2);

            int bodyX = (int)(rectPaint.Width - this.WidthOneCandle * index);
            int bodyY = GraphicShape.GetCoordinate(rectPaint.Height, maxPrice, minPrice, candleData.Open > candleData.Close ? candleData.Open : candleData.Close);

            int bodyWidth = this.WidthOneCandle - MarginCandle; //- чтобы свечки не слипались
            int bodyHeight = GraphicShape.GetCoordinate(rectPaint.Height, maxPrice, minPrice, candleData.Open < candleData.Close ? candleData.Open : candleData.Close);
            bodyHeight = bodyHeight - bodyY;
            bodyHeight = bodyHeight == 0 ? bodyHeight + 1 : bodyHeight;

            GraphicShape.PaintLine(canvas, new Point(tailX1, tailY1), new Point(tailX1, tailY2), Color.Black, 2);
            GraphicShape.PaintRectangle(canvas, bodyX, bodyY, bodyWidth, bodyHeight, Color.Black, candleData.Open > candleData.Close ? Color.LightCoral : Color.LightGreen);

            if (OnPaintCandle != null)
            {
                OnPaintCandle(new DataCandle()
                {
                    PaintRect = rectPaint,
                    Candle = candleData,
                    TailCoord = new TailCoord() { High = new Point(tailX1, tailY1), Low = new Point(tailX1, tailY2) },
                    Body = new Rectangle() { X = bodyX, Y = bodyY, Width = bodyWidth, Height = bodyHeight },
                    Index = index
                });
            }
        }
    }
}

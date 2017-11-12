using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSGraphic
{
    public class RectanglePaint
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;

        public static RectanglePaint operator +(RectanglePaint r1, RectanglePaint r2)
        {
            RectanglePaint res = new RectanglePaint();
            res.X = r1.X < r2.X ? r1.X : r2.X;
            res.Y = r1.Y < r2.Y ? r1.Y : r2.Y;
            res.Width = r1.X + r1.Width > r2.X + r2.Width ? r1.X + r1.Width - res.X : r2.X + r2.Width - res.X;
            res.Height = r1.X + r1.Height > r2.Y + r2.Height ? r1.Y + r1.Height - res.Y : r2.Y + r2.Height - res.Y;
            return res;
        }

        /// <summary>
        /// Обрезает в r1 область шириной r2, справа.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static Rectangle DeleteRight (Rectangle r1, Rectangle r2)
        {
            Rectangle res = new Rectangle();
            res.X = r1.X;
            res.Y = r1.Y;
            res.Width = r1.Width - r2.Width;
            res.Height = r1.Height;
            return res;
        }


        /// <summary>
        /// Обрезает в r1 область высотой r2, снизу.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static Rectangle DeleteBottom(Rectangle r1, Rectangle r2)
        {
            Rectangle res = new Rectangle();
            res.X = r1.X;
            res.Y = r1.Y;
            res.Width = r1.Width;
            res.Height = r1.Height - r2.Height;
            return res;
        }
        /// <summary>
        /// Добавляет к области r1, область высотой r2, снизу.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static Rectangle AttachBottom(Rectangle r1, Rectangle r2)
        {
            Rectangle res = new Rectangle();
            res.X = r1.X;
            res.Y = r1.Y;
            res.Width = r1.Width;
            res.Height = r1.Height + r2.Height;

            return res;
        }

        /// <summary>
        /// Добавляет к области r1, область высотой r2, справа.
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static Rectangle AttachRight(Rectangle r1, Rectangle r2)
        {
            Rectangle res = new Rectangle();
            res.X = r1.X;
            res.Y = r1.Y;
            res.Width = r1.Width + r2.Width;
            res.Height = r1.Height;

            return res;
        }
        public static Rectangle AttachRight(Rectangle r1, int Width)
        {
            Rectangle res = new Rectangle();
            res.X = r1.X;
            res.Y = r1.Y;
            res.Width = r1.Width + Width;
            res.Height = r1.Height;

            return res;
        }

        /// <summary>
        /// Выделить область прямоугольную, справа в текущем прямоугольнике, указанной ширины.
        /// </summary>
        /// <param name="mainRetc"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Rectangle GetRight(Rectangle mainRect, int width)
        {
            Rectangle res = new Rectangle();
            res.X = mainRect.X + mainRect.Width - width;
            res.Y = mainRect.Y;
            res.Width = width;
            res.Height = mainRect.Height;

            mainRect.Width -= width;
            return res;
        }
        /// <summary>
        /// Выделить область прямоугольную, снизу в текущем прямоугольнике, указанной высоты.
        /// </summary>
        /// <param name="mainRect"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rectangle GetBottom(Rectangle mainRect, int height)
        {
            Rectangle res = new Rectangle();
            res.X = mainRect.X;
            res.Y = mainRect.Y + mainRect.Height - height;
            res.Width = mainRect.Width;
            res.Height = height;

            return res;
        }

        public static bool operator !=(RectanglePaint r1, RectanglePaint r2)
        {
            if (Object.Equals(r1, null) && !Object.Equals(r2, null)) return true;
            if (Object.Equals(r2, null) && !Object.Equals(r1, null)) return true;
            if (Object.Equals(r1, null) && Object.Equals(r2, null)) return false;
            if (r1.Width != r2.Width) return true;
            if (r1.Height != r2.Height) return true;
            if (r1.X != r2.X) return true;
            if (r1.Y != r2.Y) return true;
            return false;
        }

        public static bool operator ==(RectanglePaint r1, RectanglePaint r2)
        {
            if (Object.Equals(r1, null) && !Object.Equals(r2, null)) return false;
            if (Object.Equals(r2, null) && !Object.Equals(r1, null)) return false;
            if (Object.Equals(r1, null) && Object.Equals(r2, null)) return true;
            if (r1.Width == r2.Width && r1.Height == r2.Height && r1.X == r2.X && r1.Y == r2.Y) return true;
            return false;
        }
    }

    public class GraphicShape
    {
        //Прямоугольники отрисовки 

        public static Rectangle PaintRectangle(Graphics g, int x, int y, int width, int height)
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            g.FillRectangle(new SolidBrush(Color.White), rect);
            g.DrawRectangle(new Pen(Color.Black, .5f), rect);
            return rect;
        }
        public static Rectangle PaintRectangle(Graphics g, int x, int y, int width, int height, Color colorBorder, Color fillColor)
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            g.FillRectangle(new SolidBrush(fillColor), rect);
            g.DrawRectangle(new Pen(colorBorder, .5f), rect);
            return rect;
        }
        public static void PaintLine(Graphics g, Point pointStart, Point pointEnd, Color color, int width = 1)
        {
            g.DrawLine(new Pen(color, width), pointStart, pointEnd);
        }

        /// <summary>
        /// Рисует горизонтальную линию с надписью цены
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectPaint"></param>
        /// <param name="maxPrice"></param>
        /// <param name="minPrice"></param>
        public static void PaintHLine(Graphics g, Rectangle rectPaint, decimal valPrice, decimal maxPrice, decimal minPrice, Color color, string TextHAlign = "right")
        {
            PaintHLine(g, rectPaint, valPrice, valPrice.ToString(), maxPrice, minPrice, color, TextHAlign);
        }

        public static void PaintHLine(Graphics g, Rectangle rectPaint, decimal valPrice, string Text, decimal maxPrice, decimal minPrice, Color color, string TextHAlign = "right")
        {
            int Y = GraphicShape.GetCoordinate(rectPaint.Height, maxPrice, minPrice, valPrice);
            //Если выходит за пределы, удаляем
            if (Y < 0 || Y > rectPaint.Y + rectPaint.Height) return;

            string ValText = Text;
            int WidthText = ValText.Length * 7;
            int fontSize = 8;
            Point pLine1 = new Point(rectPaint.X, rectPaint.Y + Y);
            Point pLine2 = new Point(rectPaint.X + rectPaint.Width - WidthText - 2, rectPaint.Y + Y);
            //ВЫравнивание лейбла
            switch (TextHAlign)
            {
                case "left":
                    g.DrawString(ValText,
                                new Font(new FontFamily("Helvetica"), fontSize, FontStyle.Regular, GraphicsUnit.Point),
                                new SolidBrush(color), rectPaint.X, rectPaint.Y + Y - (fontSize));
                    pLine1 = new Point(rectPaint.X + WidthText, rectPaint.Y + Y);
                    pLine2 = new Point(rectPaint.X + rectPaint.Width, rectPaint.Y + Y);
                    break;
                case "center":
                    //Canvas.SetLeft(this.TextLabel, PaintPanel.X + (PaintPanel.Width / 2 - this.TextLabel.ActualWidth));
                    break;
                default:
                    // Canvas.SetLeft(this.TextLabel, PaintPanel.X + PaintPanel.Width - this.TextLabel.ActualWidth - this.MarginRight);
                    g.DrawString(ValText,
                                new Font(new FontFamily("Helvetica"), fontSize, FontStyle.Regular, GraphicsUnit.Point),
                                new SolidBrush(color), rectPaint.X + rectPaint.Width - WidthText, rectPaint.Y + Y - (fontSize));
                    break;
            }
            GraphicShape.PaintLine(g, pLine1, pLine2, color);
        }
        /// <summary>
        /// Рисует текст
        /// </summary>
        /// <param name="g"></param>
        /// <param name="text"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="color"></param>
        /// <param name="TextHAlign"></param>
        /// <param name="fontSize"></param>
        public static void PaintText(Graphics g, string text, int X, int Y, Color color, string TextHAlign = "right", int fontSize = 8)
        {
            g.DrawString(text,
                        new Font(new FontFamily("Helvetica"), fontSize, FontStyle.Regular, GraphicsUnit.Point),
                        new SolidBrush(color), X, Y);
        }
        /// <summary>
        /// Рисует вертикальнцю линию с надписью
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectPaint"></param>
        /// <param name="ValText"></param>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <param name="color"></param>
        /// <param name="WidthLine"></param>
        /// <param name="TextVAlign"></param>
        public static void PaintVLine(Graphics g, Rectangle rectPaint, string ValText, Point pStart, Point pEnd, Color color, int WidthLine = 1,string TextVAlign = "bottom")
        {
            int WidthText = Convert.ToInt32(ValText.Length * 6.5);
            int HeightText = 12;
            pEnd.Y -= HeightText;
            //ВЫравнивание лейбла
            switch (TextVAlign)
            {
                case "bottom":
                    int fontSize = 8;
                    g.DrawString(ValText,
                                new Font(new FontFamily("Helvetica"), fontSize, FontStyle.Regular, GraphicsUnit.Point),
                                new SolidBrush(color), pEnd.X - WidthText / 2, pEnd.Y);                  
                    break;
            }

            GraphicShape.PaintLine(g, pStart, pEnd, color, WidthLine);
        }

        //Получение координат по Y относительно полотна, по цене
        public static int GetCoordinate(double Height, decimal maxValue, decimal minValue, decimal Value)
        {
            if (Value > maxValue) return -1;
            if (Value < minValue) return -1;
            decimal Interval = maxValue - minValue;
            if (Interval == 0) return 0;
            double percentValue = (double)((Value - minValue) * 100 / Interval);
            percentValue = percentValue < 0 ? percentValue * -1 : percentValue;
            return (int)(Height - (Height * percentValue / 100));
        }

        /// <summary>
        /// Получение значения между МИН и МАКС, по значению координат
        /// </summary>
        /// <param name="Length"></param>
        /// <param name="maxValue"></param>
        /// <param name="minValue"></param>
        /// <param name="ValueCoordinate"></param>
        /// <returns></returns>
        public static decimal GetValueFromCoordinate(int Length, decimal maxValue, decimal minValue, decimal ValueCoordinate, int scale = 2)
        {
            if (ValueCoordinate > Length) return -1;
            if (ValueCoordinate < 0) return -1;

            decimal Interval = maxValue - minValue;

            if (Interval == 0) return 0;

            decimal percentValue = ValueCoordinate * 100 / Length;
            percentValue = percentValue < 0 ? percentValue * -1 : percentValue;

            percentValue = 100 - percentValue;

            return Math.Round(minValue + (Interval * percentValue / 100), scale);
        }

        /*


                //Получение координат по Y относительно полотна, по цене
                protected double GetCoordinate(double Height, decimal maxValue, decimal minValue, decimal Value)
                {
                    if (Value > maxValue) return -1;
                    if (Value < minValue) return -1;
                    decimal Interval = maxValue - minValue;
                    if (Interval == 0) return 0;
                    double percentValue = (double)((Value - minValue) * 100 / Interval);
                    percentValue = percentValue < 0 ? percentValue * -1 : percentValue;
                    return Height - (Height * percentValue / 100);
                }

                //Получение координат по Y относительно полотна, по цене
                public decimal GetValueFromCoordinate(decimal Length, decimal maxValue, decimal minValue, decimal ValueCoordinate)
                {
                    if (ValueCoordinate > Length) return -1;
                    if (ValueCoordinate < 0) return -1;

                    decimal Interval = maxValue - minValue;

                    if (Interval == 0) return 0;

                    decimal percentValue = ValueCoordinate * 100 / Length;
                    percentValue = percentValue < 0 ? percentValue * -1 : percentValue;

                    percentValue = 100 - percentValue;

                    return minValue + (Interval * percentValue / 100);
                }
            }

            /// <summary>
            /// Горизонтальные лиинии
            /// </summary>
            public class HorizontLine : GraphicLine
            {
                public decimal Price = 0;

                private string TextHAlign = "right";
                public HorizontLine(decimal price = 0)
                {
                    this.InitLine();
                    this.ChangeHLine(price);
                }

                public void ChangeHLine(decimal price, string format = "")
                {
                    this.Price = price;
                    this.TextLabel.Text = price.ToString(format);
                }

                public void SetTextAlign(string valueAlign)
                {
                    this.TextHAlign = valueAlign;
                }

                public void PaintHLine(Canvas Canvas, RectanglePaint PaintPanel, decimal maxPrice, decimal minPrice, int zIndex)
                {
                    this.PaintLine(Canvas, zIndex);

                    double Y = this.GetCoordinate(PaintPanel.Height, maxPrice, minPrice, this.Price);
                    //Если выходит за пределы, удаляем
                    if (Y < 0)
                    {
                        this.Remove(Canvas);
                        return;
                    }

                    this._Line.X1 = PaintPanel.X;
                    this._Line.X2 = PaintPanel.Width;

                    this._Line.Y1 = PaintPanel.Y + Y;
                    this._Line.Y2 = this._Line.Y1;

                    //ВЫравнивание лейбла
                    switch (this.TextHAlign)
                    {
                        case "right":
                            Canvas.SetLeft(this.TextLabel, PaintPanel.X + PaintPanel.Width - this.TextLabel.ActualWidth - this.MarginRight);
                            break;
                        case "left":
                            Canvas.SetLeft(this.TextLabel, PaintPanel.X + this.MarginLeft);
                            break;
                        case "center":
                            Canvas.SetLeft(this.TextLabel, PaintPanel.X + (PaintPanel.Width / 2 - this.TextLabel.ActualWidth));
                            break;
                    }
                    double yEnd = this._Line.Y1 - this.TextLabel.ActualHeight;
                    if (yEnd < 0) yEnd = 0;
                    Canvas.SetTop(this.TextLabel, yEnd);
                }
            }


            /// <summary>
            /// Вертикальные лиинии
            /// </summary>
            public class VerticalLine : GraphicLine
            {
                public VerticalLine()
                {
                    this.InitLine();
                }

                public void ChangeVLine(double x, string text = "")
                {
                    if (this._Line != null)
                    {
                        this._Line.X1 = x;
                        this._Line.X2 = x;

                        this.ChangeTextLabel(text);
                    }
                }

                public void PaintVLine(Canvas Canvas, RectanglePaint PaintPanel, int zIndex = 0)
                {
                    this.PaintLine(Canvas, zIndex);
                    this._Line.Y1 = PaintPanel.Y;

                    if (!this.Rotate90)
                    {
                        Canvas.SetLeft(this.TextLabel, this._Line.X1 - (this.TextLabel.ActualWidth / 2));
                        Canvas.SetTop(this.TextLabel, PaintPanel.Y + PaintPanel.Height - this.TextLabel.ActualHeight - this.MarginTop);

                        if (this.TextLabel.IsVisible)
                            this._Line.Y2 = PaintPanel.Y + PaintPanel.Height - this.TextLabel.ActualHeight;
                        else
                            this._Line.Y2 = PaintPanel.Y + PaintPanel.Height;
                    }
                    else
                    {
                        Canvas.SetLeft(this.TextLabel, this._Line.X1 - this.TextLabel.ActualHeight);
                        Canvas.SetTop(this.TextLabel, PaintPanel.Y + PaintPanel.Height - this.MarginTop);

                        if (this.TextLabel.IsVisible)
                            this._Line.Y2 = PaintPanel.Y + PaintPanel.Height - this.TextLabel.ActualWidth;
                        else
                            this._Line.Y2 = PaintPanel.Y + PaintPanel.Height;
                    }
                }
            }





            /// <summary>
            /// Класс рисующий линии с текстовым комментарием
            /// </summary>
            public class GraphicLine : Shapes
            {
                protected string Label = "";
                protected Line _Line = null;
                protected TextBlock TextLabel = null;
                protected double MarginTop = 2;
                protected double MarginRight = 2;
                protected double MarginLeft = 2;

                protected bool Rotate90 = false;

                protected void InitLine()
                {
                    this._Line = this.getLine(0, 0, 0, 0);
                    this._Line.UpdateLayout();
                    this.TextLabel = new TextBlock();
                    this.TextLabel.Text = Label.ToString();
                    this.ChangeStyle(Brushes.Black, 0.5); //new List<double>() { 10, 1}
                    this.TextLabel.UpdateLayout();
                }

                public void BackgroundLabel(Brush bgColor)
                {
                    this.TextLabel.Background = bgColor;
                }

                public void ChangeStyle(Brush Color, double StrokeThickness = 1, IEnumerable<double> StrokeDashArray = null, double TextSize = 12)
                {
                    this._Line.Stroke = Color;
                    if (StrokeDashArray != null) this._Line.StrokeDashArray = new DoubleCollection(StrokeDashArray);
                    this._Line.StrokeThickness = StrokeThickness;

                    this.TextLabel.Foreground = Color;
                    this.TextLabel.FontSize = TextSize;
                }

                public void Rotation90(bool rotate90 = true)
                {
                    this.Rotate90 = rotate90;
                }

                protected void PaintLine(Canvas Canvas, int zIndex = 0)
                {
                    if (this._Line.Parent == null)
                    {
                        Canvas.Children.Add(this._Line);
                        Canvas.UpdateLayout();
                    }
                    if (this.TextLabel.Parent == null)
                    {
                        Canvas.Children.Add(this.TextLabel);
                        Canvas.UpdateLayout();
                    }
                    //Указываем zIndex порядка отрисовки
                    Canvas.SetZIndex(this._Line, zIndex);
                    Canvas.SetZIndex(this.TextLabel, zIndex);
                }

                protected void ChangeTextLabel(string text)
                {
                    this.TextLabel.Text = text;
                    //поворачиваем текст
                    if (this.Rotate90)
                        this.TextLabel.RenderTransform = new RotateTransform(-90);
                }

                public void HideLabel()
                {
                    this.TextLabel.Visibility = Visibility.Hidden;
                }
                public void ShowLabel()
                {
                    this.TextLabel.Visibility = Visibility.Visible;
                }

                public void Hide()
                {
                    this.HideLabel();
                    this._Line.Visibility = Visibility.Hidden;
                }
                public void Show()
                {
                    this.ShowLabel();
                    this._Line.Visibility = Visibility.Visible;
                }


                public void Remove(Canvas canvas)
                {
                    if (this._Line.Parent != null)
                        canvas.Children.Remove(this._Line);
                    if (this.TextLabel.Parent != null)
                        canvas.Children.Remove(this.TextLabel);
                }
            }

            ////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// Класс значений уровней
            /// </summary>
            public class ValueLevels
            {
                public DateTime Time;
                public decimal Price = 0;
                public decimal Value = 0;
            }
            /// <summary>
            /// Класс тела уровня
            /// </summary>
            public class LevelBody
            {
                public ValueLevels KitValues = null;    //Значения
                public Rectangle Body = null;       //Фигура уровня

                public void Paint(Canvas canvas, int zIndex = 0)
                {
                    if (this.Body.Parent == null)
                    {
                        canvas.Children.Add(this.Body);
                        //Указываем zIndex порядка отрисовки
                        Canvas.SetZIndex(this.Body, zIndex);
                        this.Body.UpdateLayout();
                    }
                }

                public void Clear(Canvas canvas)
                {
                    if (this.Body.Parent != null)
                    {
                        canvas.Children.Remove(this.Body);
                        canvas.InvalidateVisual();
                    }
                }

                public void Show()
                {
                    this.Body.Visibility = Visibility.Visible;
                }
                public void Hide()
                {
                    this.Body.Visibility = Visibility.Hidden;
                }
            }

            public class Levels : Shapes
            {
                public SolidColorBrush ColorPositive = new SolidColorBrush(Colors.Green);
                public SolidColorBrush ColorNegative = new SolidColorBrush(Colors.Red);

                protected List<LevelBody> ListViewObject = new List<LevelBody>();       //Список фигур для уровней
                protected RectanglePaint RectView = null;

                protected decimal MaxValue = 0;
                protected decimal MinValue = 10000000;

                public int CountLevel = 1;                        //Кол-во отображаемых уровней

                protected TextBlock Header = new TextBlock();     //Заголовок уровня в квадрате

                protected RectanglePaint OldRectView = null;

                protected decimal oldMaxValue = 0;
                protected decimal oldMinValue = 10000000;

                protected int countDecimalValue = 0;

                public void ChangeCountLevel(int countLevel)
                {
                    this.CountLevel = countLevel;
                }

                public void SetNewValue(int indexObjLevel, ValueLevels Value)
                {
                    Value.Value = Math.Round(Value.Value, this.countDecimalValue);
                    if (this.ListViewObject.Count > indexObjLevel)
                    {
                        this.ListViewObject[indexObjLevel].KitValues = Value;
                        var elObj = this.ListViewObject[indexObjLevel];
                        elObj.Body.Fill = elObj.KitValues.Value > 0 ? this.ColorPositive : this.ColorNegative;
                        elObj.Body.Stroke = elObj.KitValues.Value > 0 ? this.ColorPositive : this.ColorNegative;
                    }
                    else
                    {
                        LevelBody newLevel = new LevelBody();
                        newLevel.KitValues = Value;
                        newLevel.Body = Shapes.getRectangle(0, 0);
                        newLevel.Body.Fill = newLevel.KitValues.Value > 0 ? this.ColorPositive : this.ColorNegative;
                        newLevel.Body.Stroke = newLevel.KitValues.Value > 0 ? this.ColorPositive : this.ColorNegative;

                        this.ListViewObject.Insert(this.ListViewObject.Count, newLevel);
                    }
                }
                public void SetPositiveColor(SolidColorBrush Color)
                {
                    this.ColorPositive = Color;
                }

                public void SetNegativeColor(SolidColorBrush Color)
                {

                    this.ColorNegative = Color;
                }

                public void SetRectView(RectanglePaint rectView)
                {
                    this.OldRectView = this.RectView;
                    this.RectView = rectView;
                }
                //Установка кол-ва отображаемых уровней
                public void SetCountLevels(int count)
                {
                    this.CountLevel = count;
                }
                //Установить округление значения
                public void SetRountValue(int countDecimal)
                {
                    this.countDecimalValue = countDecimal;
                }

                //Получение кол-ва отображаемых уровней
                public int GetCountLevels()
                {
                    return this.CountLevel;
                }

                protected void PaintLevels(Canvas canvas, RectanglePaint RactPaint, int zIndex = 0)
                {
                    if (this.ListViewObject.Count > 0)
                    {
                        foreach (var obj in this.ListViewObject)
                        {
                            obj.Paint(canvas, zIndex);
                            if (obj.KitValues.Value > 0)
                            {
                                obj.Body.Stroke = this.ColorPositive;
                                obj.Body.Fill = this.ColorPositive;
                            }
                            else
                            {
                                obj.Body.Stroke = this.ColorNegative;
                                obj.Body.Fill = this.ColorNegative;
                            }
                        }
                    }
                }

                public void Remove(Canvas canvas, LevelBody element)
                {
                    if (canvas != null)
                    {
                        if (element != null)
                        {
                            canvas.Children.Remove(element.Body);
                            //if (this.ListViewObject.Where(obj => obj == element) != null)
                            this.ListViewObject.Remove(element);
                        }
                    }
                }
                //Текст заголовка
                public void SetHeader(string text, double textSize = 10)
                {
                    this.Header.Text = text;
                    this.Header.FontSize = textSize;
                }

                //Рисуем заголовок уровня
                protected void PaintHeader(Canvas canvas, int zIndex = 0)
                {
                    Canvas.SetLeft(this.Header, this.RectView.X + 2);
                    Canvas.SetTop(this.Header, this.RectView.Y + 1);
                    if (this.Header.Parent == null)
                    {
                        canvas.Children.Add(this.Header);
                        //Указываем zIndex порядка отрисовки
                        Canvas.SetZIndex(this.Header, zIndex);
                    }
                }
            }

            public class VerticalLevels : Levels
            {
                public List<HorizontLine> LineValues = new List<HorizontLine>();    //Линии значений

                private HorizontLine CurVal = new HorizontLine();   //Линия текущего объема

                public int CountLineValues = 4;

                private HorizontLine BorderLine = new HorizontLine();

                public VerticalLevels(RectanglePaint rectView)
                {
                    this.SetRectView(rectView);

                    this.BorderLine.ChangeStyle(Brushes.Blue);
                    this.BorderLine.HideLabel();

                    this.CurVal.ChangeStyle(Brushes.Red, 0.5, new List<double>() { 10, 5 }, 11);
                    this.CurVal.BackgroundLabel(Brushes.White);
                }

                public void Change(int indexObjLevel, double x, double Width = 1, double MarginLevel = 1)
                {
                    if (this.ListViewObject.Count > indexObjLevel)
                    {
                        Canvas.SetLeft(this.ListViewObject[indexObjLevel].Body, x);
                        this.ListViewObject[indexObjLevel].Body.Width = Width - MarginLevel > 0 ? Width - MarginLevel : 0;
                    }
                }
                public void PaintLevels(Canvas canvas, int zIndex = 0)
                {
                    if (this.ListViewObject.Count > 0)
                    {
                        while (this.ListViewObject.Count > this.CountLevel)
                        {
                            var lastobj = this.ListViewObject.Last();
                            this.Remove(canvas, lastobj);
                            this.ListViewObject.Remove(lastobj);
                        }
                        this.oldMaxValue = this.MaxValue;
                        this.oldMinValue = this.MinValue;
                        //получение min max
                        if (this.ListViewObject.Count > 0)
                        {
                            this.MaxValue = this.ListViewObject.Max(l => l.KitValues.Value);
                            this.MinValue = this.ListViewObject.Min(l => l.KitValues.Value);
                        }
                        this.MaxValue = this.MaxValue < 0 ? 0 : this.MaxValue;
                        this.MinValue = this.MinValue > 0 ? 0 : this.MinValue;

                        //Линии отметки (перерисовываем если изменились MAX и MIN значения)
                        if (this.oldMaxValue != this.MaxValue || this.oldMinValue != this.MinValue ||
                            this.OldRectView.Width != this.RectView.Width || this.OldRectView.Height != this.RectView.Height)
                        {
                            decimal Interval = this.MaxValue - this.MinValue;
                            Interval = Convert.ToInt32(Interval / this.CountLineValues);
                            for (int i = 0; i < this.CountLineValues; i++)
                            {
                                //Удаляем  лишние
                                if (this.LineValues.Count > this.CountLineValues)
                                {
                                    var lastElem = this.LineValues.Last();
                                    lastElem.Remove(canvas);
                                    this.LineValues.Remove(lastElem);
                                }
                                if (this.LineValues.Count <= i)
                                {
                                    HorizontLine line = new HorizontLine();
                                    line.ChangeStyle(Brushes.Gray, 0.5, new List<double>() { 10, 5 }, 10);
                                    this.LineValues.Add(line);
                                }
                                decimal val = this.MinValue + i * Interval;
                                //double y = this.GetCoordinate(this.RectView.Height, this.MaxValue, this.MinValue, val);
                                this.LineValues[i].ChangeHLine(val);

                                this.LineValues[i].PaintHLine(canvas, this.RectView, this.MaxValue, this.MinValue, zIndex);
                            }
                            //Отделительная линия
                            this.BorderLine.ChangeHLine(this.MaxValue);
                            this.BorderLine.PaintHLine(canvas, this.RectView, this.MaxValue, this.MinValue, zIndex);
                        }
                        //Отрисовка уровней
                        foreach (var obj in this.ListViewObject)
                        {
                            //////////////////

                            double y = this.GetCoordinate(this.RectView.Height, this.MaxValue, this.MinValue, obj.KitValues.Value);
                            double y1 = this.GetCoordinate(this.RectView.Height, this.MaxValue, this.MinValue, 0);
                            if (obj.KitValues.Value < 0)
                            {
                                Canvas.SetTop(obj.Body, this.RectView.Y + y1);
                                obj.Body.Height = y - y1 < 0 ? 0 : y - y1;
                            }
                            else
                            {
                                Canvas.SetTop(obj.Body, this.RectView.Y + y);
                                obj.Body.Height = y1 - y < 0 ? 0 : y1 - y;
                            }

                            //obj.Body.UpdateLayout();
                            obj.Paint(canvas, zIndex);
                        }
                        //Линия текущего уровня
                        this.CurVal.ChangeHLine(this.ListViewObject[0].KitValues.Value);
                        this.CurVal.PaintHLine(canvas, this.RectView, this.MaxValue, this.MinValue, zIndex + 1);

                        //Заголовок
                        this.PaintHeader(canvas, zIndex);
                    }
                }
            }

            public class HorizontalLevels : Levels
            {
                public double HeightLevel = 2;

                //private List<ValueLevels> ArrayValues = new List<ValueLevels>();
                //private decimal minStepPrice = 0;

                private List<VerticalLine> LineValues = new List<VerticalLine>();    //Линии значений
                public int CountLineValues = 4;

                private VerticalLine BorderLine = new VerticalLine();

                public HorizontalLevels(RectanglePaint rectView)
                {
                    this.SetRectView(rectView);

                    this.BorderLine.ChangeStyle(Brushes.Blue);
                    this.BorderLine.HideLabel();
                }
                public void Change(double height = 2)
                {
                    this.HeightLevel = height;
                }

                /*public void SetArrayValues(List<ValueLevels> arrayValues, decimal minStepPrice)
                {
                    this.ListViewObject = arrayValues;
                    this.minStepPrice = minStepPrice;
                }
                public void SetArrayValues(List<ValueLevels> arrayValues, decimal minStepPrice, decimal maxValue, decimal minValue)
                {
                    this.ListViewObject = arrayValues;
                    this.minStepPrice = minStepPrice;

                    this.MaxValue = 0;
                    this.MinValue = 1000000;
                    this.SetMax_Min(maxValue, minValue);
                }*/
        /*
                //Установка максим. и минмал. значений диапазона
                public void SetMax_Min(decimal Max, decimal Min)
                {
                    this.oldMaxValue = this.MaxValue;
                    this.oldMinValue = this.MinValue;

                    this.MaxValue = Max;
                    this.MinValue = Min;
                }
                //Удаляем лишние
                public void ClearExtra(Canvas canvas, int countExists)
                {
                    while (this.ListViewObject.Count > countExists)
                    {
                        var lastElem = this.ListViewObject[this.ListViewObject.Count - 1];
                        canvas.Children.Remove(lastElem.Body);
                        this.ListViewObject.Remove(lastElem);
                    }
                }
                public void PaintLevels(Canvas canvas, decimal maxPrice, decimal minPrice, bool getMaxAndMin = false, int zIndex = 0)
                {
                    if (this.ListViewObject.Count > 0)
                    {
                        int count = this.ListViewObject.Count;
                        /*while (this.ListViewObject.Count > this.CountLevel)
                        {
                            var lastElem = this.ListViewObject.Last();
                            canvas.Children.Remove(lastElem.Body);
                            this.ListViewObject.Remove(lastElem);
                        }*/
        //Получаем Min MAx значения если указан флаг
        /*                if (getMaxAndMin)
                        {
                            this.oldMaxValue = this.MaxValue;
                            this.oldMinValue = this.MinValue;
                            if (this.ListViewObject.Count > 0)
                            {
                                this.MaxValue = this.ListViewObject.Max(l => l.KitValues.Value);
                                this.MinValue = this.ListViewObject.Min(l => l.KitValues.Value);
                            }
                        }
                        decimal percentMargin = 0;// (this.MaxValue - this.MinValue) * 8 / 100; // +-10% для отображения шкалы
                        this.MaxValue += percentMargin;
                        //this.MinValue -= percentMargin;

                        this.MinValue = this.MinValue > 0 ? 0 : this.MinValue;
                        this.MaxValue = this.MaxValue < 0 ? 0 : this.MaxValue;

                        //Линии отметки (перерисовываем если изменились MAX и MIN значения)
                        if (oldMaxValue != this.MaxValue || oldMinValue != this.MinValue ||
                            this.OldRectView.Width != this.RectView.Width || this.OldRectView.Height != this.RectView.Height)
                        {
                            decimal IntervalPrice = maxPrice - minPrice;
                            decimal intervalLine = Convert.ToInt32((this.MaxValue - this.MinValue) / this.CountLineValues);
                            intervalLine = intervalLine < 0 ? intervalLine * -1 : intervalLine;
                            for (int t = 0; t < this.CountLineValues; t++)
                            {
                                //Удаляем лишние линии разметки
                                if (this.LineValues.Count > this.CountLineValues)
                                {
                                    var lastElem = this.LineValues.Last();
                                    lastElem.Remove(canvas);
                                    this.LineValues.Remove(lastElem);
                                }
                                if (this.LineValues.Count <= t)
                                {
                                    VerticalLine line = new VerticalLine();
                                    line.Rotation90();
                                    line.ChangeStyle(Brushes.Gray, 0.5, new List<double>() { 10, 5 }, 9);
                                    this.LineValues.Add(line);
                                }
                                decimal value = Math.Round(this.MinValue + (t * intervalLine), 0);
                                double x = this.GetCoordinate(this.RectView.Width, this.MaxValue, this.MinValue, value);
                                this.LineValues[t].ChangeVLine(this.RectView.X + x, value.ToString());
                                this.LineValues[t].PaintVLine(canvas, this.RectView, zIndex);
                            }

                            //Отделительная линия
                            this.BorderLine.PaintVLine(canvas, this.RectView, zIndex);
                            this.BorderLine.ChangeVLine(this.RectView.X);

                            //Заголовок
                            this.PaintHeader(canvas, zIndex);
                        }
                        //Отрисовка уровней
                        int i = 0;
                        foreach (var obj in this.ListViewObject)
                        {
                            if (obj.KitValues.Value == 0) obj.Hide();
                            else obj.Show();
                            //Расчет координат для отображения по уровню цены
                            double y = this.GetCoordinate(this.RectView.Height, maxPrice, minPrice, obj.KitValues.Price);
                            Canvas.SetTop(this.ListViewObject[i].Body, this.RectView.Y + y - (this.HeightLevel / 2));
                            this.ListViewObject[i].Body.Height = this.HeightLevel;

                            //Расчет ширины и высоты объекта отрисовки уровня
                            double x = 0;
                            double x1 = 0;
                            if (obj.KitValues.Value > 0)
                            {
                                x = this.GetCoordinate(this.RectView.Width, this.MaxValue, this.MinValue, obj.KitValues.Value);
                                x1 = this.GetCoordinate(this.RectView.Width, this.MaxValue, this.MinValue, 0);
                            }
                            else
                            {
                                x = this.GetCoordinate(this.RectView.Width, this.MaxValue, this.MinValue, 0);
                                x1 = this.GetCoordinate(this.RectView.Width, this.MaxValue, this.MinValue, obj.KitValues.Value);
                            }
                            Canvas.SetLeft(this.ListViewObject[i].Body, this.RectView.X + x);
                            this.ListViewObject[i].Body.Width = x1 - x < 0 ? 0 : x1 - x;

                            this.ListViewObject[i].Paint(canvas, zIndex);
                            i++;
                        }
                    }
                }*/
    }
}

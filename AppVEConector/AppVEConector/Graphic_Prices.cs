using System;
using System.Drawing;

namespace NSGraphic
{
    /// <summary>
    /// Объект отрисовки фрейма с ценами на графике
    /// </summary>
    public partial class Prices
    {
        public Rectangle RectPaint = new Rectangle();
        /// <summary> Цвет линий </summary>
        public Color ColorLines = Color.LightGray;
        /// <summary> Ширина области для отображения цен </summary>
        public int WidthBorder = 10;
        /// <summary> Текущее значение для контрольной линии </summary>
        public decimal CurrentValue = -1;
        /// <summary> Цвет текущего значения для контрольной линии </summary>
        public Color ColorCurrentLine = Color.Red;

        /// <summary> Кол-во линий отметок цены </summary>
        public int CountBorderPrice = 10;

        /// <summary> Точность цены </summary>
        public int CountFloat = 2;
        /// <summary> Минимальный шаг цены в инструменте </summary>
        public decimal minStepPrice;
    }



    public partial class Prices
    {
        /// <summary>  Конструктор </summary>
        /// <param name="countFloat"> Точность цены (знаков после запятой)</param>
        public Prices(int countFloat)
        {
            this.CountFloat = countFloat;
        }
        /// <summary>
        /// Отрисовка
        /// </summary>
        /// <param name="canvas">Полотно</param>
        /// <param name="MaxPrice">MAX цена</param>
        /// <param name="MinPrice">MIN цена</param>
        public void PaintPrices(Graphics canvas, decimal MaxPrice, decimal MinPrice)
        {
            decimal Interval = MaxPrice - MinPrice;
            decimal stepPrice = 0;
            if (this.minStepPrice >= 1)
                stepPrice = Convert.ToInt32(Interval / this.CountBorderPrice);
            else stepPrice = decimal.Round(Interval / this.CountBorderPrice, this.CountFloat);

            //Рисуем линию границы
            Point pBorder1 = new Point(this.RectPaint.X + this.RectPaint.Width - WidthBorder, this.RectPaint.Y);
            Point pBorder2 = new Point(this.RectPaint.X + this.RectPaint.Width - WidthBorder, this.RectPaint.Y + this.RectPaint.Height);
            GraphicShape.PaintLine(canvas, pBorder1, pBorder2, Color.Black);

            //Цены сетки
            for (int i = 0; i < this.CountBorderPrice; i++)
            {
                decimal Price = decimal.Round(MinPrice + stepPrice * i, this.CountFloat);
                GraphicShape.PaintHLine(canvas, this.RectPaint, Price, MaxPrice, MinPrice, this.ColorLines);
            }
        }

        /// <summary>  Рисует контрольную линию текущего значения </summary>
        /// <param name="canvas">Полотно</param>
        /// <param name="MaxPrice">MAX цена</param>
        /// <param name="MinPrice">MIN цена</param>
        /// <param name="minStepPrice"></param>
        public void PaintCurrentValue(Graphics canvas, decimal MaxPrice, decimal MinPrice)
        {
            //Рисуем последнюю цену
            GraphicShape.PaintHLine(canvas, this.RectPaint, this.CurrentValue, MaxPrice, MinPrice, this.ColorCurrentLine);
        }
        //Получает граничную округленную цену
        /*public static decimal GetBorderPrice(decimal price, decimal StepTicks)
        {
            string strPrice = price.ToString();
            int posFloatPoint = strPrice.IndexOf(',');
            strPrice = strPrice.Replace(",", "");
            long Price = Convert.ToInt32(strPrice);

            long k = (long)(Price / StepTicks);
            k = (int)(StepTicks * k);
            k = Price - (Price - k);

            long r = k;
            string resPrice = r.ToString();
            if (strPrice.Length > resPrice.Length) posFloatPoint--;
            if (posFloatPoint >= 0) resPrice = resPrice.Insert(posFloatPoint, ",");

            decimal resultPrice = Convert.ToDecimal(resPrice);
            return resultPrice;
        }*/
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace NSGraphic
{
    /// <summary>
    /// Клас отрисовки шкалы цен.
    /// </summary>
    public partial class TimeFrame
    {
        /// <summary> Ширина одной свечки </summary>
        public int WidthOneCandle = 0;
        /// <summary> Коллекция свечек для отображения </summary>
        public IEnumerable<CandleLib.CandleData> CollectionCandle = null;

    }

    public partial class TimeFrame
    {
        public TimeFrame() { }

        /// <summary>
        /// Отрисовка временных линий и значений.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="rectPaint"></param>
        public void PaintTimeFrame(Graphics canvas, Rectangle rectPaint)
        {
            int count = this.CollectionCandle.Count();
            if (count == 0) return;
            if (count < 5) count = 5;

            int LastX = 0;
            int index = 1;
            foreach (var candleData in CollectionCandle)
            {
                //int x = rectPaint.X + (rectPaint.Width - WidthOneCandle * index) + (WidthOneCandle / 2);
                int x = (int)((rectPaint.Width - this.WidthOneCandle * index) + this.WidthOneCandle / 2);
                if (index == 1) LastX = x;
                if (LastX - x > 40 || index == 1)
                {
                    var p1 = new Point(x, rectPaint.Y);
                    var p2 = new Point(x, rectPaint.Y + rectPaint.Height);
                    //GraphicShape.PaintLine(canvas, p1, p2, Color.Blue);
                    string min = candleData.Time.Minute.ToString();
                    string hour = candleData.Time.Hour.ToString();
                    string time = (hour.Length < 2 ? '0' + hour : hour) + ":" + (min.Length < 2 ? '0' + min : min);
                    if (LastX - x > 50) time = candleData.Time.Day.ToString() + "." + candleData.Time.Month.ToString() + " " + time;
                    GraphicShape.PaintVLine(canvas, rectPaint, time, p1, p2, Color.Gray);
                    LastX = x;
                }
                if (x < 0) break;
                index++;
            }
        }
    }


}

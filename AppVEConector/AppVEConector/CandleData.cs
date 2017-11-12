using MarketObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/// <summary>
/// Данные по свечкам.
/// </summary>
namespace CandleLib
{
    [Serializable]
    public partial class CandleData
    {
        private Mutex MutexCandle = new Mutex();

        public DateTime Time;
        public long Volume = 0;
        public long VolumeBuy = 0;
        public long VolumeSell = 0;

        public decimal High = 0;
        public decimal Low = 1000000;
        public decimal Open = -1;
        public decimal Close = 0;

        /// <summary> id первой сделки </summary>       
        public long FirstId = 0;
        /// <summary> Id последней сделки </summary>
        public long LastId = 0;

        /// <summary> Время первой сделки </summary>
        public DateTime FirstTime = new DateTime();
        /// <summary> Время последней сделки </summary>
        public DateTime LastTime = new DateTime();

        /// <summary> Горизонтальные объемы для свечи </summary>
        public VolumeLib.TradeVolume HorVolumes = new VolumeLib.TradeVolume();

        public DateTime _lastUpdate;    //Время последнего обновления
        public bool _write = false;     //флаг, была ли свеча записана в файл.
    }







    public partial class CandleData
    {
        /// <summary> Конструктор свечи</summary>
        /// <param name="time">Граничное время свечи</param>
        public CandleData(DateTime time)
        {
            this.Time = time;
        }
        /// <summary> Запись новой сделки в свечку. </summary>
        /// <param name="trade">Новая сделка</param>
        public void NewTrade(Trade trade)
        {
            MutexCandle.WaitOne();
            //Open
            if (this.FirstTime.Ticks > trade.DateTrade.Ticks ||
                this.FirstTime.Ticks == 0 ||
                this.FirstTime.Ticks == trade.DateTrade.Ticks)
            {
                if (this.FirstTime.Ticks == trade.DateTrade.Ticks)
                {
                    if (this.FirstId > trade.Number)
                    {
                        this.FirstTime = trade.DateTrade;
                        this.Open = trade.Price;
                        this.FirstId = trade.Number;
                    }
                }
                else
                {
                    this.FirstTime = trade.DateTrade;
                    this.Open = trade.Price;
                    this.FirstId = trade.Number;
                }
            }
            //Close
            if (this.LastTime.Ticks < trade.DateTrade.Ticks ||
                    this.LastTime.Ticks == 0 ||
                    this.LastTime.Ticks == trade.DateTrade.Ticks)
            {
                if (this.LastTime.Ticks == trade.DateTrade.Ticks)
                {
                    if (this.LastId < trade.Number)
                    {
                        this.LastTime = trade.DateTrade;
                        this.Close = trade.Price;
                        this.LastId = trade.Number;
                    }
                }
                else
                {
                    this.LastTime = trade.DateTrade;
                    this.Close = trade.Price;
                    this.LastId = trade.Number;
                }
            }

            if (this.High < trade.Price) this.High = trade.Price;
            if (this.Low > trade.Price) this.Low = trade.Price;

            this.Volume += trade.Volume;
            //Считаем объемы отдельно
            if (trade.Direction == OrderDirection.Sell) this.VolumeSell += trade.Volume;
            else this.VolumeBuy += trade.Volume;

            HorVolumes.AddTrade(trade);

            _lastUpdate = DateTime.Now;
            MutexCandle.ReleaseMutex();
        }

        /// <summary>  Расчет времени для свечи (граничной), по текущему времени.  </summary>
        /// <param name="time">Время сделки</param>
        /// <returns></returns>
        public static DateTime GetTimeCandle(DateTime time, int TimeFrame)
        {
            time = time.AddMilliseconds(time.Millisecond * -1);
            time = time.AddSeconds(time.Second * -1);
            int k = (int)(TimeFrame / 60);
            if (k > 0)
            {
                double r = (double)(time.Hour % k);
                if (r >= 1)
                {
                    k = 60 * (time.Hour - (int)(time.Hour / k) * k);
                }
                else
                {
                    k = 0;
                }
            }
            else
            {
                k = (int)(time.Minute / TimeFrame);
                if (time.Minute == k * TimeFrame) k = time.Minute;
                else k = (k * TimeFrame);
                k *= -1;
            }
            k = k + time.Minute;

            time = time.AddMinutes(k * -1);
            return time;
        }
    }
}
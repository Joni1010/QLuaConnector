using MarketObject;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>  Данные по свечкам.  </summary>
namespace CandleLib
{
    [Serializable]
    public partial class CandleData
    {
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

        /// <summary> Время последнего обновления </summary>
        public DateTime _lastUpdate;
        /// <summary> флаг, была ли свеча записана в файл. </summary>
        public bool _write = false;

        /// <summary> </summary>
        private List<long> CollectionNumTrades = null;
    }

    public partial class CandleData
    {
        /// <summary> Проверка уже записанной сделки в данную свечу </summary>
        /// <param name="trade"></param>
        /// <returns>true - если сделка уже записана </returns>
        public bool CheckExistsTrade(Trade trade)
        {
            if (this.CollectionNumTrades.IsNull()) return false;
            var num = this.CollectionNumTrades.FirstOrDefault(n => n == trade.Number);
            if (!num.IsNull() && num > 0) return true;
            return false;
        }
        /// <summary> Конструктор свечи</summary>
        /// <param name="time">Граничное время свечи</param>
        public CandleData(DateTime time)
        {
            this.Time = time;
        }
        /// <summary> Запись новой сделки в свечку. </summary>
        /// <param name="trade">Новая сделка</param>
        public void NewTrade(Trade trade, bool controlTrades = false)
        {
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
            if (controlTrades)
            {
                if (CollectionNumTrades.IsNull()) CollectionNumTrades = new List<long>();
                CollectionNumTrades.Add(trade.Number);
            }

            _lastUpdate = DateTime.Now;
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
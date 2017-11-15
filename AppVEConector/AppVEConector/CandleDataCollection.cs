using MarketObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CandleLib
{
    /// <summary>  Класс коллекции данных по свечекам, за определнный тайм-фрейм. </summary>
    [Serializable]
    public class CandleCollection
    {
        private Mutex MutexCollection = new Mutex();
        /// <summary> Текущий тайм-фрейм </summary>
        public int TimeFrame = 1;

        public delegate void EventCandle(int timeframe, CandleData candle);
        /// <summary> Событие появления новой свечки </summary>
        public event EventCandle OnNewCandle;


        public delegate void DeleteExtra(CandleData candle);
        /// <summary> Событие удаления избыточной свечки</summary>
        public event DeleteExtra OnDeleteExtra;
        /// <summary>
        /// Набор значений готовых свечей для отрисовки
        /// </summary>
        private List<CandleData> Collection = new List<CandleData>();       //
        /// <summary>
        /// Кол-во хранимых свечек в каждом тайм-фрейме
        /// </summary>
        public int CountKeepCandle = 500;                                   //

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="timeFrame">Кол-во минут</param>
        public CandleCollection(int timeFrame)
        {
            this.TimeFrame = timeFrame;
        }

        /// <summary>
        /// Получить коллекцию данных по свечкам.
        /// </summary>
        public IEnumerable<CandleData> MainCollection
        {
            get
            {
                MutexCollection.WaitOne();
                var list = this.Collection;
                MutexCollection.ReleaseMutex();
                return list;
            }
        }
        /// <summary> Перебор коллекции </summary>
        /// <param name="eachAction"></param>
        public void ForEach(Action<CandleData> eachAction)
        {
            if (this.Count == 0) return;
            if (eachAction.IsNull()) return;
            foreach (var el in this.MainCollection) { eachAction(el); }
        }
        /// <summary> Блокирует коллекцию</summary>
        public void LockCollection()
        {
            MutexCollection.WaitOne();
        }
        /// <summary> Снимает блокировку с коллекции </summary>
        public void UnlockCollection()
        {
            MutexCollection.ReleaseMutex();
        }

        /// <summary> Возвращает кол-во свечек в текущем тайм-фрейме  </summary>
        public int Count
        {
            get
            {
                MutexCollection.WaitOne();
                int count = this.Collection.Count;
                MutexCollection.ReleaseMutex();
                return count;
            }
        }

        /// <summary>  Получить первую свечку по порядку. [0] элемент, если пустая то возвращается null. </summary>
        public CandleData FirstCandle
        {
            get
            {
                if (this.Count > 0) return this.GetElement(0);
                return null;
            }
        }

        /// <summary>
        /// Получить последнюю свечку в колеккции. MAX-индекс в коллекциит, если пустая то возвращается null.
        /// </summary>
        public CandleData LastCandle
        {
            get
            {
                return this.GetElement(this.Count - 1);
            }
        }

        /// <summary> Возвращает i-ый элемент в коллекции.  </summary>
        /// <param name="i">Индекс в коллекции</param>
        /// <returns></returns>
        public CandleData GetElement(int i)
        {
            MutexCollection.WaitOne();
            var el = this.Collection.Count > 0 ? this.Collection.ElementAt(i) : null;
            MutexCollection.ReleaseMutex();
            return el;
        }
        /// <summary> Получает i-элемент с конца</summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CandleData GetElementFromEnd(int i)
        {
            MutexCollection.WaitOne();
            i = this.Collection.Count - i - 1;
            if (i < 0)
            {
                MutexCollection.ReleaseMutex();
                return null;
            }
            var el = this.Collection.Count > 0 ? this.Collection.ElementAt(i) : null;
            MutexCollection.ReleaseMutex();
            return el;
        }

        //**************************************************************

        /// <summary> Добавление "первой" свечки в коллекцию, в [0] по индексу.  </summary>
        /// <param name="candle"></param>
        public void InsertFirst(CandleData candle)
        {
            if (candle.IsNull()) return;
            MutexCollection.WaitOne();
            this.Collection.Insert(0, candle);
            MutexCollection.ReleaseMutex();
        }
        /// <summary> Создает новую свечку и добавляем вначало [0] коллекции. </summary>
        /// <param name="time">Расчитанное граничное время свечки.</param>
        private void AddNewCandle(DateTime time)
        {
            this.InsertFirst(new CandleData(time));
        }


        /// <summary> Добавить новую сделку в свечку с соответствущим временем. </summary>
        /// <param name="trade">Сделка</param>
        /// <param name="history"> Флаг загрузки исторических сделок </param>
        public void AddNewTrade(Trade trade, bool history = false)
        {
            if (trade.IsNull()) return;
            DateTime time = CandleData.GetTimeCandle(trade.DateTrade, this.TimeFrame);
            if (this.Count > 0)
            {
                MutexCollection.WaitOne();
                var LastFindCandle = this.Collection.FirstOrDefault(c => c.Time == time);
                if (!LastFindCandle.IsNull())
                {
                    if (!history && !LastFindCandle._write)
                        LastFindCandle.NewTrade(trade);
                    else if (history)
                    {
                        if (!LastFindCandle._write) LastFindCandle = new CandleData(time);
                        LastFindCandle.NewTrade(trade);
                        LastFindCandle._write = true; //Определяем что уже записана
                    }
                }
                else
                {
                    this.AddNewCandle(time);
                    LastFindCandle = this.FirstCandle;
                    if (history)
                    {
                        LastFindCandle.NewTrade(trade);
                        LastFindCandle._write = true; //Определяем что уже записана
                    }
                    else
                    {
                        LastFindCandle.NewTrade(trade);
                    }
                    //Сортируем по времени
                    this.Collection = this.Collection.OrderByDescending(c => c.Time).ToList();
                    if (!OnNewCandle.IsNull())
                    {
                        if (!history)
                        {
                            OnNewCandle(this.TimeFrame, LastFindCandle);
                        }
                    }
                    //Удаляем свечки c конца, которые выше допустимого кол-ва хранения
                    if (this.Collection.Count > this.CountKeepCandle)
                    {
                        if (this.OnDeleteExtra != null)
                            OnDeleteExtra(this.LastCandle);
                        this.Collection.Remove(this.LastCandle);
                    }
                }
                MutexCollection.ReleaseMutex();
            }
            else
            {
                MutexCollection.WaitOne();
                //Добавляем первую свечку в коллекцию
                this.AddNewCandle(time);
                this.FirstCandle.NewTrade(trade);
                if (history)
                    this.FirstCandle._write = true; //Определяем что уже записана
                MutexCollection.ReleaseMutex();
            }
        }
    }
}

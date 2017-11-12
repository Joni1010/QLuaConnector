using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VolumeLib
{
    /// <summary> Класс горизонтальных объемов </summary>
    [Serializable]
    public class HVolume
    {
        private List<MarketObject.ChartVol> Collection = new List<MarketObject.ChartVol>();
        private Mutex LockingMutex = new Mutex();
        /// <summary> Кол-во элементов (цен) в коллекции  </summary>
        public int Count {
            get
            {
                LockingMutex.WaitOne();
                var res = this.Collection.Count;
                LockingMutex.ReleaseMutex();
                return res;
            }
        }

        /// <summary> Коллекция </summary>
        public MarketObject.ChartVol[] CollectionArray
        {
            get
            {
                LockingMutex.WaitOne();
                var list = this.Collection.ToArray();
                LockingMutex.ReleaseMutex();
                return list;
            }
        }
        public HVolume()
        {
            this.Clear();
        }
        /// <summary> Очистка </summary>
        public void Clear()
        {
            LockingMutex.WaitOne();
            this.Collection.Clear();
            LockingMutex.ReleaseMutex();
        }

        /// <summary> Добавляем цену и объем в коллекцию </summary>
        /// <param name="price"></param>
        /// <param name="volume"></param>
        public void AddVolume(decimal price, long volume, bool isBuy)
        {
            LockingMutex.WaitOne();
            var elem = this.Collection.FirstOrDefault(e => e.Price == price);
            if (elem != null)
            {
                if(isBuy) elem.VolBuy += volume;
                else elem.VolSell += volume;
            }
            else
            {
                this.Collection.Add(
                    isBuy ? 
                        new MarketObject.ChartVol() { Price = price, VolBuy = volume } :
                        new MarketObject.ChartVol() { Price = price, VolSell = volume }
                    );
            }
            LockingMutex.ReleaseMutex();
        }
        /// <summary>  Получить список элементов между двумя ценами </summary>
        /// <param name="price1"></param>
        /// <param name="price2"></param>
        /// <returns></returns>
        public IEnumerable<MarketObject.ChartVol> GetElementBetween(decimal price1, decimal price2)
        {
            decimal tmp = 0;
            if (price1 > price2)
            {
                tmp = price1;
                price1 = price2;
                price2 = tmp;
            }
            return this.CollectionArray.Where(e => e.Price >= price1 && e.Price <= price2);
        }

        /// <summary> Возвращает объем по текущей цене </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public MarketObject.ChartVol GetVolume(decimal price)
        {
            var elem = this.CollectionArray.FirstOrDefault(e => e.Price == price);
            if (elem != null) return elem;
            return null;
        }

        /// <summary> Возвращает сумму всех объемов </summary>
        /// <returns></returns>
        public decimal GetSumAllVolume(bool isBuy)
        {
            if(isBuy) return this.CollectionArray.Sum(e => e.VolBuy);
            else return this.CollectionArray.Sum(e => e.VolSell);
        }
        /// <summary> Возвращает сумму объемов между 2мя ценами </summary>
        /// <returns></returns>
        public decimal GetSumVolumeBetween(decimal price1, decimal price2, bool isBuy)
        {
            decimal tmp = 0; 
            if(price1 > price2)
            {
                tmp = price1;
                price1 = price2;
                price2 = tmp;
            } 
            var list = this.GetElementBetween(price1, price2);
            if(isBuy)  return list != null ? list.Sum(e => e.VolBuy) : 0;
            else return list != null ? list.Sum(e => e.VolSell) : 0;
        }
    }
}


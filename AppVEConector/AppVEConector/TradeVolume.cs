using MarketObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeLib
{
    [Serializable]
    public class TradeVolume : Volume
    {
        public TradeVolume() { }

        /// <summary>  Добавление данных  </summary>
        /// <param name="trade"></param>
        public void AddTrade(Trade trade)
        {
            this.addVolumeToArray(trade);
        }

        //Запись данных в массив
        private void addVolumeToArray(Trade trade)
        {
            if (trade.Direction == OrderDirection.Sell)
            {
                this.AddSell(trade.Price, (int)trade.Volume);
                this.AddBuy(trade.Price, 0);
            }
            if (trade.Direction == OrderDirection.Buy)
            {
                this.AddBuy(trade.Price, (int)trade.Volume);
                this.AddSell(trade.Price, 0);
            }
        }
    }
}

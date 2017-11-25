using MarketObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AppVEConector
{
    public class TradeController
    {
        public class DateCollection
        {
            public DateTime Date;
            public List<Trade> Trades = new List<Trade>();
        }
        Mutex mutex = new Mutex();
        List<DateCollection> ListDate = new List<DateCollection>();

        public void Add(Trade trade)
        {
            mutex.WaitOne();
            var date = trade.DateTrade.Date;
            var el = this.ListDate.FirstOrDefault(d => d.Date == date);
            if (el.IsNull())
            {
                el = new DateCollection() { Date = date };
                LoadTradesFromFile(el);
                this.ListDate.Add(el);

                var tradeExists = el.Trades.FirstOrDefault(t => t.Number == trade.Number);
                if (tradeExists.IsNull())
                {
                    AppendTradeFile(trade);
                    el.Trades.Add(trade);
                }
            }
            else
            {
                var tradeExists = el.Trades.FirstOrDefault(t => t.Number == trade.Number);
                if (tradeExists.IsNull())
                {
                    AppendTradeFile(trade);
                    el.Trades.Add(trade);
                }
            }
            mutex.ReleaseMutex();
        }

        private string GetFilePath(DateTime date)
        {
            string dir = "./charts/";
            if (!Directory.Exists(dir)) return "";
            return dir + date.Date.ToString().Replace(':', '.') + ".tr";
        }

        private void AppendTradeFile(Trade trade)
        {
            FileLib.WFile f = new FileLib.WFile(GetFilePath(trade.DateTrade));
            //if (!f.Exists()) f.Append("");
            string text = trade.Number.ToString() + '\t' +
                trade.DateTrade.ToString() + '\t' +
                trade.Price.ToString() + '\t' +
                trade.Volume.ToString() + '\t' +
                (trade.Direction == OrderDirection.Buy ? 'B' : 'S') + '\t' +
                trade.Sec.Code + ':' + trade.Sec.Class.Code;
            f.Append(text);
        }

        private void LoadTradesFromFile(DateCollection newCol)
        {
            FileLib.WFile f = new FileLib.WFile(GetFilePath(newCol.Date));
            if (!f.Exists()) return;
            var strings = f.ReadAllLines();
            strings.ForEach<string>((s) => {
                if (s.Empty()) return;
                var dataTrade = s.Split('\t');
                var trade = new Trade();
                trade.Number = dataTrade[0].ToLong();
                trade.Price = dataTrade[2].ToDecimal();
                trade.Volume = dataTrade[3].ToInt32();
                trade.Direction = dataTrade[4] == "B" ? OrderDirection.Buy : OrderDirection.Sell;
                trade.SecCode = dataTrade[5];

                newCol.Trades.Add(trade);
            });
        }
    }
}

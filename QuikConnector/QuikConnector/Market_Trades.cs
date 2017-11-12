using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MarketObject
{
    [Serializable]
    public class MyTrade
    {
        /// <summary> Номер заявки по которой прошла сделка </summary>
        public long OrderNum;
        /// <summary> Заявка по которой совершена сделка </summary>
        public Order Order;
        /// <summary> Сделка (параметры) </summary>
        public Trade Trade;

        public long uid;
        /// <summary> Комиссия брокера </summary>
        public decimal BrokerComission;
        /// <summary> Клиринговая комиссия </summary>
        public decimal ClearingComission;
        /// <summary> Комиссия Фондовой биржи </summary>
        public decimal ExchangeComission;
        /// <summary> Блокировка обеспечения </summary>
        public decimal BlockSecurities;
        /// <summary> Коментарий </summary>
        public string Comment;

        public MyTrade() { }
        public MyTrade(Trade trade)
        {
            this.Trade = trade;
        }
    }
    [Serializable]
    public class Trade
    {
        /// <summary> Номер сделки </summary>
        public long Number;
        /// <summary> Инструмент </summary>
        public Securities Sec;
        /// <summary> Код инструмента </summary>
        public string SecCode;
        /// <summary> Цена сделки </summary>
        public decimal Price;
        /// <summary> Объем сделки </summary>
        public int Volume;
        /// <summary> Направление сделки </summary>
        public OrderDirection? Direction { set; get; }
        /// <summary> Время сделки </summary>
        public DateTime DateTrade;
        /// <summary> Открытый интерес </summary>
        public decimal OpenInterest;
    };
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketObject
{
    /// <summary> Направление заявок и сделок </summary>
    public enum OrderDirection
    {
        Buy = 1,
        Sell = 2
    }
    /// <summary> Условие стоп-заявок </summary>
    public enum ConditionStopOrder
    {
        MoreOrEqual = 5,    // >=
        LessOrEqual = 4    // <=
    }
    /// <summary> Тип стоп-заявки </summary>
    public enum StopOrderType
    {
        StopLimit = 1,
        ConditionOtherSec = 2,
        LinkOrder = 3,
        TakeProfit = 6,
        StopLimitActiveOrder = 7,
        TakeProfitActiveOrder = 8,
        TakeProfitStopLimit = 9
    }
    public enum OrderStatus  {
        ACTIVE = 1,
        EXECUTED = 2,
        //PART_EXEC = 3,
        CLOSED = 10
    };
    [Serializable]
    public class Order
    {
        /// <summary> Номер заявки </summary>
        public long OrderNumber;
        /// <summary> Код инструмента </summary>
        public string SecCode;
        /// <summary> Инструмент </summary>
        public Securities Sec; 
        /// <summary> Цена сделки </summary>
        public decimal Price;
        /// <summary> Объем сделки </summary>
        public int Volume;
        /// <summary> Статус заявки </summary>
        public OrderStatus? Status { get; set; }
        /// <summary> Направление сделки </summary>
        public OrderDirection? Direction { set; get; }
        /// <summary> Остаток(NUMBER) </summary>
        public int Balance;
        /// <summary> Объем в денежных средствах(NUMBER) </summary>
        public decimal Value;          
        /// <summary> Время выставления заявки </summary>
        public DateTime DateCreateOrder;
        /// <summary> Время активации  (NUMBER) </summary>
        public decimal ActivationTime;
        /// <summary> Причина отклонения заявки брокером(STRING)  </summary>
        public decimal RejectReason;
        /// <summary> Идентификатор транзакции  (NUMBER)  </summary>
        public long TransID = 0;

        public decimal uid;
        /// <summary> Комментарий (STRING)  </summary>
        public string Comment;
        /// <summary> Сделки от данной заявки </summary>
        public IEnumerable<MyTrade> OrderTrades = null;
    }


    /// <summary> Класс стоп заявки  </summary>
    [Serializable]
    public class StopOrder : Order
    {
        /// <summary> Номер связанной заявки </summary>
        public long LinkOrderNum = 0;
        /// <summary> Цена связанной заявки </summary>
        public decimal LinkOrderPrice = 0;
        /// <summary>  Номер заявки зарегистрированной по наступлению условия стоп-цены </summary>
        public long OrderNumExecute = 0;
        /// <summary> Стоп-цена </summary>
        public decimal ConditionPrice = 0;
        /// <summary> Стоп-лимит цена (для заявок типа "Тэйк-профит и стоп-лимит")  (NUMBER) </summary>
        public decimal ConditionPrice2 = 0;
        /// <summary> Отступ от мин/макс </summary>
        public decimal Offset = 0;
        /// <summary> Защитный спред </summary>
        public decimal Spread = 0;
        /// <summary> Условие стоп-завки </summary>
        public ConditionStopOrder Condition;
        /// <summary> Время снятия заявки(NUMBER) </summary>
        public DateTime WithDrawTime;
        /// <summary> Дата экспирации </summary>
        public DateTime DateExpiry = DateTime.MaxValue;
        /// <summary> Тип стоп-заявки </summary>
        public StopOrderType? TypeStopOrder { get; set; }
        /// <summary> Код бумаги стоп-цены  (STRING) </summary>
        public string ConditionSecCode = null;
        /// <summary> Код класса стоп-цены  (STRING) </summary>
        public string ConditionClassCode = null;
    }

}

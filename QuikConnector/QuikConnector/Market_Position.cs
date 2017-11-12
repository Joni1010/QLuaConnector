using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarketObject
{
    public class Position
    {
        ///<summary>Код инструмента </summary>
        public string SecCode;
        ///<summary>Инструмент </summary>
        public Securities Sec;
        ///<summary>Данны по клиенту </summary>
        public Client Client;
        ///<summary>Фирма </summary>
        public Firm Firm;
        ///<summary>Счет </summary>
        public Account Account;
        ///<summary> Данные о позиции </summary>
        public DataPos Data = new DataPos();
        public class DataPos
        {

            ///<summary>Заблокированного на покупку количества лотов. Фьючерсы, активные на покупку(NUMBER) </summary>
            public int OrdersBuy = 0;
            ///<summary>Заблокировано на продажу количества лотов. Фьючерсы, активные на продажу(NUMBER) </summary>
            public int OrdersSell = 0;

            /// <summary> Текущие длинные позиции за сессию(NUMBER) </summary>
            public int TodayBuy = 0;
            ///<summary> Текущие короткие позиции за сессию(NUMBER) </summary>
            public int TodaySell = 0;
            ///<summary>Текущие чистые позиции(NUMBER)</summary>
            public int CurrentNet = 0;

            ///<summary>Средняя цена приобретения. Фьючерсы, эффективная цена позиций. (NUMBER)</summary>
            public decimal AwgPositionPrice = 0;

            ///<summary>Входящий остаток по бумагам. Фьючерсы, входящие длинные позиции. (NUMBER)</summary>
            public int StartBuy = 0;
            ///<summary>Фьючерсы. Входящие короткие позиции (NUMBER)</summary>
            public int StartSell = 0;
            ///<summary>Входящие чистые позиции(NUMBER)</summary>
            public int StartNet = 0;

            ///<summary>Текущий лимит по бумагам. Фьючерсы, стоимость позиций. (NUMBER)</summary>
            public decimal PositionValue = 0;

            /// 
            /// /////////////////////////////////////////////////////
            /// 

            //Заблокированного на покупку количества лотов  (NUMBER)  
            //public decimal LockedBuy = 0;
            //Заблокировано на продажу количества лотов  (NUMBER) 
            //public decimal LockedSell = 0;
            //Текущий остаток по бумагам  (NUMBER) 
            //public decimal CurrentBal = 0;
            ///<summary>Текущий лимит по бумагам  (NUMBER)  </summary>
            //public decimal CurrentLimit = 0;
            ///<summary>Входящий остаток по бумагам  (NUMBER)  </summary>
            //public decimal OpenBal;
            ///<summary>Входящий лимит по бумагам  (NUMBER)  </summary>
            public decimal OpenLimit;

            ///<summary>Стоимость ценных бумаг, заблокированных под покупку  (NUMBER)</summary>
            public decimal LockedBuyValue = 0;
            ///<summary>Стоимость ценных бумаг, заблокированных под продажу  (NUMBER)</summary>
            public decimal LockedSellValue = 0;

            ///<summary>Фьючерсы. Оценка текущих чистых позиций(NUMBER)</summary>
            public decimal CbplUsed = 0;
            ///<summary>Фьючерсы. Плановые чистые позиции(NUMBER)</summary>
            public decimal CbplPlanned = 0;
            ///<summary>Фьючерсы. Вариационная маржа(NUMBER)</summary>
            public decimal VarMargin = 0;
            ///<summary>Фьючерсы. Эффективная цена позиций(NUMBER)</summary>
            //public decimal AvrPosnPrice = 0;
            ///<summary>Фьючерсы. Реально начисленная в ходе клиринга вариационная маржа.Отображается с точностью до 2 двух знаков.При этом, в поле       "varmargin" транслируется вариационная маржа, рассчитанная с учетом установленных границ изменения цены  (NUMBER)</summary>
            public decimal RealVarMargin = 0;
            ///<summary>Фьючерсы. Суммарная вариационная маржа по итогам основного клиринга начисленная по всем позициям.Отображается с точностью до 2 двух знаков(NUMBER)</summary>
            public decimal TotalVarMargin = 0;
            ///<summary>Фьючерсы. SessionStatus</summary>
            public decimal SessionStatus = 0;
            ///<summary>Тип лимита  (NUMBER).</summary>
            public short Type;
        };
    }
}

using MarketObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServiceMessage
{
    public class ConvertorMsg
    {
        public delegate void EventStartMarket();
        public event EventStartMarket OnStartMarket;

        public QuikControl.QControlTerminal Trader = null;

        /// <summary> Последняя полученная сделка </summary>
        public Trade LastTrade = null;

        /// <summary> Последнее сообщение полученное от терминала. </summary>
        public string LastMessage = "";

        /// <summary> Название кода класса для торгуемых акций </summary>
        public static string CodesClassForStock = "";

        public ConvertorMsg(QuikControl.QControlTerminal trader)
        {
            this.Trader = trader;
        }

        /// <summary>Конвертор полученных сообщений </summary>
        /// <param name="MsgObject"></param>
        /// <param name="message"></param>
        public void NewMessage(MManager MsgObject, StackMsg elStack)
        {
            if (MsgObject != null && elStack != null)
            {
                if (elStack.Message == null) return;
                string message = elStack.Message.ToString();
                //Разбивает сообщение на данные
                string[] Params = message.Split('#');
                if (Params.Length > 0)
                {
                    switch (Params[0])
                    {
                        case "OnStartTrade":
                            MsgObject.Send("Data_loaded", "1");

                            if (OnStartMarket != null)
                                OnStartMarket();
                            break;
                        ///////////////////////////////////////////////////////
                        case "OnConnectInfo":
                            this.GetTerminalInfo(elStack, Params);
                            break;
                        case "OnTransReply":
                            this.GetTransReplyFromArrayMsg(elStack, Params);
                            break;
                        case "checkOnSecurities":
                            //this.checkSumSec = Convert.ToDecimal(Params[1]);
                            break;
                        case "OnFirm":
                            this.GetFirmFromArrayMsg(elStack, Params);
                            break;
                        case "OnClass":
                            this.GetClassFromArrayMsg(elStack, Params);
                            break;
                        case "OnSecurities":
                            this.GetSecuritiesFromArrayMsg(elStack, Params);
                            break;
                        case "OnChangeSecurities":
                            this.GetChangeSecuritiesFromArrayMsg(elStack, Params);
                            break;
                        case "OnAccount":
                            this.GetAccountsFromArrayMsg(elStack, Params);
                            break;
                        case "OnPortfolioInfo":
                            this.GetPortfoliosFromArrayMsg(elStack, Params);
                            break;
                        case "OnAccountPosition":
                            this.GetAccountPositionFromArrayMsg(elStack, Params);
                            break;
                        case "OnMoneyLimit":
                            this.GetMoneyLimitsFromArrayMsg(elStack, Params);
                            break;
                        case "OnDepoLimit":
                            this.GetDepoLimitFromArrayMsg(elStack, Params);
                            break;
                        case "OnFuturesHolding":
                            this.GetFuturesHoldingFromArrayMsg(elStack, Params);
                            break;
                        case "OnFuturesLimit":
                            this.GetFutLimitsFromArrayMsg(elStack, Params);
                            break;
                        case "OnFuturesLimitChange":
                            this.GetFutLimitsFromArrayMsg(elStack, Params);
                            break;
                        case "OnClient":
                            this.GetClientsFromArrayMsg(elStack, Params);
                            break;
                        case "OnOrder":
                            this.GetOrderFromArrayMsg(elStack, Params);
                            break;
                        case "OnStopOrder":
                            this.GetStopOrderFromArrayMsg(elStack, Params);
                            break;
                        case "OnQuote":
                            this.GetQuoteFromArrayMsg(elStack, Params);
                            break;
                        case "OnMyTrade":
                            this.GetMyTradeFromArrayMsg(elStack, Params);
                            break;
                        case "OnAllTrades":
                        case "OnAllTradesOld":
                            Trade t = this.GetTradeFromArrayMsg(elStack, Params);
                            //TEST контроль корректной последовательности получения сообщений

                            if (t != null)
                            {
                                if (LastTrade == null)
                                {
                                    LastTrade = t;
                                }
                                else
                                {
                                    if (LastTrade.Number + 1 != t.Number)
                                    {
                                        int er = 0;
                                    }
                                    LastTrade = t;
                                }
                            }
                            break;
                        default:
                            MessageBox.Show("Error msg: " + elStack.Message);
                            break;

                    }
                }
                LastMessage = message;
            }
        }
    

        /// <summary> Список активных инструментов </summary>
        private List<Securities> ActiveSec = new List<Securities>();

        /// <summary> Поиск инструмента среди активных, если не найден то добавляет его в активные. <summary>
        /// <param name="CodeSec"> Код инструмента</param>
        /// <param name="classCode">Код класса инструмента</param>
        /// <returns> Объекс Security, иначе null.</returns>
        public Securities FindSecurities(string CodeSec, string classCode)
        {
            Securities LastSecFind = this.ActiveSec.FirstOrDefault(s => s.Code == CodeSec && classCode.Contains(s.Class.Code));
            if (LastSecFind == null)
            {
                LastSecFind = Trader.tSecurities.AsIEnumerable.FirstOrDefault(s => s.Code == CodeSec && classCode.Contains(s.Class.Code));
                if (LastSecFind == null) return null;
                this.ActiveSec.Add(LastSecFind);
                return LastSecFind;
            }
            else return LastSecFind;
        }

        /// <summary> Поиск инструмента по ID cx, если не найден то добавляет его. </summary>
        /// <param name="AccID">ID аккаунта</param>
        /// <param name="SecCode">Код инструмента</param>
        /// <returns></returns>
        public Securities FindSecuritiesByAccID(string AccID, string SecCode)
        {
            var acc = Trader.Accounts.FirstOrDefault(a => a.AccID == AccID);
            if (acc != null)
            {
                foreach (var cl in acc.AccClasses)
                {
                    var sec = Trader.Securities.FirstOrDefault(s => s.Code == SecCode && s.Class.Code == cl.Code);
                    if (sec != null) return sec;
                }
            }
            return null;
        }

        /// <summary> Список активных фирм </summary>
        private List<Firm> ActiveFirms = new List<Firm>();
        /// <summary> Поиск фирмы среди активных, если не найден то добавляет его. </summary>
        public Firm FindFirm(string idFirm)
        {
            Firm LastFindFirm = this.ActiveFirms.FirstOrDefault(f => f.Id == idFirm);
            if (LastFindFirm == null)
            {
                LastFindFirm = Trader.Firms.FirstOrDefault(f => f.Id == idFirm);
                if (LastFindFirm == null) return null;
                this.ActiveFirms.Add(LastFindFirm);
                return LastFindFirm;
            }
            else return LastFindFirm;
        }

        /// <summary> Список активных классов </summary>
        private List<MarketClass> ActiveClasses = new List<MarketClass>();
        /// <summary> Поиск класса среди активных, если не найден то добавляет его. </summary>
        public MarketClass FindClass(string Code)
        {
            MarketClass LastFindClass = this.ActiveClasses.FirstOrDefault(c => c.Code == Code);
            if (LastFindClass == null)
            {
                LastFindClass = Trader.Classes.FirstOrDefault(c => c.Code == Code);
                if (LastFindClass == null) return null;
                this.ActiveClasses.Add(LastFindClass);
                return LastFindClass;
            }
            else return LastFindClass;
        }

        /// <summary>
        /// Преобразование сообщения в класс TerminalInfo
        /// </summary>
        /// <param name="m"></param>
        /// <param name="Msg"></param>
        public void GetTerminalInfo(StackMsg elStack, string[] Msg)
        {
            if (Msg == null) return;
            int Count = Msg.Length;
            if (Count > 2 && Msg != null)
            {
                //"OnConnectInfo , 1 "VERSION", 2 "TRADEDATE", 3 "SERVERTIME", 4 "LASTRECORDTIME", 5 "CONNECTION", 6 "IPADDRESS",
                //7 "IPPORT", 8 "SERVER", 9 "USER", 10 "USERID", 11 "ORG", 12 "LOCALTIME", 13 "CONNECTIONTIME",
                //14 "MAXPINGTIME", 15 "MAXPINGDURATION"
                Trader.Terminal.Version = Msg[1];
                Trader.Terminal.TradeDate = Convert.ToDateTime(Msg[2]);
                if (Msg[3] != "") Trader.Terminal.ServerTime = Convert.ToDateTime(Msg[3]).TimeOfDay;
                Trader.Terminal.LastRecordTime = Msg[4] != "" ? Convert.ToDateTime(Msg[4]).TimeOfDay : TimeSpan.MinValue;

                if (Msg[5] == "установлено")
                {
                    Trader.Terminal.Connect = true;
                }
                if (Msg[5] == "не установлено")
                {
                    Trader.Terminal.Connect = false;
                }

                Trader.Terminal.IpServerAddr =          Msg[6];
                Trader.Terminal.Port =                  Msg[7];
                Trader.Terminal.ServerDescription =     Msg[8];
                Trader.Terminal.User =                  Msg[9];
                Trader.Terminal.UserID =                Msg[10];
                Trader.Terminal.Organization =          Msg[11];
                Trader.Terminal.Localtime =             Convert.ToDateTime(Msg[12]).TimeOfDay;
                Trader.Terminal.ConnectionTime =        Convert.ToDateTime(Msg[12]).TimeOfDay;
            }
        }
        /// <summary> Преобразование сообщения в класс Сделки </summary>
        /// <param name="m"></param>
        /// <param name="MsgTrade"></param>
        /// <returns></returns>
        public Trade GetTradeFromArrayMsg(StackMsg elStack, string[] MsgTrade)
        {
            if (MsgTrade == null) return null;
            // 0 event | 1 trade_num | 2 sec_code | 3 class_code | 4 price | 5 qty | 
            // 6 datetime.week_day | 7 datetime.hour | 8 datetime.ms | 9 datetime.mcs | 10 datetime.day | 
            // 11 datetime.month | 12 datetime.sec | 13 datetime.year | 14 datetime.min | 15 flags | 16 value | 17 open_interest 
            int Count = MsgTrade.Length;
            if (Count > 5 && MsgTrade != null)
            {
                Trade newTrade = new Trade();
                Common.DateMarket date = new Common.DateMarket();

                newTrade.Number = MsgTrade[1].ToLong();
                newTrade.SecCode = MsgTrade[2];
                newTrade.Sec = this.FindSecurities(newTrade.SecCode, MsgTrade[3]);

                newTrade.Price = MsgTrade[4].ToDecimal(newTrade.Sec.Scale);
                newTrade.Volume = MsgTrade[5].ToInt32();

                date.day = MsgTrade[10];
                date.month = MsgTrade[11];
                date.year = MsgTrade[13];

                date.hour = MsgTrade[7];
                date.min = MsgTrade[14];
                date.sec = MsgTrade[12];
                date.mcs = MsgTrade[9];

                newTrade.DateTrade = Convert.ToDateTime(date.DateTime);
                BitArray bitsFlags = new BitArray(new int[] { MsgTrade[15].ToInt32() });

                if (bitsFlags[0] == true) newTrade.Direction = OrderDirection.Buy;
                if (bitsFlags[1] == true) newTrade.Direction = OrderDirection.Sell;

                newTrade.OpenInterest = MsgTrade[17] != "" ? MsgTrade[17].ToDecimal() : 0;

                //Считаем сумму всех объемов за сессию
                if (newTrade.Direction == OrderDirection.Buy) newTrade.Sec.SumAllTradesBuy += newTrade.Volume;
                else newTrade.Sec.SumAllTradesSell += newTrade.Volume;

                //последняя сделка
                if(newTrade.Sec.LastTrade.Empty())
                    newTrade.Sec.LastTrade = newTrade;
                else
                {
                    if(newTrade.Sec.LastTrade.Number < newTrade.Number)
                        newTrade.Sec.LastTrade = newTrade;
                }

                if (newTrade.Number > 0)
                {
                    Trader.tTrades.Add(newTrade, false);
                }
                return newTrade;
            }
            return null;
        }

        /// <summary> Обработка сообщения со стоп заявкой. </summary>
        /// <param name="m">Сообщение</param>
        /// <param name="Msg">Сообщение в виде массива</param>
        /// <param name="Trader">Главный объект Trader</param>
        /// <returns></returns>
        public StopOrder GetStopOrderFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            if (Msg == null) return null;

            if (Msg.Length > 5)
            {
                // 0 OnStopOrder | 1 firmid | 2 sec_code | 3 order_num |  4 client_code | 
                // 5 ordertime | 6 price | 7 qty | 8 balance | 9 condition | 10 stopflags | 
                // 11 withdraw_time | 12 offset | 13 order_date_time.week_day | 14 order_date_time.hour | 
                // 15 order_date_time.ms | 16 order_date_time.mcs | 17 order_date_time.day | 18 order_date_time.month | 
                // 19 order_date_time.sec | 20 order_date_time.year | 21 order_date_time.min | 22 class_code | 
                // 23 condition_sec_code | 24 expiry | 25 flags | 26 co_order_num | 27 linkedorder | 
                // 28 alltrade_num | 29 condition_price | 30 condition_price2 | 31 co_order_price | 
                // 32 spread | 33 withdraw_datetime.week_day | 34 withdraw_datetime.hour | 35 withdraw_datetime.ms | 
                // 36 withdraw_datetime.mcs | 37 withdraw_datetime.day | 38 withdraw_datetime.month | 
                // 39 withdraw_datetime.sec | 40 withdraw_datetime.year | 41 withdraw_datetime.min | 42 active_to_time | 
                // 43 condition_class_code | 44 condition_seccode | 45 canceled_uid | 46 active_from_time | 
                // 47 uid | 48 brokerref | 49 filled_qty | 50 trans_id | 51 stop_order_type

                StopOrder s_order = new StopOrder();
                s_order.uid = Msg[47] != "" ? Msg[47].ToDecimal() : 0;
                int flags = Msg[25].ToInt32();
                s_order.SecCode = Msg[2];
                s_order.Sec = this.FindSecurities(s_order.SecCode, Msg[22]);

                s_order.OrderNumber = Msg[3].ToLong();
                s_order.TransID = Msg[50].ToLong();
                //Заявка не прошла (или отброшена из-за ненадобности)
                //if (s_order.uid == 0) return null;
                StopOrder orFind = Trader.StopOrders.FirstOrDefault(or => or.OrderNumber == s_order.OrderNumber && or.SecCode == s_order.SecCode);
                bool changeOrder = true;
                if (orFind == null)
                {
                    orFind = s_order;
                    Trader.tStopOrders.Add(orFind, false);
                    changeOrder = false;
                }

                orFind.Price = Msg[6].ToDecimal(orFind.Sec.Scale); 
                orFind.ConditionPrice = Msg[29].ToDecimal(orFind.Sec.Scale); 
                orFind.ConditionPrice2 = Msg[30].ToDecimal(orFind.Sec.Scale); 
                orFind.Volume = Msg[7].ToInt32();
                orFind.Balance = Msg[8].ToInt32();
                orFind.Offset = Msg[12].ToDecimal(); 
                orFind.LinkOrderNum = Msg[26].ToLong();
                orFind.LinkOrderPrice = Msg[31].ToDecimal(orFind.Sec.Scale);
                orFind.OrderNumExecute = Msg[27].ToLong();
                orFind.ConditionClassCode = Msg[43];
                orFind.ConditionSecCode = Msg[44];
                /*if (orFind.OrderExecute != null)
                {
                    decimal t = orFind.OrderExecute.OrderNumber;
                }*/
                int cond = Msg[9].ToInt32();
                if (cond == (int)ConditionStopOrder.MoreOrEqual)
                    orFind.Condition = ConditionStopOrder.MoreOrEqual;
                else orFind.Condition = ConditionStopOrder.LessOrEqual;
                orFind.Spread =     Msg[32].ToDecimal(orFind.Sec.Scale);

                Common.DateMarket date = new Common.DateMarket();
                date.day = Msg[17];
                date.month = Msg[18];
                date.year = Msg[20];

                date.hour = Msg[14];
                date.min = Msg[21];
                date.sec = Msg[19];
                date.mcs = Msg[16];
                orFind.DateCreateOrder = Convert.ToDateTime(date.DateTime);

                Common.DateMarket dateWithDr = new Common.DateMarket();
                dateWithDr.day = Msg[37];
                dateWithDr.month = Msg[38];
                dateWithDr.year = Msg[40];

                dateWithDr.hour = Msg[34];
                dateWithDr.min = Msg[41];
                dateWithDr.sec = Msg[39];
                dateWithDr.mcs = Msg[36];
                orFind.WithDrawTime = Convert.ToDateTime(dateWithDr.DateTime);

                orFind.Comment = Msg[48];

                orFind.TypeStopOrder = (StopOrderType)Convert.ToInt32(Msg[51]);

                orFind.DateExpiry = Msg[24].ConvertToDateForm_YYYYMMDD();


                BitArray bitsFlags = new BitArray(new int[] { flags });

                if (bitsFlags[0] == true)
                {
                    orFind.Status = OrderStatus.ACTIVE;
                }
                else if (bitsFlags[1] == true)
                {
                    orFind.Status = OrderStatus.CLOSED;
                }
                else
                {
                    orFind.Status = OrderStatus.EXECUTED;
                }

                if (bitsFlags[2] == true)
                {
                    orFind.Direction = OrderDirection.Sell;
                }
                else
                {
                    orFind.Direction = OrderDirection.Buy;
                }

                if (changeOrder)
                {
                    Trader.tStopOrders.Change(orFind, false);// !MManager.EventPackages);
                    return orFind;
                }
                return null;
            }
            return null;
        }

        public Order GetOrderFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            if (Msg == null) return null;

            int Count = Msg.Length;
            if (Count > 5 && Msg != null)
            {
                Order ord = new Order();
                // 0 OnOrder | 1 sec_code | 2 ordernum | 3 userid | 4 client_code | 5 firmid | 
                // 6 account | 7 flags | 8 price | 9 balance | 10 value | 11 qty | 12 canceled_uid | 
                // 13 uid | 14 week_day | 15 hour | 16 ms | 17 mcs | 18 day | 19 month | 20 sec | 
                // 21 year | 22 min | 23 brokerref | 24 trans_id | 25 class_code

                string userid =     Msg[3];
                string clientCode = Msg[4];
                string firmid =     Msg[5];
                int flags =         Msg[7].ToInt32();
                ord.OrderNumber =   Msg[2].ToLong();
                ord.SecCode =       Msg[1];
                ord.Sec = this.FindSecurities(ord.SecCode, Msg[25]);
                Order orFind = Trader.Orders.FirstOrDefault(or => or.OrderNumber == ord.OrderNumber && or.SecCode == ord.SecCode);

                bool change = true;
                if (orFind == null)
                {
                    orFind = ord;
                    Trader.tOrders.Add(orFind, false); // !MManager.EventPackages);
                    change = false;
                }

                orFind.OrderTrades = Trader.MyTrades.Where(mt => mt.OrderNum == ord.OrderNumber);
                if (orFind.OrderTrades != null) orFind.OrderTrades.ToList().ForEach(t => t.Order = ord);

                orFind.Price = Msg[8].ToDecimal(orFind.Sec.Scale);
                orFind.Volume = Msg[11].ToInt32();
                orFind.Balance = Msg[9].ToInt32();
                orFind.Value = Msg[10].ToDecimal(2);

                Common.DateMarket date = new Common.DateMarket();
                date.day = Msg[18];
                date.month = Msg[19];
                date.year = Msg[21];

                date.hour = Msg[15];
                date.min = Msg[22];
                date.sec = Msg[20];
                date.mcs = Msg[17];
                orFind.DateCreateOrder = Convert.ToDateTime(date.DateTime);

                orFind.Comment =    Msg[23];
                orFind.uid =        Msg[13].ToDecimal();
                orFind.TransID =    Msg[24].ToInt32();

                BitArray bitsFlags = new BitArray(new int[] { flags });

                if (bitsFlags[0] == true)
                {
                    orFind.Status = OrderStatus.ACTIVE;
                }
                else if (bitsFlags[1] == true)
                {
                    orFind.Status = OrderStatus.CLOSED;
                }
                else
                {
                    orFind.Status = OrderStatus.EXECUTED;
                }

                if (bitsFlags[2] == true)
                {
                    orFind.Direction = OrderDirection.Sell;
                }
                else
                {
                    orFind.Direction = OrderDirection.Buy;
                }
                //Заявка не прошла (или отброшена из-за ненадобности)
                //if (orFind.uid == 0) return null;

                if (change)
                {
                    Trader.tOrders.Change(orFind, false);// !MManager.EventPackages);
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Преобразование сообщения в класс "Моя сделка"
        /// </summary>
        /// <param name="elStack"></param>
        /// <param name="MsgTrade"></param>
        /// <returns></returns>
        public MyTrade GetMyTradeFromArrayMsg(StackMsg elStack, string[] MsgTrade)
        {
            if (MsgTrade == null) return null;

            int Count = MsgTrade.Length;
            if (Count > 5 && MsgTrade != null)
            {
                /////// Forming Trade /////////
                Trade trade = new Trade();
                // 0 OnMyTrade | 1 trade_num | 2 sec_code | 3 class_code | 4 price | 5 qty | 
                // 6 datetime.week_day | 7 datetime.hour | 8 datetime.ms | 9 datetime.mcs | 10 datetime.day | 11 datetime.month | 
                // 12 datetime.sec | 13 datetime.year | 14 datetime.min | 15 flags | 16 value | 17 order_num | 
                // 18 uid | 19 broker_comission | 20 exchange_comission | 21 clearing_comission | 22 block_securities | 23 brokerref
                Common.DateMarket date = new Common.DateMarket();

                trade.Number =  MsgTrade[1].ToLong();
                trade.SecCode = MsgTrade[2];
                trade.Sec =     this.FindSecurities(trade.SecCode, MsgTrade[3]);

                trade.Price =   MsgTrade[4].ToDecimal(trade.Sec.Scale);
                trade.Volume =  Convert.ToInt32(MsgTrade[5]);

                date.day = MsgTrade[10];
                date.month = MsgTrade[11];
                date.year = MsgTrade[13];
                date.hour = MsgTrade[7];
                date.min = MsgTrade[14];
                date.sec = MsgTrade[12];
                date.mcs = MsgTrade[9];

                trade.DateTrade = Convert.ToDateTime(date.DateTime);

                /////////// Construct my trade ///////////
                MyTrade my_trade = new MyTrade(trade);
                my_trade.OrderNum =             MsgTrade[17].ToLong();
                my_trade.uid =                  MsgTrade[18].ToLong();
                my_trade.BrokerComission =      MsgTrade[19].ToDecimal(2);
                my_trade.ExchangeComission =    MsgTrade[20].ToDecimal(2); 
                my_trade.ClearingComission =    MsgTrade[21].ToDecimal(2); 
                my_trade.BlockSecurities =      MsgTrade[22].ToDecimal(2); 
                my_trade.Comment =              MsgTrade[23];

                if (my_trade.Trade.Number > 0 && my_trade.OrderNum > 0)
                {
                    //из-за того что приходит 2 дубликата откидываем один.
                    MyTrade mT = Trader.MyTrades.FirstOrDefault(mt => mt.Trade.Number == my_trade.Trade.Number && mt.OrderNum == my_trade.OrderNum);
                    if (mT == null)
                    {
                        //Поиск своей заявки и добавлении транзакции в список сделок заявки.
                        Order or = Trader.Orders.FirstOrDefault(o => o.OrderNumber == my_trade.OrderNum);

                        if (or != null)
                        {
                            my_trade.Trade.Direction = or.Direction;
                            or.OrderTrades = Trader.MyTrades.Where(mt => mt.OrderNum == or.OrderNumber);
                            my_trade.Order = or;
                        }
                        if (my_trade.Trade.Direction == null)
                        {
                            BitArray bitsFlags = new BitArray(new int[] { MsgTrade[15].ToInt32() });
                            if (bitsFlags[2] == true) my_trade.Trade.Direction = OrderDirection.Sell;
                            else my_trade.Trade.Direction = OrderDirection.Buy;
                        }
                        Trader.tMyTrades.Add(my_trade);
                        return my_trade;
                    }
                    return null;
                }
                return null;
            }
            return null;
        }

        //Преобразование сообщения в класс Фирмы
        public Firm GetFirmFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            if (Msg.Length > 0)
            {
                Firm firm = new Firm();
                //0 OnFirm | 1 firmid | 2 exchange | 3 status | 4 firm_name
                firm.Id = Msg[1];
                firm.Exchange = Msg[2];
                firm.Status = Msg[3].ToInt32();
                firm.Name = Msg[4];

                if (firm.Id != "")
                {
                    Trader.tFirms.Add(firm, false);// !MManager.EventPackages);
                    return firm;
                }
                return null;
            }
            return null;
        }

        //Преобразование сообщения в класс MarketClass
        public MarketClass GetClassFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            if (Msg.Length > 0)
            {
                //System.IO.File.AppendAllText(@"debug_file_111.txt", elStack.Message + "\n");
                MarketClass marketClass = new MarketClass();
                //0 OnClass | 1 firmid | 2 code | 3 name | 4 npars | 5 nsecs

                marketClass.FirmId = Msg[1];
                marketClass.Firm = Trader.Firms.FirstOrDefault(f => f.Id == marketClass.FirmId);
                marketClass.Code = Msg[2];
                marketClass.Name = Msg[3];

                marketClass.CountParams = Msg[4].ToInt32();
                marketClass.CountSecurities = Msg[5].ToInt32();

                if (marketClass.Code != null)
                {
                    Trader.tClasses.Add(marketClass);
                    return marketClass;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Преобразование сообщения в класс Securities
        /// </summary>
        /// <param name="m">Сообщение</param>
        /// <param name="Msg">Разбитое сообщение на массив</param>
        /// <param name="Trader">Объект Trader</param>
        /// <returns></returns>
        public Securities GetSecuritiesFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    Securities NewSec = new Securities();
                    // 0 OnSecurities | 1 sec_code | 2 class_code | 3 name | 4 short_name | 5 face_unit | 
                    // 6 scale | 7 face_value | 8 lot_size | 9 mat_date | 10 min_step_price

                    NewSec.Code = Msg[1];
                    NewSec.ClassCode = Msg[2];
                    NewSec.Class = Trader.Classes.ToArray().FirstOrDefault(c => c.Code == NewSec.ClassCode);
                    Trader.tSecurities.LockCollection();
                    var sec = Trader.Securities.FirstOrDefault(s => s.Code == NewSec.Code && s.ClassCode == NewSec.Class.Code);
                    Trader.tSecurities.UnLockCollection();
                    if (sec == null)
                    {
                        try { NewSec.Scale = Msg[6].ToInt32(); }
                        catch (Exception) { NewSec.Scale = 0; }

                        try { NewSec.Params.MinPriceStep = Msg[10].ToDecimal();}
                        catch (Exception) { NewSec.Params.MinPriceStep = 0; }

                        NewSec.Lot = Msg[8].ToInt32();

                        NewSec.Name = Msg[3];
                        NewSec.Shortname = Msg[4];
                        NewSec.Params.FaceUnit = Msg[5];

                        try { NewSec.Params.FaceValue = Msg[7].ToDecimal(); }
                        catch (Exception) { NewSec.Params.FaceValue = 0; }

                        NewSec.Params.MatDate = Msg[9] != "" ? Msg[9].ToLong() : 0;
                    }
                    else
                    {
                        NewSec = null;
                    }

                    if (NewSec != null)
                    {
                        Trader.tSecurities.Add(NewSec, false);// !MManager.EventPackages);
                        return NewSec;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + elStack.Message);
            }
            return null;
        }


        //Преобразование сообщения в класс ChangeSecurities
        public Securities GetChangeSecuritiesFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    Securities chSec = new Securities();
                    // 0 OnChangeSecurities| 1 CODE | 2 0.000000 | 3 ROSN | 4 CLASS_CODE | 5 0.000000 | 6 QJSIM | 7 SEC_SCALE | 8 2.000000 | 9 2 |
                    // STATUS | 0.000000 |  | LOTSIZE | 10.000000 | 10 | BID | 311.400000 | 311,40 | 
                    // BIDDEPTH | 0.000000 |  | BIDDEPTHT | 1260.000000 | 1 260 | NUMBIDS | 0.000000 |  | 
                    // OFFER | 311.500000 | 311,50 | OFFERDEPTH | 0.000000 |  | OFFERDEPTHT | 1480.000000 | 1 480 | 
                    // NUMOFFERS | 0.000000 |  | OPEN | 316.000000 | 316,00 | HIGH | 0.000000 |  | LOW | 0.000000 |  | 
                    // LAST | 311.500000 | 311,50 | CHANGE | 0.000000 |  | QTY | 0.000000 |  | TIME | 171823.000000 | 17:18:23 | 
                    // VOLTODAY | 0.000000 |  | VALTODAY | 1378179001.000000 | 1 378 179 001 | 
                    // TRADINGSTATUS | 1.000000 | открыта | VALUE | 0.000000 |  | WAPRICE | 0.000000 |  | 
                    // HIGHBID | 0.000000 |  | LOWOFFER | 0.000000 |  | NUMTRADES | 33454.000000 | 33 454 | 
                    // PREVPRICE | 239.880000 | 239,88 | PREVWAPRICE | 0.000000 |  | CLOSEPRICE | 0.000000 |  | 
                    // LASTCHANGE | 0.000000 | 0,00 | PRIMARYDIST | 0.000000 |  | ACCRUEDINT | 0.000000 |  | 
                    // YIELD | 0.000000 |  | LONGNAME | 0.000000 | ОАО \"НК \"Роснефть\" | SHORTNAME | 0.000000 | Роснефть | 
                    // TRADE_DATE_CODE | 20170528.000000 | 28.05.2017 | MAT_DATE | 0.000000 |  | DAYS_TO_MAT_DATE | 0.000000 |  | 
                    // SEC_FACE_VALUE | 1.000000 | 1,00 | SEC_FACE_UNIT | 0.000000 | SUR | 
                    // SEC_PRICE_STEP | 0.050000 | 0,05 | SECTYPE | 0.000000 | Ценные бумаги | PRICEMAX | 0.000000 |  | 
                    // PRICEMIN | 0.000000 |  | NUMCONTRACTS | 0.000000 |  | BUYDEPO | 0.000000 |  | SELLDEPO | 0.000000 |  | 
                    // CHANGETIME | 0.000000 |  | TRADECHANGE | 0.000000 |  | FACEVALUE | 0.000000 |  | 
                    // MARKETPRICETODAY | 0.000000 |  | BUYBACKPRICE | 0.000000 |  | BUYBACKDATE | 0.000000 |  | 
                    // ISSUESIZE | 0.000000 |  | PREVDATE | 0.000000 |  | LOPENPRICE | 0.000000 |  | 
                    // LCURRENTPRICE | 0.000000 |  | LCLOSEPRICE | 0.000000 |  | ISPERCENT | 0.000000 |  | 
                    // CLSTATE | 0.000000 |  | CLPRICE | 0.000000 |  | STARTTIME | 0.000000 |  | ENDTIME | 0.000000 |  | 
                    // EVNSTARTTIME | 0.000000 |  | EVNENDTIME | 0.000000 |  | CURSTEPPRICE | 0.000000 |  | 
                    // REALVMPRICE | 0.000000 |  | MARG | 0.000000 |  | EXPDATE | 0.000000
                    chSec.Code = Msg[3];
                    chSec.Class = this.FindClass(Msg[6]);
                    if (chSec.Class == null) return null;
                    Securities findSec = Trader.Securities.FirstOrDefault(s => s.Code == chSec.Code && s.Class.Code == chSec.Class.Code);
                    if (findSec == null) return null;

                    findSec.Scale = Msg[9].ToInt32();

                    for (int i = 1; i < Msg.Length; i++)
                    {
                        if (Msg[i] == "STATUS")
                        {
                            findSec.Params.Status = Msg[i + 1].ToInt32();
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "SEC_FACE_UNIT")
                        {
                            findSec.Params.FaceUnit = Msg[i + 2];
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "BID")
                        {
                            findSec.Params.Bid = Msg[i + 1].ToDecimal(findSec.Scale);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "OFFER")
                        {
                            findSec.Params.Ask = Msg[i + 1].ToDecimal(findSec.Scale);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "BIDDEPTH")
                        {
                            findSec.Params.BidDepth = Msg[i + 1].ToDecimal(findSec.Scale);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "OFFERDEPTH")
                        {
                            findSec.Params.AskDepth = Msg[i + 1].ToDecimal(findSec.Scale); 
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "BIDDEPTHT")
                        {
                            findSec.Params.SumBidDepth = Msg[i + 1].ToDecimal(0);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "OFFERDEPTHT")
                        {
                            findSec.Params.SumAskDepth = Msg[i + 1].ToDecimal(0);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "NUMBIDS")
                        {
                            findSec.Params.NumBid = Msg[i + 1].ToLong();
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "NUMOFFERS")
                        {
                            findSec.Params.NumAsk = Msg[i + 1].ToLong();
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "BUYDEPO")
                        {
                            findSec.Params.BuyDepo = Msg[i + 1].ToDecimal(2);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "SELLDEPO")
                        {
                            findSec.Params.SellDepo = Msg[i + 1].ToDecimal(2);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "SEC_FACE_VALUE")
                        {
                            findSec.Params.FaceValue = Msg[i + 1].ToDecimal();
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "SHORTNAME")
                        {
                            findSec.Shortname = Msg[i + 2];
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "LONGNAME")
                        {
                            findSec.Name = Msg[i + 2];
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "LOTSIZE")
                        {
                            findSec.Lot = Msg[i + 1].ToInt32();
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "SEC_PRICE_STEP")
                        {
                            findSec.Params.MinPriceStep = Msg[i + 1].ToDecimal(findSec.Scale);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "MAT_DATE")
                        {
                            findSec.Params.MatDate = Msg[i + 1] != "" ? Msg[i + 1].ToLong() : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "SECTYPE")
                        {
                            findSec.Params.SecType = Msg[i + 2];
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "DAYS_TO_MAT_DATE")
                        {
                            findSec.Params.DaysToMatDate = Msg[i + 2] != "" ? Msg[i + 2].ToInt32() : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "TIME")
                        {
                            if (Msg[i + 2] != "") findSec.Params.TimeLastTrade = TimeSpan.Parse(Msg[i + 2]);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "QTY")
                        {
                            findSec.Params.VolumeLastTrade = Msg[i + 1] != "" ? Msg[i + 1].ToInt32() : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "LAST")
                        {
                            findSec.Params.LastPrice = Msg[i + 1] != "" ? Msg[i + 1].ToDecimal(findSec.Scale) : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "OPEN")
                        {
                            findSec.Params.Open = Msg[i + 1] != "" ? Msg[i + 1].ToDecimal(findSec.Scale) : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "HIGH")
                        {
                            findSec.Params.High = Msg[i + 1] != "" ? Msg[i + 1].ToDecimal(findSec.Scale) : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "LOW")
                        {
                            findSec.Params.Low = Msg[i + 1] != "" ? Msg[i + 1].ToDecimal(findSec.Scale) : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "VALTODAY")
                        {
                            findSec.Params.ValToday = Msg[i + 1] != "" ? Msg[i + 1].ToDecimal(2) : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "TRADINGSTATUS")
                        {
                            findSec.Params.TradingStatus = Msg[i + 1] != "" ? Msg[i + 1].ToInt32() : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "NUMTRADES")
                        {
                            findSec.Params.NumTrades = Msg[i + 1] != "" ? Msg[i + 1].ToLong() : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "PREVPRICE")
                        {
                            findSec.Params.PriceClose = Msg[i + 1] != "" ? Msg[i + 1].ToDecimal(findSec.Scale) : 0;
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "TRADE_DATE_CODE")
                        {
                            if (Msg[i + 2] != "") findSec.Params.TradeDate = Convert.ToDateTime(Msg[i + 2]);
                            i = i + 2;
                            continue;
                        }
                        if (Msg[i] == "YIELD")
                        {
                            findSec.Params.Yield = Msg[i + 1] != "" ? Msg[i + 1].ToDecimal(2) : 0;
                            i = i + 2;
                            continue;
                        }
                    }

                    if (findSec != null)
                    {
                        Trader.tSecurities.Change(findSec, false);
                        return findSec;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        //Преобразование сообщения в класс Account
        public Account GetAccountsFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    Account acc = new Account();
                    // 0 OnAccount | 1 class_codes | 2 firmid | 3 trdaccid | 4 trdacc_type
                    string[] listC = Msg[1].Trim('|').Split('|');
                    listC.ToList().ForEach(el =>
                    {
                        if (el != "" && el != null)
                        {
                            var cl = Trader.Classes.FirstOrDefault((c) => c.Code == el);
                            if (cl != null) acc.AccClasses.Add(cl);
                            else cl = null;
                        }
                    });
                    acc.Firm = Trader.Firms.FirstOrDefault((f) => f.Id == Msg[2]);
                    acc.AccID = Msg[3];
                    acc.AccType = Msg[4].ToInt32();

                    if (acc.AccID != null && acc.AccID != "" && acc.AccClasses.Count > 0)
                    {
                        Trader.tAccounts.Add(acc);
                        return acc;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// Получение объекта "Клиент" из сообщения.
        /// </summary>
        /// <param name="elStack"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public Client GetClientsFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length >= 2)
                {
                    Client cl = new Client();
                    if (Msg[1] != "")
                    {
                        cl.Code = Msg[1];
                        if (cl.Code.Length > 0)
                        {
                            Trader.tClients.Add(cl, false);
                            return cl;
                        }
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + elStack.Message);
            }
            return null;
        }

        /// <summary> Получение аккаунта из сообщения </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        public Client GetAccountPositionFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            if (Msg.Length > 0)
            {
                /*Client cl = new QuikVEC.Client();
                if (Msg[1] != "") cl.Code = Msg[1].Trim(' ', '\r', '\n');
                Msg = null;
                if (cl.Code.Length > 0)
                {
                    return cl;
                }*/
                return null;
            }
            return null;
        }
        public Position GetDepoLimitFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    short limit_kind = Convert.ToInt16(Msg[1]);
                    if (limit_kind != 0) return null;

                    Position _Pos = null;
                    // 0 OnDepoLimit | 1 limit_kind | 2 sec_code | 3 trdaccid | 4 firmid | 5 client_code | 
                    // 6 openbal | 7 openlimit | 8 currentbal | 9 currentlimit | 10 locked_sell | 
                    // 11 locked_buy | 12 locked_buy_value | 13 locked_sell_value | 14 awg_position_price

                    string SecCode = Msg[2];
                    Firm firm = this.FindFirm(Msg[4]);
         
                    Account Account = Trader.Accounts.FirstOrDefault(a => a.AccID == Msg[3]);
                    Securities Sec = this.FindSecurities(SecCode, ConvertorMsg.CodesClassForStock);
                    Client client = Trader.Clients.FirstOrDefault(c => c.Code.Contains(Msg[5]));

                    if (Sec == null) return null;

                    if (SecCode != "" && Account != null && firm != null)
                    {
                        _Pos = Trader.Positions.FirstOrDefault(p => p.SecCode == SecCode && p.Account.AccID == Account.AccID);
                    }
                    bool change = false;
                    if (_Pos == null)
                    {
                        _Pos = new Position();
                        _Pos.SecCode = SecCode;
                        _Pos.Sec = Sec;
                        _Pos.Account = Account;
                        _Pos.Firm = firm;
                        _Pos.Client = client;
                        Trader.tPositions.Add(_Pos, false);//!MManager.EventPackages);
                    }
                    else change = true;

                    _Pos.Data.StartBuy = Convert.ToInt32(Msg[6]);
                    _Pos.Data.StartNet = _Pos.Data.StartBuy;
                    _Pos.Data.OpenLimit = Msg[7].ToDecimal(0);

                    _Pos.Data.TodayBuy = Msg[8].ToInt32();
                    _Pos.Data.CurrentNet = _Pos.Data.TodayBuy;

                    _Pos.Data.PositionValue = Msg[9].ToDecimal(2);

                    _Pos.Data.OrdersSell = Msg[10].ToInt32();
                    _Pos.Data.OrdersBuy = Msg[11].ToInt32();

                    _Pos.Data.LockedBuyValue = Msg[12].ToDecimal(2);
                    _Pos.Data.LockedSellValue = Msg[13].ToDecimal(2);

                    _Pos.Data.AwgPositionPrice = Msg[14].ToDecimal(2);

                    _Pos.Data.Type = limit_kind;

                    if (change)
                    {
                        Trader.tPositions.Change(_Pos, false);// !MManager.EventPackages);
                    }

                    if (_Pos.Account!= null) return _Pos;
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// Получение лимитов по фьючерсам из сообщения. (Позиции)
        /// </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        public Position GetFuturesHoldingFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    short type = Convert.ToInt16(Msg[11]);
                    //if (type != 0) return null;

                    Position _Pos = null;
                    // 0 OnFuturesHolding | 1 sec_code | 2 firmid | 3 trdaccid | 4 startbuy | 5 startnet | 6 startsell | 
                    // 7 openbuys | 8 opensells | 9 todaybuy | 10 totalnet | 11 todaysell,
                    // 12 real_varmargin | 13 type | 14 avrposnprice | 15 positionvalue | 16 total_varmargin |
                    // 17 session_status | 18 varmargin | 19 cbplplanned | 20 cbplused |

                    string SecCode = Msg[1];
                    Firm firm = this.FindFirm(Msg[2]);
                    //Securities Sec = this.FindSecuritiesByCode(SecCode, Trader);
                    Account Account = Trader.Accounts.FirstOrDefault(a => a.AccID == Msg[3]);
                    Securities Sec = this.FindSecuritiesByAccID(Account.AccID, SecCode);

                    if (SecCode != "" && Account != null && firm != null)
                    {
                        _Pos = Trader.Positions.FirstOrDefault(p => p.SecCode == SecCode && p.Account.AccID == Account.AccID);
                    }
                    bool change = false;
                    if (_Pos == null)
                    {
                        _Pos = new Position();
                        _Pos.SecCode = SecCode;
                        _Pos.Sec = Sec;
                        _Pos.Account = Account;
                        _Pos.Firm = firm;
                        Trader.tPositions.Add(_Pos);
                    }
                    else change = true;

                    _Pos.Data.StartBuy =    Msg[4].ToInt32(); 
                    _Pos.Data.StartSell =   Msg[6].ToInt32();
                    _Pos.Data.StartNet =    Msg[5].ToInt32(); 

                    _Pos.Data.OrdersBuy =   Msg[7].ToInt32(); 
                    _Pos.Data.OrdersSell =  Msg[8].ToInt32(); 
                    _Pos.Data.TodayBuy =    Msg[9].ToInt32(); 
                    _Pos.Data.TodaySell =   Msg[11].ToInt32(); 
                    _Pos.Data.CurrentNet =  Msg[10].ToInt32(); 

                    _Pos.Data.TotalVarMargin =  Msg[16].ToDecimal(2); 
                    _Pos.Data.RealVarMargin =   Msg[12].ToDecimal(2);  
                    _Pos.Data.AwgPositionPrice= Msg[14].ToDecimal(2); 
                    _Pos.Data.PositionValue =   Msg[15].ToDecimal(2);  
                    _Pos.Data.SessionStatus =   Msg[17].ToDecimal(2);  
                    _Pos.Data.VarMargin =       Msg[18].ToDecimal(2);  
                    _Pos.Data.CbplPlanned =     Msg[19].ToDecimal(2);  
                    _Pos.Data.CbplUsed =        Msg[20].ToDecimal(2);  

                    _Pos.Data.Type = type;
                    //Обрабатываем сообщения с типом 0
                    if (change)
                    {
                        Trader.tPositions.Change(_Pos, false);// !MManager.EventPackages);
                    }

                    if (_Pos.Account != null) return _Pos;
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        public Portfolio GetPortfoliosFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 5)
                {
                    Portfolio porFind = null;
                    // 0 OnPortfolioInfo | 1 firmid | 2 client_code | 3 limit_kind | 4 portfolio_value | 
                    // 5 is_marginal | 6 total_money_bal | 7 fut_total_asset | 8 fundslevel | 
                    // 9 curr_tag | 10 current_bal | 11 client_type | 12 all_assets | 
                    // 13 rate_change | 14 in_assets | 15 in_all_assets | 16 is_leverage | 
                    // 17 profit_loss | 18 assets | 19 rate_futures | 20 lim_non_margin
                    var Acc = Trader.Accounts.FirstOrDefault(a => a.Firm.Id == Msg[1]);
                    if (Acc == null) Acc = Trader.Accounts.FirstOrDefault(a => a.Firm.Id == Msg[1]);
                    var client = Trader.Clients.FirstOrDefault(c => c.Code.Contains(Msg[2]));
                    var limitKind = Msg[3].ToInt32();
                    if (Acc != null)
                    {
                        porFind = Trader.Portfolios.FirstOrDefault(p => p.Account.AccID == Acc.AccID
                            && p.Account.Firm.Id == Acc.Firm.Id && p.Client.Code == client.Code && p.LimitKind == limitKind);
                    }
                    bool change = false;
                    if (porFind == null)
                    {
                        porFind = new Portfolio();
                        porFind.Account = Acc;
                        porFind.Client = client;
                        porFind.LimitKind = limitKind;
                        Trader.tPortfolios.Add(porFind);
                    }
                    else change = true;

                    porFind.Balance =           Msg[4].ToDecimal(2);
                    porFind.CurrentBalance =    Msg[10].ToDecimal(2);
                    porFind.LastPositionBalance = Msg[15].ToDecimal(2);
                    porFind.PositionBalance =   porFind.Balance - porFind.CurrentBalance;//Math.Round(Convert.ToDecimal(Msg[12].Replace('.', ',')), 2);
                    porFind.VarMargin =         Msg[17].ToDecimal(2); 
                    porFind.RateChange =        Msg[13].ToDecimal();
                    //porFind.Tag = Msg[13];

                    if (change)
                    {
                        Trader.tPortfolios.Change(porFind, false);// !MManager.EventPackages);
                    }
                    if (porFind.Account != null) return porFind;

                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + elStack.Message);
            }
            return null;
        }

        /// <summary>
        /// Получение портфеля и его изменений из сообщения
        /// </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        public Portfolio GetMoneyLimitsFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    Portfolio porFind = null;
                    // 0 OnMoneyLimits | 1 leverage | 2 currentbal | 3 limit_kind | 4 client_code | 5 openlimit | 6 firmid | 
                    // 7 locked_margin_value | 8 currcode | 9 openbal | 10 locked | 11 locked_value_coef | 12 currentlimit | 13 tag
                    var Acc = Trader.Accounts.FirstOrDefault(a => a.Firm.Id == Msg[6]);
                    if (Acc == null) Acc = Trader.Accounts.FirstOrDefault(a => a.Firm.Id == Msg[6]);
                    var client = Trader.Clients.FirstOrDefault(c => c.Code.Contains(Msg[4]));
                    if (Acc != null)
                    {
                        porFind = Trader.Portfolios.FirstOrDefault(p => p.Account.AccID == Acc.AccID
                            && p.Account.Firm.Id == Acc.Firm.Id && p.Client.Code == client.Code);
                    }
                    bool change = false;
                    if (porFind == null)
                    {
                        porFind = new Portfolio();
                        porFind.Account = Acc;
                        porFind.Client = client;

                        Trader.tPortfolios.Add(porFind);
                    }
                    else change = true;

                    //porFind.CurrentBalance = Convert.ToDecimal(Msg[9].Replace('.', ','));
                    //porFind.PositionBalance = Convert.ToDecimal(Msg[2].Replace('.', ','));
                    //porFind.VarMargin = Convert.ToDecimal(Msg[5].Replace('.', ','));
                    porFind.Tag = Msg[13];

                    if (change)
                    {
                        Trader.tPortfolios.Change(porFind, false);// !MManager.EventPackages);
                    }
                    if (porFind.Account != null) return porFind;

                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + elStack.Message);
            }
            return null;
        }
        /// <summary>
        /// Получение портфеля и его изменений из сообщения
        /// </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        public Portfolio GetFutLimitsFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    Portfolio porFind = null;
                    // 0 OnFuturesLimits | 1 cbplused | 2 cbp_prev_limit | 3 varmargin | 4 options_premium | 5 limit_type | 
                    // 6 firmid | 7 currcode | 8 cbplused_for_orders | 9 liquidity_coef | 10 real_varmargin | 
                    // 11 cbplused_for_positions | 12 accruedint | 13 kgo | 14 ts_comission | 15 cbplplanned | 16 trdaccid | 17 cbplimit
                    int LimitKind = Msg[5].ToInt32();// Convert.ToDecimal(Msg[5]);
                    var Acc = Trader.Accounts.FirstOrDefault(a => a.AccID == Msg[16] && a.Firm.Id == Msg[6]);
                    if (Acc == null) Acc = Trader.Accounts.FirstOrDefault(a => a.Firm.Id == Msg[6]);
                    if (Acc != null)
                    {
                        porFind = Trader.Portfolios.FirstOrDefault(p => p.Account.AccID == Acc.AccID
                            && p.Account.Firm.Id == Acc.Firm.Id && p.LimitKind == LimitKind);
                    }
                    bool change = false;
                    if (porFind == null)
                    {
                        porFind = new Portfolio();
                        porFind.Account = Acc;
                        porFind.Client = Trader.Clients.FirstOrDefault(c => c.Code == Acc.AccID);
                        Trader.tPortfolios.Add(porFind);
                    }
                    else change = true;

                    porFind.LimitKind =         LimitKind;
                    porFind.PrevBalance =       Msg[2].ToDecimal(2);
                    porFind.CurrentBalance =    Msg[17].ToDecimal(2);
                    porFind.Balance =           Msg[15].ToDecimal(2);
                    porFind.VarMargin =         Msg[3].ToDecimal(2);
                    porFind.PositionBalance =   Msg[1].ToDecimal(2);
                    porFind.Commission =        Msg[14].ToDecimal(0);
                    porFind.RealMargin =        Msg[10].ToDecimal(0);

                    if (change)
                    {
                        Trader.tPortfolios.Change(porFind, false);// !MManager.EventPackages);
                    }
                    if (porFind.Account != null) return porFind;

                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        /// <summary> Получение стакана из сообщения </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        public Quote GetQuoteFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    Quote _Quote = new Quote();
                    // 0 OnQuote | 1 QJSIM | 2 MGNT | 3 10.000000 | 4 10.000000 | 
                    // 9288.000000 | 16 | 9289.000000 | 17 | 9290.000000 | 5 | 9292.000000 | 11 | 
                    // 9295.000000 | 56 | 9300.000000 | 1 | 9301.000000 | 1 | 9306.000000 | 1 | 
                    // 9311.000000 | 36 | 9325.000000 | 22 | 9331.000000 | 3 | 9333.000000 | 35 | 
                    // 9348.000000 | 57 | 9349.000000 | 6 | 9350.000000 | 20 | 9351.000000 | 50 | 
                    // 9352.000000 | 27 | 9353.000000 | 6 | 9356.000000 | 1 | 9366.000000 | 20

                    _Quote.Sec = this.FindSecurities(Msg[2], Msg[1]);
                    int CountBid = Msg[3].ToInt32();// Convert.ToDecimal(Msg[3].Replace('.', ','));
                    int CountAsk = Msg[4].ToInt32();// Convert.ToDecimal(.Replace('.', ','));

                    List<Quote.QuoteRow> listBid = new List<Quote.QuoteRow>();
                    List<Quote.QuoteRow> listAsk = new List<Quote.QuoteRow>();
                    int indexStartDepth = 5;

                    int CounterRowBid = 0; //Счетчик строк для bid
                    int CounterRowAsk = 0; //Счетчик строк для ask

                    bool BidAfterAsk = true;

                    for (int i = indexStartDepth; i < Msg.Length; i++)
                    {
                        decimal price = Msg[i].ToDecimal(_Quote.Sec.Scale);
                        int vol = Msg[i + 1].ToInt32();
                        i = i + 1;
                        Quote.QuoteRow row = new Quote.QuoteRow()
                        {
                            Price = price,
                            Volume = vol
                        };

                        //BID
                        if (BidAfterAsk)
                        {
                            listBid.Add(row);
                            CounterRowBid++;
                            if (CountBid <= CounterRowBid) BidAfterAsk = !BidAfterAsk;
                        }
                        //ASK
                        else
                        {
                            listAsk.Add(row);
                            CounterRowAsk++;
                            if (CountAsk <= CounterRowAsk) BidAfterAsk = !BidAfterAsk;
                        }
                    }

                    _Quote.Bid = listBid.ToArray();
                    _Quote.Ask = listAsk.ToArray();

                    Trader.tQuote.Change(_Quote);//.ChangeQoute(_Quote, true);
                    return _Quote;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }
        /// <summary> Обработчик сообщений с транзакциями </summary>
        /// <param name="tmpM"></param>
        /// <param name="Msg"></param>

        /// <returns></returns>
        public TransReply GetTransReplyFromArrayMsg(StackMsg elStack, string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    TransReply trans = new TransReply();
                    // 0 OnTransReply | 1 account | 2 firm_id | 3 order_num, 4 trans_id, 5 price , 
                    // 6 quantity, 7 client_code , 8 balance , 9 time , 10 status , 11 exchange_code , 
                    // 12 date_time.week_day | 13 date_time.hour | 14 date_time.ms | 15 date_time.mcs | 16 date_time.day | 
                    // 17 date_time.month | 18 date_time.sec | 19 date_time.year | 20 date_time.min , 21 uid , 
                    // 22 result_msg , 23 brokerref , 24 server_trans_id , 25 flags

                    trans.Account = Trader.Accounts.FirstOrDefault(a => a.AccID == Msg[1]);
                    trans.Firm = this.FindFirm(Msg[2]);

                    if (trans.Account == null && trans.Firm == null) return null;

                    trans.OrderNumber =     Msg[3].ToLong();
                    trans.TransID =         Msg[4].ToLong();
                    trans.Price =           Msg[5].ToDecimal(); 
                    trans.Volume =          Msg[6].ToInt32(); 
                    trans.Balance =         Msg[8].ToDecimal();

                    trans.Client = Trader.Clients.FirstOrDefault(c => c.Code.Contains(Msg[7]));
                    trans.Status = Msg[10].ToInt32();

                    Common.DateMarket date = new Common.DateMarket();
                    date.day = Msg[16];
                    date.month = Msg[17];
                    date.year = Msg[19];

                    date.hour = Msg[13];
                    date.min = Msg[20];
                    date.sec = Msg[18];
                    date.mcs = Msg[17];
                    trans.DateTrans = Convert.ToDateTime(date.DateTime);

                    trans.uid = Msg[21].ToDecimal();
                    trans.ResultMsg = Msg[22];
                    trans.Comment = Msg[23];

                    trans.ServerTransID = Msg[24].ToLong();

                    Trader.tTransaction.NewTransReply(trans, true);
                    
                    return trans;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }
    }
}


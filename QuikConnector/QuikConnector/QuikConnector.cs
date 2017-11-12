using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using QuikControl;
using MarketObject;
using ServiceMessage;

namespace Connector
{
    public class QuikConnector
    {
        /// <summary> Контролер терминала </summary>
        private QControlTerminal ConTerminal;
        /// <summary>
        /// Параметры и объекты терминала
        /// </summary>
        public QControlTerminal Objects
        {
            get
            {
                return this.ConTerminal;
            }
        }

        /// <summary>
        /// Создает объект для подключения к терминалу.
        /// </summary>
        /// <param name="serverAddr">Адрес сервера</param>
        /// <param name="port">Порт сервера</param>
        public QuikConnector(string serverAddr = "localhost", int port = 8080)
        {
            this.ConTerminal = new QControlTerminal(serverAddr, port);

            //Read config
            this.GetConfig();
        }

        /// <summary> Получает данные из конфиг. файла</summary>
        private void GetConfig()
        {
            //Считываем конфигурационный файл
            var Config = new IniFile("conf.ini");
            var ClassForStock = Config.Read("MarketClass", "SettingForStock");
            ConvertorMsg.CodesClassForStock = ClassForStock;
        }

        /// <summary> Подключиться к терминалу </summary>
        public void Connect()
        {
            this.ConTerminal.CreateSockets();
        }

        /// <summary>
        /// Разорвать соединение 
        /// </summary>
        public void Disconnect()
        {
            this.ConTerminal.CloseSockets();
        }



        /// <summary> Регистрирует инструмент для получения данных о нем. </summary>
        public void RegisterSecurities(string SecCode, string SecClassCode)
        {
            this.ConTerminal.SendMsgToServer("RegSec", SecCode + MManager.SpliterData + SecClassCode);
        }
        /// <summary> Регистрирует инструмент для получения данных о нем. </summary>
        public void RegisterSecurities(Securities sec)
        {
            this.ConTerminal.SendMsgToServer("RegSec", sec.Code + MManager.SpliterData + sec.Class.Code);
        }
        /// <summary> Регистрирует стакан инструмента для получения данных стакана. </summary>
        public void RegisterDepth(string SecCode, string SecClassCode)
        {
            this.ConTerminal.SendMsgToServer("RegDepthSec", SecCode + MManager.SpliterData + SecClassCode);
        }
        public void RegisterDepth(Securities sec)
        {
            if (!sec.Empty())
                this.ConTerminal.SendMsgToServer("RegDepthSec", sec.Code + MManager.SpliterData + sec.Class.Code);
        }
        /// <summary> Снять регистрацию стакана инструмента для отмены получения данных стакана. </summary>
        public void UnregisterDepth(string SecCode, string SecClassCode)
        {
            this.ConTerminal.SendMsgToServer("UnRegDepthSec", SecCode + MManager.SpliterData + SecClassCode);
        }
        /// <summary> Снять регистрацию стакана инструмента для отмены получения данных стакана. </summary>
        public void UnregisterDepth(Securities sec)
        {
            if (!sec.Empty())
                this.ConTerminal.SendMsgToServer("UnRegDepthSec", sec.Code + MManager.SpliterData + sec.Class.Code);
        }

        /// <summary> Регистрирует параметры для инструментов которые будут обновляться. </summary>
        public void RegisterParamSec(string[] arrayParams)
        {
            if (arrayParams.Length > 0)
            {
                arrayParams.ToList().ForEach(el =>
                {
                    this.RegisterParamSec(el);
                });
            }
        }
        /// <summary> Регистрирует все доступные параметры для инструментов которые будут обновляться. </summary>
        public void RegisterAllParamSec()
        {
            if (Securities.ListAllParams.Length > 0)
            {
                string p = "";
                Securities.ListAllParams.ToList().ForEach(el =>
                {
                    p += el + MManager.SpliterData;
                });
                this.RegisterParamSec(p);
            }
        }
        /// <summary> Регистрирует один указанный параметр для инструментов которые будут обновляться. </summary>
        public void RegisterParamSec(string Param)
        {
            this.ConTerminal.SendMsgToServer("RegParamsSec", Param);
        }

        /// <summary>
        /// Отправка транзакции
        /// </summary>
        /// <param name="ArrayParams"> Параметры необходимые для транзакции </param>
        /// <returns></returns>
        private int SendTransaction(string[] ArrayParams)
        {
            int count = ArrayParams.Length;
            if (count == 0) return -1;
            string msg =  count.ToString();
            foreach (var param in ArrayParams)
            {
                msg += MManager.SpliterData + param;
            }
            this.ConTerminal.SendMsgToServer("Transaction", msg);
            return 0;
        }
        /// <summary>
        /// Создает заявку.
        /// </summary>
        /// <param name="createOrder"></param>
        /// <returns></returns>
        public int CreateOrder(Order createOrder)
        {
            if (createOrder.Sec.Empty()) return -1;
            if (createOrder.Price <= 0) return -2;
            if (createOrder.Volume <= 0) return -3;
            Common.Ext.NewThread(() =>
            {
                Qlog.CatchException(() =>
                {
                    if (!this.ConTerminal.Accounts.Empty())
                    {
                        Account acc = this.ConTerminal.Accounts.FirstOrDefault(a => !a.AccClasses.FirstOrDefault(c => c.Code == createOrder.Sec.Class.Code).Empty());
                        if (acc.Empty()) return;
                        Random rnd = new Random();

                        string[] Params = {
                            "TRANS_ID",     rnd.Next(1, 1000000).ToString(),
                            "ACTION",       "NEW_ORDER",
                            "CLASSCODE",    createOrder.Sec.Class.Code,
                            "SECCODE",      createOrder.Sec.Code,
                            "OPERATION",    createOrder.Direction == OrderDirection.Buy ? "B" : "S",
                            "TYPE",         "L",
                            "PRICE",        createOrder.Price.ToString(),
                            "QUANTITY",     createOrder.Volume.ToString(),
                            "ACCOUNT",      acc.AccID
                        };
                        this.SendTransaction(Params);
                    }
                });
            });
            return 0;
        }

        /// <summary>
        /// Создает стоп-заявку.
        /// </summary>
        /// <param name="createOrder"></param>
        /// <returns></returns>
        public int CreateStopOrder(StopOrder createOrder, StopOrderType type)
        {
            if (createOrder.Sec.Empty()) return -1;
            if (createOrder.Price <= 0) return -2;
            if (createOrder.Volume <= 0) return -3;
            Common.Ext.NewThread(() =>
            {
                Qlog.CatchException(() =>
                {
                    Account acc = this.ConTerminal.Accounts.FirstOrDefault(a => a.AccClasses.FirstOrDefault(c => c.Code == createOrder.Sec.Class.Code) != null);
                    if (acc.Empty()) return;
                    Random rnd = new Random();
                    string TypeSOrder = "";
                    string[] DopParam = { "TRANS_ID", rnd.Next(1, 1000000).ToString() };

                    string condition = ((int)ConditionStopOrder.MoreOrEqual).ToString();

                    switch (type)
                    {
                        case StopOrderType.StopLimit:
                            TypeSOrder = "SIMPLE_STOP_ORDER";
                            if (createOrder.Direction == OrderDirection.Buy) condition = ((int)ConditionStopOrder.MoreOrEqual).ToString();
                            else condition = ((int)ConditionStopOrder.LessOrEqual).ToString();
                            var dateExpiry = createOrder.DateExpiry.ToString_YYYYMMDD();
                            DopParam = DopParam.Concat(new string[] {
                                "CONDITION", condition,
                                "EXPIRY_DATE", (dateExpiry.Empty() ? "TODAY" : dateExpiry),
                                "STOPPRICE",    Math.Round(createOrder.ConditionPrice, (int)createOrder.Sec.Scale).ToString(),
                            }).ToArray();
                            break;
                        case StopOrderType.LinkOrder:
                            TypeSOrder = "WITH_LINKED_LIMIT_ORDER";
                            if (createOrder.Direction == OrderDirection.Buy) condition = ((int)ConditionStopOrder.MoreOrEqual).ToString();
                            else condition = ((int)ConditionStopOrder.LessOrEqual).ToString();
                            DopParam = DopParam.Concat(new string[] {
                                "CONDITION",        condition,
                                "EXPIRY_DATE",      "TODAY",
                                "STOPPRICE",        Math.Round(createOrder.ConditionPrice, (int)createOrder.Sec.Scale).ToString(),
                                "LINKED_ORDER_PRICE", Math.Round(createOrder.LinkOrderPrice, (int)createOrder.Sec.Scale).ToString(),
                                "KILL_IF_LINKED_ORDER_PARTLY_FILLED","NO"
                            }).ToArray();

                            break;
                        case StopOrderType.TakeProfit:
                            TypeSOrder = "TAKE_PROFIT_STOP_ORDER";
                            if (createOrder.Direction == OrderDirection.Buy) condition = ((int)ConditionStopOrder.LessOrEqual).ToString();
                            else condition = ((int)ConditionStopOrder.MoreOrEqual).ToString();
                            DopParam = DopParam.Concat(new string[] {
                                "CONDITION", condition,
                                "EXPIRY_DATE", "TODAY",
                                "OFFSET",       Math.Round(createOrder.Offset, (int)createOrder.Sec.Scale).ToString(),
                                "OFFSET_UNITS", "PRICE_UNITS",
                                "SPREAD",       Math.Round(createOrder.Spread, (int)createOrder.Sec.Scale).ToString(),
                                "SPREAD_UNITS", "PRICE_UNITS",
                                "STOPPRICE",    Math.Round(createOrder.ConditionPrice, (int)createOrder.Sec.Scale).ToString(),
                            }).ToArray();
                            break;
                        case StopOrderType.TakeProfitStopLimit:
                            TypeSOrder = "TAKE_PROFIT_AND_STOP_LIMIT_ORDER";
                            DopParam = DopParam.Concat(new string[] {
                                "OFFSET",       Math.Round(createOrder.Offset, (int)createOrder.Sec.Scale).ToString(),
                                "OFFSET_UNITS", "PRICE_UNITS",
                                "SPREAD",       Math.Round(createOrder.Spread, (int)createOrder.Sec.Scale).ToString(),
                                "SPREAD_UNITS", "PRICE_UNITS",
                                "STOPPRICE",    Math.Round(createOrder.ConditionPrice, (int)createOrder.Sec.Scale).ToString(),
                                "STOPPRICE2",   Math.Round(createOrder.ConditionPrice2, (int)createOrder.Sec.Scale).ToString(),
                                "IS_ACTIVE_IN_TIME", "NO"
                            }).ToArray();
                            break;

                    };
                    string[] Params = {
                        "ACTION",       "NEW_STOP_ORDER",
                        "CLASSCODE",    createOrder.Sec.Class.Code,
                        "SECCODE",      createOrder.Sec.Code,
                        "OPERATION",    createOrder.Direction == OrderDirection.Buy ? "B" : "S",
                        "TYPE",         "L",
                        "PRICE",        Math.Round(createOrder.Price, (int)createOrder.Sec.Scale).ToString(),
                        "QUANTITY",     createOrder.Volume.ToString(),
                        "ACCOUNT",      acc.AccID,
                        "STOP_ORDER_KIND", TypeSOrder,
                    };
                    Params = Params.Concat(DopParam).ToArray();
                    this.SendTransaction(Params);
                });
            });
            return 0;
        }

        /// <summary>
        /// Отменяет указанную заявку.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="OrderNumber">Номер заявки</param>
        /// <returns></returns>
        public int CancelOrder(Securities sec, decimal OrderNumber)
        {
            if (sec.Empty()) return -1;
            if (OrderNumber <= 0) return -2;
            Common.Ext.NewThread(() =>
            {
                Qlog.CatchException(() =>
                {
                    Account acc = this.ConTerminal.Accounts.FirstOrDefault(a => !a.AccClasses.FirstOrDefault(c => c.Code == sec.Class.Code).Empty());
                    if (acc.Empty()) return;
                    Random rnd = new Random();
                    //"|CLASSCODE|QJSIM|SECCODE|SBER|ORDER_KEY|3181375550|ACCOUNT|NL0011100043";
                    string[] Params = {
                            "TRANS_ID",     rnd.Next(1, 1000000).ToString(),
                            "ACTION",       "KILL_ORDER",
                            "CLASSCODE",    sec.Class.Code,
                            "SECCODE",      sec.Code,
                            "ACCOUNT",      acc.AccID,
                            "ORDER_KEY",    OrderNumber.ToString()
                        };
                    this.SendTransaction(Params);
                });
            });
            return 0;
        }

        /// <summary>
        /// Отменяет указанную стоп-заявку.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="OrderNumber">Номер заявки</param>
        /// <returns></returns>
        public int CancelStopOrder(Securities sec, decimal OrderNumber)
        {
            if (sec.Empty()) return -1;
            if (OrderNumber <= 0) return -2;
            Common.Ext.NewThread(() =>
            {
                Qlog.CatchException(() =>
                {
                    Account acc = this.ConTerminal.Accounts.FirstOrDefault(a => !a.AccClasses.FirstOrDefault(c => c.Code == sec.Class.Code).Empty());
                    if (acc.Empty()) return;
                    Random rnd = new Random();
                    //"|CLASSCODE|QJSIM|SECCODE|SBER|ORDER_KEY|3181375550|ACCOUNT|NL0011100043";
                    string[] Params = {
                        "TRANS_ID",     rnd.Next(1, 1000000).ToString(),
                        "ACTION",       "KILL_STOP_ORDER",
                        "CLASSCODE",    sec.Class.Code,
                        "SECCODE",      sec.Code,
                        "ACCOUNT",      acc.AccID,
                        "STOP_ORDER_KEY",    OrderNumber.ToString()
                    };
                    this.SendTransaction(Params);
                });
            });
            return -1;
        }
        /// <summary>
        /// Снимает все заявки по инструменту
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int CancelAllOrder(Securities sec)
        {
            if (sec.Empty()) return -1;
            Qlog.CatchException(() =>
            {
                IEnumerable<Order> orders = this.ConTerminal.Orders.Where(o => o.Sec.Code == sec.Code
                    && o.Sec.Class.Code == sec.Class.Code && o.Status == OrderStatus.ACTIVE);
                if (!orders.Empty())
                {
                    foreach (var ord in orders)
                    {
                        this.CancelOrder(sec, ord.OrderNumber);
                    }
                }
            });
            return 0;
        }

        /// <summary>
        /// Снимает все стоп-заявки по инструменту
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public int CancelAllStopOrder(Securities sec)
        {
            if (sec.Empty()) return -1;
            Qlog.CatchException(() =>
            {
                IEnumerable<Order> orders = this.ConTerminal.StopOrders.Where(o => o.Sec.Code == sec.Code
                    && o.Sec.Class.Code == sec.Class.Code && o.Status == OrderStatus.ACTIVE);
                if (!orders.Empty())
                {
                    foreach (var ord in orders)
                    {
                        this.CancelStopOrder(sec, ord.OrderNumber);
                    }
                }
            });
            return 0;
        }
    }
}

using ServiceMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketObject;
using QLuaApp;
using System.Text.RegularExpressions;

namespace QuikControl
{
    public class QControlTerminal : MarketTools
    {

        /// <summary> Флаг подключения к серверной части (к скрипту LUA) </summary>
        public bool isConnected = false;

        /// <summary> Последнее сообщение полученное от терминала. </summary>
        public string LastMessage = "";
        /// <summary> Последняя полученная сделка </summary>
        public Trade LastTrade = null;

        /// <summary> Настройки для подключения.  </summary>
        private QLuaAppServer Server = new QLuaAppServer();


        /// <summary> Информация по торговому терминалу  </summary>
        public MarketTerminal Terminal = new MarketTerminal();

        /// <summary> Менеджер сообщений полученных из терминала Quik </summary>
        private MManager MsgManager = null;
        /// <summary> Менеджер сообщений полученных из терминала Quik (СДЕЛКИ) </summary>
        private MManager MsgManTraders = null;
        /// <summary> Менеджер сообщений полученных из терминала Quik (РАЗЛИЧНЫЕ РЫНОЧНЫЕ ДАННЫЕ) </summary>
        private MManager MsgManMarket = null;


        //Для отладки
        private delegate void AllEvent(string Str);
        //private event AllEvent OnAllEvent;
        /// <summary>  Событие возникновения команд от сервера </summary>
        private event AllEvent OnAnswerServer;

        /// <summary> Контролер терминала. </summary>
        /// <param name="serverAddr">Адрес подключения к серверу. </param>
        /// <param name="port">Порт подключения</param>
        public QControlTerminal(string serverAddr, int port)
        {
            MsgManager = new MManager(this);
            MsgManTraders = new MManager(this);
            MsgManMarket = new MManager(this);

            Server.ServerAddr = serverAddr;
            Server.Port = port;
        }

        /// <summary>
        /// Создает сокеты для подключения
        /// </summary>
        public void CreateSockets()
        {
            Qlog.CatchException(() =>
            {
                //if (MsgManager.ActiveStatus) return;
                MsgManager.Type = 1;
                MsgManTraders.Type = 2;
                MsgManMarket.Type = 3;

                MsgManager.OnNewSysMessage += new MManager.eventNewMessage(Event_OnNewSysMessage);

                this.OnAnswerServer += (command) => { };

                //Конект до сокета сервера
                if (MsgManager.ConnectSocket(Server.ServerAddr, Server.Port, true) == 0)
                {
                    //Активатор отложенных событий
                    MsgManager.AcivateAllEvent += () =>
                    {
                        if (MarketTools.ListAllDeferredEventBase.Count > 0)
                        {
                            foreach (var act in MarketTools.ListAllDeferredEventBase)
                            {
                                act.ExecEvent();
                            }
                        }
                    };
                    //Инициализация контроллекра сообщений
                    MsgManager.InitThreadsMessages();

                    this.isConnected = true;
                    MsgManager.Convertor.OnStartMarket += () =>
                    {
                        //MsgManTraders.PortionInTime = 1000;
                        if (MsgManTraders.ConnectSocket(Server.ServerAddr, Server.Port) == 0)
                        {
                            //Активатор отложенных событий
                            MsgManTraders.AcivateAllEvent += () =>
                            {
                                if (MarketTools.ListAllDeferredEventTrades.Count > 0)
                                {
                                    foreach (var act in MarketTools.ListAllDeferredEventTrades)
                                    {
                                        act.ExecEvent();
                                    }
                                }
                            };
                            MsgManTraders.InitThreadsMessages();
                        }
                        if (MsgManMarket.ConnectSocket(Server.ServerAddr, Server.Port) == 0)
                        {
                            MsgManTraders.AcivateAllEvent += () =>
                            {
                                if (MarketTools.ListAllDeferredEventMarkets.Count > 0)
                                {
                                    foreach (var act in MarketTools.ListAllDeferredEventMarkets)
                                    {
                                        act.ExecEvent();
                                    }
                                }
                            };
                            MsgManMarket.InitThreadsMessages();
                        }
                    };
                }
                else
                {
                    this.isConnected = false;
                }
            });
        }
        /// <summary>
        /// Отправка сообщения на сервер
        /// </summary>
        /// <param name="header">Заголовок сообщения</param>
        /// <param name="msg">Сообщение</param>
        public void SendMsgToServer(string header, string msg)
        {
            if(!msg.Empty())
                this.MsgManager.Send(header, msg);
        }

        /// <summary> Событие системного сообщения </summary>
        /// <param name="MsgObject">Объект менеджера сообщений </param>
        /// <param name="message">Строковое сообщение</param>
        private void Event_OnNewSysMessage(MManager MsgObject, string message)
        {
            if (message == "" || message.Empty()) return;

            Regex reg = new Regex(@"^ServerCommand:", RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(message);
            if (mc.Count > 0)
            {
                string command = message.Replace("ServerCommand:", "");
                if (!OnAnswerServer.Empty())
                    OnAnswerServer(command);
            }
        }

        /// <summary> Завершает соединение сокета с терминалом</summary>
        public void CloseSockets()
        {
            this.isConnected = false;
            MsgManager.OnNewSysMessage -= new MManager.eventNewMessage(Event_OnNewSysMessage);

            MsgManMarket.Close();
            MsgManTraders.Close();
            MsgManager.Close();
        }
    }
}

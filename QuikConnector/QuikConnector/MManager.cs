using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Common;
using QuikControl;

namespace ServiceMessage
{

    /// <summary> Класс стека сообщений. </summary>
	public class StackMessages
    {
        /// <summary> Mutex lock stack </summary>
        private Mutex mutexLock = new Mutex();
        /// <summary> Список сообщений. </summary>
        private List<string> Stack = new List<string>();
        /// <summary> Кол-во сообщений в стеке. </summary>
        public int Count { get { return this.Stack.Count; } }
        /// <summary> Последее добавленное сообщение </summary>
        public string Last = null;

        /// <summary> Получает список хранящих в стеке данных </summary>
        /*public IEnumerable<string> DataStack
        {
            get
            {
                return this.Stack;
            }
        }*/
        /// <summary> Добавить сообщение в стек. </summary>
        public void Add(string msg)
        {
            Qlog.CatchException(() =>
            {
                mutexLock.WaitOne();
                this.Stack.Add(msg);
                this.Last = msg;
                mutexLock.ReleaseMutex();
            });
        }
        /// <summary> Добавить сообщение в стек. </summary>
        public string getFirst
        {
            get
            {
                string msg = null;
                mutexLock.WaitOne();
                if (this.Count > 0)
                    msg = this.Stack.ElementAt(0);
                else this.Last = null;
                mutexLock.ReleaseMutex();
                return msg;
            }
        }
        /// <summary> Удаляет первый элемент из стека </summary>
        public void DeleteFirst()
        {
            Qlog.CatchException(() =>
            {
                mutexLock.WaitOne();
                if (this.Stack.Count > 0)
                    this.Stack.RemoveAt(0);
                if (this.Count <= 0) this.Last = null;
                mutexLock.ReleaseMutex();
            });
        }
        /// <summary> Очищает стек сообщений </summary>
        public void Clear()
        {
            Qlog.CatchException(() =>
            {
                mutexLock.WaitOne();
                if (this.Stack.Count > 0)
                    this.Stack.Clear();
                if (this.Count <= 0) this.Last = null;
                mutexLock.ReleaseMutex();
            });
        }
    }

    /// <summary> Менеджер сообщений </summary>
    public class MManager
    {
        /// <summary> Разделитель сообщений в одной посылке </summary>
        public static char SpliterMsg = '\t';
        /// <summary> Разделитель данных в сообщении </summary>
        public static char SpliterData = '|';

        /// <summary> Размер принимаемого сообщения от сервера. </summary>
        private const int SizeMessage = 100000;
        /// <summary> Сокет. </summary>
        private QSocket qSocket = new QSocket(SizeMessage);

        /// <summary> Стек полученных приоритетных сообщений </summary>
        private StackMessages MsgPriority = new StackMessages();
        /// <summary> Стек полученных сообщений </summary>
        private StackMessages Msg = new StackMessages();

        /// <summary> Стек полученных системных сообщений </summary>
        private static StackMessages MsgSys = new StackMessages();
        /// <summary> Стек сообщений на отправку </summary>
        private static StackMessages MsgSend = new StackMessages();

        /// <summary> Флаг работы основного цикла. </summary>
        public static bool LoopProcessing = true;
        /// <summary> Неполная часть предыдущего сообщения. Добавляется в начало следующего сообщения. </summary>
        private string LastMessagePart = "";  //Неполная часть от последнего сообщения

        /// <summary> Тип для идентификации объекта MMessage</summary>
        public int Type = 0;

        /// <summary> Время в sec за которое выгружается порция накопленных событий. </summary>
        //public int PortionInTime = 0;

        /// <summary> Делегат нового сообщения. </summary>
        /// <param name="MsgObject">Объект менеджера сообщений.</param>
        /// <param name="message">Текстовое сообщение.</param>
        public delegate void eventNewMessage(MManager MsgObject, string message);
        /// <summary>  Обработчик нового системного сообщения. </summary>
        public event eventNewMessage OnNewSysMessage;

        /// <summary> Поток получения сообщений. </summary>
        private Thread MainThreadLoop = null;
        /// <summary> Статус активности приема сообщений. </summary>
        public bool ActiveStatus { get { return this.MainThreadLoop != null ? true : false; } }

        /// <summary> Флаг что сообщение отправленно и ждет подтверждения о принятии. </summary>
        private bool FlagSendMsg = false;

        /// <summary> Объект конвертора </summary>
        public ConvertorMsg Convertor = null;

        /// <summary> Конструктор объекста Менеджер сообщений </summary>
        /// <param name="trader"></param>
        public MManager(QuikControl.QControlTerminal trader)
        {
            if (!trader.Empty())
                Convertor = new ConvertorMsg(trader);
        }

        /// <summary> Добавить сообщение на отправку серверу  </summary>
        /// <param name="Type">Тип сообщения или заголовок.</param>
        /// <param name="msgSend">Тело сообщения(само сообщение)</param>
        public void Send(string Type, string msgSend)
        {
            if (!msgSend.Empty())
                MManager.MsgSend.Add(Type + MManager.SpliterData + msgSend);
        }
        /// <summary> Добавить сообщение на отправку серверу </summary>
        /// <param name="msgSend"></param>
        public void Send(string msgSend)
        {
            if (!msgSend.Empty())
                MManager.MsgSend.Add(msgSend);
        }
        /// <summary> Добавить сообщение на отправку серверу с проверкой предыдущей, если совпадает то не добавляет. </summary>
        /// <param name="msgSend"> Отправляемое сообщение </param>
        public void SendCheckLast(string msgSend)
        {
            if (!msgSend.Empty())
                if (msgSend != MManager.MsgSend.Last)
                    MManager.MsgSend.Add(msgSend);
        }


        /// <summary> Функция осуществляет подключение к скрипту LUA, запущенный в терминале. </summary>
        /// <param name="ServerAddr">Адрес подключения (по умолчанию localhost)</param>
        /// <param name="port">Порт подключения (по умолчанию 8080)</param>
        /// <param name="noBaseSocket">Флаг определяющий базовый сокет или нет</param>
        /// <returns></returns>
        public int ConnectSocket(string ServerAddr, int port, bool noBaseSocket = false)
        {
            if (this.MainThreadLoop != null) return -1;

            qSocket.OnReceive += new QSocket._Receive(GetDataFromSocket);
            if (qSocket.CreateSocket(ServerAddr, port, MManager.SizeMessage) == 0)
            {
                //Запускаем слушатель сокета
                MManager.MsgSend.Add("StartTrader|1");
                if (!noBaseSocket)
                {
                    //Цикл отправки первоочередных сообщений
                    while (MManager.LoopProcessing)
                    {
                        this.qSocket.Receive(this);
                        this.ProcessSendMessage();
                        //Обработка полученных системных сообщений
                        ProcessSysMessage();
                        if (MManager.MsgSend.Count == 0 && !this.FlagSendMsg) break;
                        //Обработка полученных сообщений
                        Thread.Sleep(1);
                    }
                }
                return 0;
            }
            return -1;
        }

        /// <summary> Контроллер сообщений, управляет входящими и исходящими сообщениями. </summary>
        public void InitControllerMessages()
        {
            Thread ThreadSendMsg = null;
            //Основной цикл приема сообщений
            ParameterizedThreadStart actionSendMessage = (object classMM) =>
            {
                MManager mm = (MManager)classMM;
                while (MManager.LoopProcessing)
                {
                    //Отправка
                    mm.ProcessSendMessage();
                    Thread.Sleep(1);
                }
                mm.qSocket.CloseSocket();
            };
            ThreadSendMsg = new Thread(actionSendMessage);
            ThreadSendMsg.Priority = ThreadPriority.Highest;
            ThreadSendMsg.Start(this);

            Thread MainThreadProcMsg = null;
            //Основной цикл приема сообщений
            ParameterizedThreadStart actionMessage = (object classMM) =>
            {
                MManager mm = (MManager)classMM;
                while (MManager.LoopProcessing)
                {
                    //Получает данные со стека
                    mm.qSocket.Receive(mm);
                    //Обработка полученных системных сообщений
                    mm.ProcessSysMessage();
                    //Обработка полученных сообщений
                    mm.ProcessMessage();
                    Thread.Sleep(1);
                }
                mm.qSocket.CloseSocket();
            };
            MainThreadProcMsg = new Thread(actionMessage);
            MainThreadProcMsg.Priority = ThreadPriority.Normal;
            MainThreadProcMsg.Start(this);


        }

        /// <summary> Функция посылает сообщение на прекращение отправки сообщений сервером, до получения сообщения продолжить. </summary>
        private void StopGettingData()
        {
            this.SendCheckLast("Stop" + MManager.SpliterData + "1");
        }

        /// <summary> Функция посылает сообщение на возобновление отправки данных сервером. </summary>
        private void ContinueGettingData()
        {
            this.SendCheckLast("Continue" + MManager.SpliterData + "1");
        }

        /// <summary>  Обработчик получения данных из сокета  </summary>
        /// <param name="byteRecv">Кол-во байт принятых.</param>
        /// <param name="recvData">Принятые данные</param>
        /// <returns></returns>
        private int GetDataFromSocket(object baseObj, int byteRecv, byte[] recvData)
        {
            if (byteRecv == 0) return 0;
            MManager mm = (MManager)baseObj;

            //Дополняем сообщение предыдущей частью
            string content = mm.LastMessagePart + Encoding.GetEncoding(1251).GetString(recvData, 0, byteRecv);
            mm.LastMessagePart = "";

            bool AttachLastMsg = false;
            if (content[content.Length - 1] != MManager.SpliterMsg) AttachLastMsg = true;
            else content = content.Remove(content.Length - 1, 1);

            if (content.Contains("ServerCommand") || AttachLastMsg)
            {
                string[] arMessage = content.Split(MManager.SpliterMsg);
                if (arMessage.Length > 0)
                {
                    if (AttachLastMsg)
                    {
                        mm.LastMessagePart = arMessage[arMessage.Length - 1];
                        arMessage[arMessage.Length - 1] = "";
                    }
                    string msg = "", msgPr = "";
                    for (int i = 0; i < (AttachLastMsg ? arMessage.Length - 1 : arMessage.Length); i++)
                    {
                        if (arMessage[i] == "") continue;
                        if (arMessage[i].Contains("ServerCommand"))
                        {
                            MManager.MsgSys.Add(arMessage[i]);
                        }
                        else
                        {
                            if (!arMessage[i].Contains("OnAllTradesOld"))
                            {
                                msgPr += arMessage[i] + MManager.SpliterMsg;
                            }
                            else
                                msg += arMessage[i] + MManager.SpliterMsg;
                            //this.Convertor.NewMessage(this, new StackMsg(0, arMessage[i]));
                        }
                    }
                    if (!msgPr.Empty()) this.MsgPriority.Add(msgPr);
                    if (!msg.Empty()) this.Msg.Add(msg);
                }
            }
            else this.Msg.Add(content);

            return byteRecv;
        }

        public delegate void ActivatorEvents();
        /// <summary> Событие которое позволяет прогрузить события в очереди. Чтоб избежать застоя. </summary>
        public event ActivatorEvents AcivateAllEvent;

        /// <summary> Поток для события сообщений </summary>
        private Thread _threadNewEvent = null;
        private DateTime LastTimeLoadEvents;
        private int PortionInTime = 500;
        /// <summary> Функция распределения и обработки сообщений </summary>
        /// <param name="contentMsg">Поступающее сообщение</param>
        private void ProcessMessage()
        {
            //Если сообщения на отправку то пропускаем их
            if (MManager.MsgSend.Count > 0) return;

            //Выгружаем все накопившиеся события
            if (this.MsgPriority.Count == 0)
            {
                var timeVal = DateTime.Now.Ticks - LastTimeLoadEvents.AddMilliseconds(PortionInTime).Ticks;
                if (timeVal > PortionInTime)
                {
                    if (!AcivateAllEvent.Empty())
                        AcivateAllEvent();
                    LastTimeLoadEvents = DateTime.Now;
                }
            }
            //Обработка стека сообщений PRIORITY
            if (this.Msg.Count > 0 || this.MsgPriority.Count > 0)
            {
                if (this._threadNewEvent.IsNull())
                {
                    this._threadNewEvent = new Thread(eventMessage);
                    this._threadNewEvent.Priority = ThreadPriority.Normal;
                    this._threadNewEvent.Start(this);
                }
            }
        }

        /// <summary>
        /// Обработчик сообщейни из стека
        /// </summary>
        /// <param name="classMM"></param>
        private void eventMessage(object classMM)
        {
            Qlog.CatchException(() =>
            {
                MManager mmcl = (MManager)classMM;
                int count = mmcl.MsgPriority.Count;
                StackMessages activeStack = mmcl.MsgPriority;
                if (count == 0)
                {
                    activeStack = mmcl.Msg;
                    count = mmcl.Msg.Count;
                }
                if (!activeStack.Empty() && count > 0)
                {
                    string message = activeStack.getFirst;
                    activeStack.DeleteFirst();

                    string[] AllParts = message.Split(MManager.SpliterMsg);
                    if (AllParts.Length > 0)
                    {
                        for (int i = 0; i < AllParts.Length; i++)
                        {
                            string partMsg = AllParts[i];
                            if (partMsg == "") continue;
                            mmcl.Convertor.NewMessage(mmcl, new StackMsg(0, partMsg));
                        }
                    }
                    this._threadNewEvent = null;
                }
            });
        }

        /// <summary> Функция обработки системных сообщений </summary>
        /// <param name="contentMsg">Поступающее сообщение</param>
        private void ProcessSysMessage()
        {
            if (MManager.MsgSys.Count > 0)
            {
                if (OnNewSysMessage != null)
                    OnNewSysMessage(this, MManager.MsgSys.getFirst);
                MManager.MsgSys.DeleteFirst();
                this.FlagSendMsg = false;
            }
        }

        /// <summary> Функция обработки отправки сообщений </summary>
        /// <param name="contentMsg">Поступающее сообщение</param>
        private void ProcessSendMessage()
        {
            if (!this.FlagSendMsg)
            {
                if (MManager.MsgSend.Count > 0)
                {
                    this.qSocket.Send(MManager.MsgSend.getFirst);
                    MManager.MsgSend.DeleteFirst();
                    this.FlagSendMsg = true;
                }
            }
        }


        /// <summary>
        /// Закрыть соединение и прекратить передачу сообщений
        /// </summary>
        public void Close()
        {
            this.StopGettingData();
            Thread.Sleep(1000);
            MManager.LoopProcessing = false;
            if (!this.MainThreadLoop.Empty()) this.MainThreadLoop.Abort();
            this.qSocket.CloseSocket();
        }
    }
}

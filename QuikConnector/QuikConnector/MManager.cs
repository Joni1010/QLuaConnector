using System;
using System.Text;
using System.Threading;

using QuikControl;

namespace ServiceMessage
{
    /// <summary> Менеджер сообщений </summary>
    public class MManager
    {
        /// <summary> Разделитель сообщений в одной посылке </summary>
        public static char SpliterMsg = '\t';
        /// <summary> Разделитель данных в сообщении </summary>
        public static char SpliterData = '|';
        /// <summary> Флаг работы основного цикла. </summary>
        public static bool LoopProcessing = true;

        /// <summary> Размер принимаемого сообщения от сервера. </summary>
        private const int SizeMessage = 100000;


        /// <summary> Сокет. </summary>
        private QSocket qSocket = new QSocket(SizeMessage);

        /// <summary> Стек полученных сообщений </summary>
        private ServiceStackMessages Msg = new ServiceStackMessages();
        /// <summary> Стек полученных системных сообщений </summary>
        private ServiceStackMessages MsgSys = new ServiceStackMessages();
        /// <summary> Стек сообщений на отправку </summary>
        private ServiceStackMessages MsgSend = new ServiceStackMessages();

        
        /// <summary> Неполная часть предыдущего сообщения. Добавляется в начало следующего сообщения. </summary>
        private string LastMessagePart = "";  //Неполная часть от последнего сообщения

        /// <summary> Флаг определяющий базовый менеджер или нет.</summary>
        private bool BaseMManager = false;

        /// <summary> Тип для идентификации объекта MMessage</summary>
        public int Type = 0;

        /// <summary> Делегат нового сообщения. </summary>
        /// <param name="MsgObject">Объект менеджера сообщений.</param>
        /// <param name="message">Текстовое сообщение.</param>
        public delegate void eventNewMessage(MManager MsgObject, string message);
        /// <summary>  Обработчик нового системного сообщения. </summary>
        public event eventNewMessage OnNewSysMessage;

        /// <summary> Флаг что сообщение отправленно и ждет подтверждения о принятии. </summary>
        private bool FlagSendMsg = false;

        /// <summary> Объект конвертора </summary>
        public ServiceConvertorMsg Convertor = null;

        /// <summary> Конструктор объекста Менеджер сообщений </summary>
        /// <param name="trader"></param>
        public MManager(QuikControl.QControlTerminal trader)
        {
            if (!trader.Empty())
                Convertor = new ServiceConvertorMsg(trader);
            Convertor.MsgObject = this;
        }

        /// <summary> Добавить сообщение на отправку серверу  </summary>
        /// <param name="Type">Тип сообщения или заголовок.</param>
        /// <param name="msgSend">Тело сообщения(само сообщение)</param>
        public void Send(string Type, string msgSend)
        {
            if (!msgSend.Empty())
                this.MsgSend.Add(Type + MManager.SpliterData + msgSend);
        }
        /// <summary> Добавить сообщение на отправку серверу </summary>
        /// <param name="msgSend"></param>
        public void Send(string msgSend)
        {
            if (!msgSend.Empty())
                this.MsgSend.Add(msgSend);
        }
        /// <summary> Добавить сообщение на отправку серверу с проверкой предыдущей, если совпадает то не добавляет. </summary>
        /// <param name="msgSend"> Отправляемое сообщение </param>
        public void SendCheckLast(string msgSend)
        {
            if (!msgSend.Empty())
                if (msgSend != this.MsgSend.Last)
                    this.MsgSend.Add(msgSend);
        }

        /// <summary> Установка соединения. Отправка служебного сообщения. </summary>
        private void SetConnect()
        {
            //Запускаем слушатель сокета
            this.MsgSend.Add("Connect|1");
        }


        /// <summary> Функция осуществляет подключение к скрипту LUA, запущенный в терминале. </summary>
        /// <param name="ServerAddr">Адрес подключения (по умолчанию localhost)</param>
        /// <param name="port">Порт подключения (по умолчанию 8080)</param>
        /// <param name="baseSocket">Флаг определяющий базовый сокет или нет</param>
        /// <returns></returns>
        public int ConnectSocket(string ServerAddr, int port, bool baseSocket = false)
        {
            //if (this.MainThreadLoop.IsNull()) return -1;
            this.BaseMManager = baseSocket;
            qSocket.OnReceive += new QSocket._Receive(GetDataFromSocket);
            if (qSocket.CreateSocket(ServerAddr, port, MManager.SizeMessage) == 0)
            {
                this.SetConnect();
                if (this.BaseMManager)
                {
                    //Цикл отправки первоочередных сообщений
                    while (MManager.LoopProcessing)
                    {
                        this.qSocket.Receive(this);
                        this.ProcessSendMessage();
                        //Обработка полученных системных сообщений
                        ProcessSysMessage();
                        if (this.MsgSend.Count == 0 && !this.FlagSendMsg) break;
                        ProcessMessage();
                        //Обработка полученных сообщений
                        Thread.Sleep(1);
                    }
                }
                return 0;
            }
            return -1;
        }

        /// <summary> Инициализация потоков сообщений, управляет входящими и исходящими сообщениями. </summary>
        public void InitThreadsMessages()
        {
            if (this.BaseMManager)
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
            }

            Thread MainThreadProcMsg = null;
            //Основной цикл приема сообщений
            ParameterizedThreadStart actionMessage = (object classMM) =>
            {
                MManager mm = (MManager)classMM;
                while (MManager.LoopProcessing)
                {
                    //Получает данные с сокета
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

            if (this.BaseMManager)
            {
                Thread ThreadConvertor = null;
                //Основной цикл конвертора
                ParameterizedThreadStart actionThreadConvertor = (object classMM) =>
                {
                    MManager mm = (MManager)classMM;
                    while (MManager.LoopProcessing)
                    {
                        //Отправка
                        mm.Convertor.ProcessConvert();
                        Thread.Sleep(1);
                    }
                    mm.qSocket.CloseSocket();
                };
                ThreadConvertor = new Thread(actionThreadConvertor);
                ThreadConvertor.Priority = ThreadPriority.Highest;
                ThreadConvertor.Start(this);
            }
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

            int indLastSpliter = content.LastIndexOf(MManager.SpliterMsg);
            if (indLastSpliter + 1 != content.Length)
                mm.LastMessagePart = content.Substring(indLastSpliter + 1, content.Length - indLastSpliter - 1);
            content = content.Substring(0, indLastSpliter > 0 ? indLastSpliter + 1 : indLastSpliter);

            if (content.Length > 0) this.Msg.Add(content);
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
            //Выгружаем все накопившиеся события
            if (this.Msg.Count == 0)
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
            if (this.Msg.Count > 0)
            {
                if (this._threadNewEvent.IsNull())
                {
                    this._threadNewEvent = new Thread(eventMessage);
                    this._threadNewEvent.Priority = ThreadPriority.Normal;
                    this._threadNewEvent.Start(this);
                }
            }
        }

        /// <summary> Обработчик сообщейни из стека </summary>
        /// <param name="classMM"></param>
        private void eventMessage(object classMM)
        {
            if (classMM.IsNull()) return;
            Qlog.CatchException(() =>
            {
                MManager mmcl = (MManager)classMM;
                if (mmcl.Msg.Count > 0)
                {
                    string message = mmcl.Msg.getFirst;
                    mmcl.Msg.DeleteFirst();

                    string[] AllParts = message.Split(MManager.SpliterMsg);
                    if (AllParts.Length > 0)
                    {
                        for (int i = 0; i < AllParts.Length; i++)
                        {
                            if (AllParts[i] == "") continue;
                            if (AllParts[i].Contains("ServerCommand"))
                            {
                                this.MsgSys.Add(AllParts[i]);
                            }
                            else
                            {
                                mmcl.Convertor.NewMessage(new ServiceMessage(AllParts[i]));
                            }
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
            if (this.MsgSys.Count > 0)
            {
                if (OnNewSysMessage != null)
                    OnNewSysMessage(this, this.MsgSys.getFirst);
                this.MsgSys.DeleteFirst();
                this.FlagSendMsg = false;
            }
        }

        /// <summary> Функция обработки отправки сообщений </summary>
        /// <param name="contentMsg">Поступающее сообщение</param>
        private void ProcessSendMessage()
        {
            if (!this.FlagSendMsg)
            {
                if (this.MsgSend.Count > 0)
                {
                    this.qSocket.Send(this.MsgSend.getFirst);
                    this.MsgSend.DeleteFirst();
                    this.FlagSendMsg = true;
                }
            }
        }


        /// <summary> Закрыть соединение и прекратить передачу сообщений</summary>
        public void Close()
        {
            this.StopGettingData();
            Thread.Sleep(500);
            MManager.LoopProcessing = false;
            //if (!this.MainThreadLoop.Empty()) this.MainThreadLoop.Abort();
            this.qSocket.CloseSocket();
        }
    }
}

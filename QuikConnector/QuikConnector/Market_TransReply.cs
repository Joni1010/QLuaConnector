using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MarketObject
{
    /// <summary> Класс содержащий результат выполнения транзакции </summary>
    public class TransReply
    {
        //OnTransReply | price | client_code | balance | time | status | 
        // trans_id | exchange_code | date_time.week_day | date_time.hour | date_time.ms | date_time.mcs | 
        // date_time.day | date_time.month | date_time.sec | date_time.year | date_time.min | uid | flags | 
        // result_msg | brokerref | firm_id | quantity | order_num | server_trans_id | account
        /// <summary> Пользовательский ID трансакции </summary>
        public long TransID;
        /// <summary> Серверный ID трансакции </summary>
        public long ServerTransID;
        /// <summary> Счет </summary>
        public Account Account;
        /// <summary> Фирма </summary>
        public Firm Firm;
        /// <summary> Клиент </summary>
        public Client Client;
        /// <summary> Статус </summary>
        public int Status;
        /// <summary> Цена </summary>
        public decimal Price;
        /// <summary> Объем </summary>
        public int Volume;
        /// <summary> Баланс </summary>
        public decimal Balance;
        /// <summary> Номер завки </summary>
        public long OrderNumber;

        /// <summary> Сообщение результата транзакции </summary>
        public string ResultMsg;
        /// <summary> Коментарий </summary>
        public string Comment;
        /// <summary> Дата и время транзакции </summary>
        public DateTime DateTrans;
        /// <summary> Uniq ID </summary>
        public decimal uid;
    }
    public class ToolsTrans
    {
        public delegate void eventTrans(IEnumerable<TransReply> listTransReply);

        /// <summary> Событие возникновения сообщения о выполнении транзакции</summary>
        public event eventTrans OnTransReply;

        /// <summary> Список для выгрузки в событие  </summary>
        private List<TransReply> ListEvents = new List<TransReply>();
        private Mutex mutexEvent = new Mutex();

        public void NewTransReply(TransReply trReply, bool generateEvent = true)
        {
            if (OnTransReply == null) return;

            mutexEvent.WaitOne();
            this.ListEvents.Add(trReply);
            mutexEvent.ReleaseMutex();

            if (generateEvent)
                GenerateEvent();
        }


        
        protected Mutex mutexThread = new Mutex();
        /// <summary> Поток обработки измененных объектов </summary>
        private Thread ThreadEvent= null;
        /// <summary> Обработка события изменения стакана </summary>
        private void GenerateEvent()
        {
            if (this.ListEvents.Count == 0) return;

            try
            {
                mutexThread.WaitOne();
                ThreadStart eventMessage = () =>
                {
                    try
                    {
                        mutexEvent.WaitOne();
                        IEnumerable<TransReply> list = this.ListEvents.ToArray();
                        this.ListEvents.Clear();
                        mutexEvent.ReleaseMutex();

                        if (OnTransReply != null)
                            OnTransReply(list);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                };
                if (this.ThreadEvent != null && this.ThreadEvent.ThreadState == ThreadState.Running)
                    this.ThreadEvent.Join();
                this.ThreadEvent = null;
                this.ThreadEvent = new Thread(eventMessage);
                this.ThreadEvent.Priority = ThreadPriority.Normal;
                if (this.ThreadEvent != null && this.ThreadEvent.ThreadState == ThreadState.Unstarted)
                    this.ThreadEvent.Start();
                this.mutexThread.ReleaseMutex();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}

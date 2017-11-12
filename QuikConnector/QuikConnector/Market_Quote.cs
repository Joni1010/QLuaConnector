using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MarketObject
{

    
    /// <summary> Стакан </summary>
    public class Quote
    {
        /// <summary> Строка в стакане </summary>
        public class QuoteRow
        {
            /// <summary> Цена </summary>
            public decimal Price;
            /// <summary> Объем </summary>
            public int Volume;
        };
        /// <summary> Инструмент стакана </summary>
        public Securities Sec;
        /// <summary> Массив цен на покупку </summary>
        public QuoteRow[] Bid;
        /// <summary> Массив цен на продажу </summary>
        public QuoteRow[] Ask;
        public string msg;  
    }
    /// <summary> Инструменты для стакана </summary>
    /*public class ToolsQuote {
        public delegate void eventQuote(IEnumerable<Quote> listQuote);
        public event eventQuote OnQuote;

        /// <summary> Список для выгрузки в событие изменения стакана </summary>
        private List<Quote> ListChQuote = new List<Quote>();
        private Mutex mutexChQuote = new Mutex();

        public void ChangeQoute(Quote quote, bool generateEvent = true)
        {
            if (OnQuote == null) return;

            mutexChQuote.WaitOne();
            this.ListChQuote.Add(quote);
            mutexChQuote.ReleaseMutex();

            if (generateEvent)
                GenerateEventChQuote();
        }
        /// <summary> Обработка события изменения стакана </summary>
        private void GenerateEventChQuote()
        {
            if (this.ListChQuote.Count == 0) return;
            ThreadStart eventMessage = () =>
            {
                try
                {
                    mutexChQuote.WaitOne();
                    IEnumerable<Quote> list = this.ListChQuote.ToArray();
                    this.ListChQuote.Clear();
                    mutexChQuote.ReleaseMutex();

                    if (OnQuote != null)
                        OnQuote(list);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            };
            Thread ThreadEvent = new Thread(eventMessage);
            ThreadEvent.Priority = ThreadPriority.Normal;
            ThreadEvent.Start();
        }
    }*/
}

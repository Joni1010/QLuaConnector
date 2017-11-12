using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace QuikControl
{
    public class MarketEvents<T>
    {
        public delegate void eventElement(IEnumerable<T> listElem);
        /// <summary> Событие нового элемента </summary>
        public event eventElement OnNew;
        /// <summary> Событие изменения элемента </summary>
        public event eventElement OnChange;

        /// <summary> Список для нового элемента </summary>
        protected List<T> ListEventNew = new List<T>();
        protected Mutex mutexListEventNew = new Mutex();

        /// <summary> Список для измененного элемента </summary>
        protected List<T> ListEventChange = new List<T>();
        protected Mutex mutexListEventChange = new Mutex();
        /// <summary> Максимальное кол-во событий накапливаемых в списке для выгрузки</summary>
        protected int MaxElementInList = 5000;

        /// <summary> Обработчик New </summary>
        ParameterizedThreadStart eventMessageNew = (obj) =>
        {
            var thisObj = (MarketEvents<T>)obj;
            try
            {
                thisObj.mutexListEventNew.WaitOne();
                IEnumerable<T> list = thisObj.ListEventNew.ToArray();
                thisObj.ListEventNew.Clear();
                thisObj.mutexListEventNew.ReleaseMutex();

                if (thisObj != null && thisObj.OnNew != null)
                    thisObj.OnNew(list);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        };

        protected Mutex mutexThreadNew = new Mutex();
        /// <summary> Поток обработки новых объектов </summary>
        private Thread ThreadEventNew = null;
        /// <summary> Событие нового элемента</summary>
        public void GenerateEventOnNew()
        {
            try
            {
                if (this.ListEventNew.Count == 0) return;
                mutexThreadNew.WaitOne();
                if (this.ThreadEventNew != null && this.ThreadEventNew.ThreadState == ThreadState.Running)
                    ThreadEventNew.Join();
                this.ThreadEventNew = null;
                this.ThreadEventNew = new Thread(eventMessageNew);
                this.ThreadEventNew.Priority = ThreadPriority.Normal;
                if(this.ThreadEventNew != null && this.ThreadEventNew.ThreadState == ThreadState.Unstarted)
                    this.ThreadEventNew.Start(this);
                mutexThreadNew.ReleaseMutex();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /// <summary> Обработчик Change </summary>
        private ParameterizedThreadStart eventMessageChange = (obj) =>
        {
            var thisObj = (MarketEvents<T>)obj;
            try
            {
                thisObj.mutexListEventChange.WaitOne();
                IEnumerable<T> list = thisObj.ListEventChange.ToArray();
                thisObj.ListEventChange.Clear();
                thisObj.mutexListEventChange.ReleaseMutex();

                if (thisObj != null && thisObj.OnChange != null)
                    thisObj.OnChange(list);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        };
        protected Mutex mutexThreadChange = new Mutex();
        /// <summary> Поток обработки измененных объектов </summary>
        private Thread ThreadEventChange = null;
        /// <summary> Событие измененного элемента</summary>
        public void GenerateEventOnChange()
        {
            try
            {
                if (this.ListEventChange.Count == 0) return;
                mutexThreadChange.WaitOne();
                if (this.ThreadEventChange != null && this.ThreadEventChange.ThreadState == ThreadState.Running)
                    this.ThreadEventChange.Join();
                this.ThreadEventChange = null;
                this.ThreadEventChange = new Thread(eventMessageChange);
                this.ThreadEventChange.Priority = ThreadPriority.Normal;
                if (this.ThreadEventChange != null && this.ThreadEventChange.ThreadState == ThreadState.Unstarted)
                    this.ThreadEventChange.Start(this);
                mutexThreadChange.ReleaseMutex();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace QuikControl
{
    /// <summary> Класс для отложенного запуск событий из очереди. </summary>
    public class MarketElemActivatorEvent
    {
        /// <summary> Событие нового элемента. </summary>
        public Action NewEvent = null;
        /// <summary> Событие изменения элемента. </summary>
        public Action ChangeEvent = null;
        /// <summary> Строковый тип объекта </summary>
        public string TypeObject = null;
        /// <summary> Активировать весь список событий. </summary>
        public void ExecEvent()
        {
            if (NewEvent != null) NewEvent();
            if (ChangeEvent != null) ChangeEvent();
        }
    }
    public class MarketElement<T>: MarketEvents<T>
    {
        /// <summary> Коллекция элементов </summary>
        protected List<T> Collection = new List<T>();
        /// <summary> Мьютекс для коллекции </summary>
        protected Mutex mutexCollection = new Mutex();

        /// <summary> Получить список коллекции в виде IEnumerable </summary>
        public IEnumerable<T> AsIEnumerable
        {
            get
            {
                mutexCollection.WaitOne();
                var list = this.Collection.AsEnumerable();
                mutexCollection.ReleaseMutex();
                return list;
            }
        }

        /// <summary> Получить список коллекции в виде List </summary>
        public IEnumerable<T> AsList
        {
            get
            {
                mutexCollection.WaitOne();
                var list = this.Collection;
                mutexCollection.ReleaseMutex();
                return list;
            }
        }

        /// <summary> Получить список коллекции в виде Array </summary>
        public T[] AsArray
        {
            get
            {
                mutexCollection.WaitOne();
                var ar = this.Collection.ToArray();
                mutexCollection.ReleaseMutex();
                return ar;

            }
        }

        /// <summary> Кол-во элементов в коллекции </summary>
        public decimal Count { get { return this.Collection.Count; } }
        /// <summary>
        /// Объект запуска отложенных событий
        /// </summary>
        private MarketElemActivatorEvent ActEvents = new MarketElemActivatorEvent();

        public MarketElement(List<MarketElemActivatorEvent> ListAcEvents){
            ActEvents.TypeObject = this.GetType().ToString();
            ActEvents.NewEvent = GenerateEventOnNew;
            ActEvents.ChangeEvent = GenerateEventOnChange;
            ListAcEvents.Add(ActEvents);
        }
        /// <summary>
        /// Заблокировать коллекцию на изменение. Обязательно после, выполнять UnLockCollection
        /// </summary>
        public void LockCollection()
        {
            mutexCollection.WaitOne();
        }
        /// <summary>
        /// Разблокировать коллекцию, после LockCollection
        /// </summary>
        public void UnLockCollection()
        {
            mutexCollection.ReleaseMutex();
        }

        /// <summary> Добавить в коллекцию новый элемент. </summary>
        /// <param name="elem">Элемент коллекции</param>
        /// <param name="generateEvent">true - генерировать событие OnNew</param>
        public void Add(T elem, bool generateEvent = true)
        {
            try
            {
                this.mutexCollection.WaitOne();
                this.Collection.Add(elem);
                this.mutexCollection.ReleaseMutex();

                this.mutexListEventNew.WaitOne();
                this.ListEventNew.Add(elem);
                this.mutexListEventNew.ReleaseMutex();

                if (generateEvent || this.ListEventNew.Count >= this.MaxElementInList)
                {
                    GenerateEventOnNew();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// Изменение элемента в коллекции
        /// </summary>
        /// <param name="elem">Изменяемый элемент</param>
        /// <param name="functBeforeEvent">Действие перед вызовом события OnChange.</param>
        /// <param name="generateEvent">true - генерировать событие OnChange, иначе накапливать список событий.</param>
        public void Change(T elem, bool generateEvent = true)
        {
            try
            {
                mutexListEventChange.WaitOne();
                this.ListEventChange.Add(elem);
                mutexListEventChange.ReleaseMutex();

                if (generateEvent || this.ListEventNew.Count >= this.MaxElementInList)
                {
                    GenerateEventOnChange();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        
    }
}

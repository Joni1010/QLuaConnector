using QuikControl;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ServiceMessage
{
    /// <summary> Класс стека сообщений. </summary>
	public class ServiceStackMessages
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
}

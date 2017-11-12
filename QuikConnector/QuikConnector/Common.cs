using System;
using System.Collections;
using System.Linq;
using System.Threading;

public static class IEnumerableExtension
{
    /// <summary>
    /// Перебор коллекции
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable source, Action<T> action)
    {
        if (source.IsNull()) return;
        if (!action.IsNull())
        {
            foreach (T el in source) action(el);
        }
    }
}

public static class ArrayExtension
{
    /// <summary>
    /// Перебор массива
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this Array source, Action<T> action)
    {
        if (source.IsNull() || source.Length == 0) return;
        if (!action.IsNull())
        {
            foreach (T el in source) action(el);
        }     
    }
}

public static class DateTimeExtension
{
    /// <summary>
    /// Конвертирует дату в строку формата YYYYMMDD
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string ToString_YYYYMMDD(this DateTime date)
    {
        if (date.Year > 2050) return null;
        if (date.Year < 1900) return null;
        return date.Year.ToString() +
            (date.Month < 10 ? '0' + date.Month.ToString() : date.Month.ToString()) +
            (date.Day < 10 ? '0' + date.Day.ToString() : date.Day.ToString());
    }
}

public static class objectExtension
{
    /// <summary>
    /// Функция проверяет пустой объект или нет.
    /// </summary>
    /// <param name="self">Объект</param>
    /// <returns>true - если объект пуст</returns>
    public static bool Empty(this object self)
    {
        if (self == null) return true;
        if (self is string)
            if ((string)self == "") return true;

        return false;
    }
    /// <summary>
    /// Проверяет является ли объект null
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsNull(this object self)
    {
        if (self == null) return true;
        return false;
    }
}

public static class stringExtension
{
    public static decimal ToDecimal(this string str, int scale = -1)
    {
        if (str.Contains('.'))
        {
            str = str.Replace(".", ",");
        }
        if (scale < 0) return Convert.ToDecimal(str);
        return Math.Round(Convert.ToDecimal(str), scale);
    }
    public static Int32 ToInt(this string str)
    {
        return ToInt32(str);
    }
    public static Int32 ToInt32(this string str)
    {
        if (str.Contains('.'))
        {
            return Convert.ToInt32(str.ToDecimal());
        }
        return Convert.ToInt32(str);
    }
    public static Int64 ToLong(this string str)
    {
        return ToInt64(str);
    }
    public static Int64 ToInt64(this string str)
    {
        if (str.Contains('.'))
        {
            return Convert.ToInt64(str.ToDecimal());
        }
        return Convert.ToInt64(str);
    }

    /// <summary>
    /// Конвертирует строки формата YYYYMMDD или YYMMDD в формат даты
    /// </summary>
    /// <param name="str"></param>
    /// <param name="yearNumeral">Кол-во цифр в годе (default = 4)</param>
    /// <returns>DateTime</returns>
    public static DateTime ConvertToDateForm_YYYYMMDD(this string str, int yearNumeral = 4)
    {
        if (str.Empty()) return DateTime.MinValue;
        var date = new Common.DateMarket();
        date.day = str.Substring(6, 2);
        date.month = str.Substring(4, 2);
        date.year = str.Substring(0, 4);
        return Convert.ToDateTime(date.Date);
    }
}

namespace Common
{
    using System.Windows;
    using System.Windows.Threading;

    /// <summary> Объект защищенный мутексом, на единичный доступ</summary>
    /// <typeparam name="T"></typeparam>
    public class LockObject<T>
    {
        private T _object;
        private Mutex LockMutex = new Mutex();
        public LockObject()
        {

        }
        public T Object
        {
            set
            {
                LockMutex.WaitOne();
                this._object = value;
                LockMutex.ReleaseMutex();
            }
            get
            {
                LockMutex.WaitOne();
                T ret = this._object;
                LockMutex.ReleaseMutex();
                return ret;
            }
        }
    }
    public class Ext
    {
        /// <summary>
        /// Создавет новый поток
        /// </summary>
        /// <param name="EventThread"></param>
        /// <returns></returns>
        public static Thread NewThread(ThreadStart EventThread)
        {
            Thread thread = new Thread(EventThread);
            thread.Priority = ThreadPriority.Normal;
            thread.Start();
            return thread;
        }
        public static Thread NewThreadParam(ParameterizedThreadStart EventThread, object param)
        {
            Thread thread = new Thread(EventThread);
            thread.Priority = ThreadPriority.Normal;
            thread.Start(param);
            return thread;
        }

        public static DispatcherTimer NewTimer(Action<object, EventArgs> eventTimer, int min, int sec)
        {
            /*Forms timer = new Timer(;
            dispatcherTimer.Tick += new EventHandler(eventTimer);
            dispatcherTimer.Interval = new TimeSpan(0, 0, min, sec);
            dispatcherTimer.Start();
            return dispatcherTimer;*/
            return null;
        }
    }
    //Дата для правильной конвертации
    public class DateMarket
    {
        public string day = "00";
        public string month = "00";
        public string year = "0000";
        public string hour = "00";
        public string min = "00";
        public string sec = "00";
        public string ms = "000";
        public string mcs = "000000";
        public IEnumerator t;
        public string DateTime
        {
            get
            {
                return day + "/" + month + "/" + year + " " +
                    hour + ":" + min + ":" + sec + "." + ms;
            }
        }
        public string Date
        {
            get
            {
                return day + "." + month + "." + year;
            }
        }
    }
}

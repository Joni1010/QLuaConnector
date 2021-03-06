﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MarketObject;

/// <summary> Библиотека торгуемых элеметов </summary>
namespace TradingLib
{
    /// <summary> Коллекция активных торгуемых инструментов </summary>
    public class TElementCollection
    {
        private List<TElement> _Collection = new List<TElement>();
        private Mutex MutexCollection = new Mutex();
        public TElement[] Collection
        {
            get
            {
                MutexCollection.WaitOne();
                var col = this._Collection.ToArray();
                MutexCollection.ReleaseMutex();
                return col;
            }
        }

        public void Add(TElement newElem)
        {
            if (newElem == null) return;
            MutexCollection.WaitOne();
            this._Collection.Add(newElem);
            MutexCollection.ReleaseMutex();
        }
    }



    /// <summary> Торгуемы активный элемент</summary>
    public class TElement
    {
        public Securities Security = null;
        /// <summary> Коллекция тайм-фреймов со свечками </summary>
        public List<CandleLib.CandleCollection> CollectionTimeFrames = new List<CandleLib.CandleCollection>();
        private Mutex MutexCollectionCandles = new Mutex();

        /// <summary>
        /// Событие новой свечи в любом тайм фрейме
        /// </summary>
        public CandleLib.CandleCollection.EventCandle OnNewCandle = null;

        //private bool HistoryLoaded = false;
        public TElement(Securities sec)
        {
            this.Security = sec;
        }
        public void Create()
        {
            MutexCollectionCandles.WaitOne();
            this.CollectionTimeFrames.Add(new CandleLib.CandleCollection(1));
            this.CollectionTimeFrames.Last().OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
            };

            this.CollectionTimeFrames.Add(new CandleLib.CandleCollection(2));
            this.CollectionTimeFrames.Last().OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
            };

            this.CollectionTimeFrames.Add(new CandleLib.CandleCollection(3));
            this.CollectionTimeFrames.Last().OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
            };

            var tf = new CandleLib.CandleCollection(5);
            tf.ControlTrades = true;
            this.CollectionTimeFrames.Add(tf);
            tf.OnDeleteExtra += (delCandle) =>
            {
                if (this.IndexWriteCandle > 0) this.IndexWriteCandle--;
            };
            tf.OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
                this.SaveCharts();
            };


            this.CollectionTimeFrames.Add(new CandleLib.CandleCollection(15));
            this.CollectionTimeFrames.Last().OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
            };

            this.CollectionTimeFrames.Add(new CandleLib.CandleCollection(30));
            this.CollectionTimeFrames.Last().OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
            };

            this.CollectionTimeFrames.Add(new CandleLib.CandleCollection(60));
            tf = this.CollectionTimeFrames.Last();
            tf.OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
            };
            tf.OnNewOldCandle += (tframe, candle) => { this.SaveCharts(); };

            this.CollectionTimeFrames.Add(new CandleLib.CandleCollection(240));
            this.CollectionTimeFrames.Last().OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
            };

            //this.CollectionTimeFrames.Add(new CANDLE.CandleCollection(720));
            this.CollectionTimeFrames.Add(new CandleLib.CandleCollection(1440));
            this.CollectionTimeFrames.Last().OnNewCandle += (tframe, candle) =>
            {
                if (!this.OnNewCandle.IsNull()) this.OnNewCandle(tframe, candle);
            };

            this.LoadCharts();
            MutexCollectionCandles.ReleaseMutex();
        }
        /// <summary> Сохранение всех котировок в файл </summary>
        public void SaveCharts()
        {
            this.CollectionTimeFrames.ForEach((tf) =>
            {
                this.SaveAllCollection(tf.TimeFrame);
            });
        }

        /// <summary> Загружает котировки из файла </summary>
        public void LoadCharts()
        {
            this.CollectionTimeFrames.ForEach((tf) =>
            {
                this.LoadAllCollection(tf.TimeFrame);
            });
        }

        /// <summary> Получает директорию хранения котировок </summary>
        /// <returns></returns>
        private string GetDirCharts()
        {
            string dir = "./charts/" + this.Security.ClassCode + "/";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            dir = dir + this.Security.Code + "/";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }
        /// <summary>
        /// Возвращает путь к файлу с котировками
        /// </summary>
        /// <param name="timeFrame">Тайм фрейм</param>
        /// <returns></returns>
        private string GetFileCharts(int timeFrame)
        {
            string filePath = GetDirCharts() + this.Security.Code + "_" + timeFrame + "_dump.dat";
            return filePath;
        }


        /// <summary> Запись всей коллекции тайм фреймов в файл </summary>
        private void SaveAllCollection(int timeframe)
        {
            var tf = this.CollectionTimeFrames.FirstOrDefault(t => t.TimeFrame == timeframe);
            if (!tf.IsNull())
            {
                tf.WriteCollectionInFile(this.GetFileCharts(tf.TimeFrame));
            }
        }

        /// <summary> Загрузка тайм-фрейма из файла</summary>
        /// <param name="timeframe"></param>
        private void LoadAllCollection(int timeframe)
        {
            var tf = this.CollectionTimeFrames.FirstOrDefault(t => t.TimeFrame == timeframe);
            if (!tf.IsNull())
            {
                string filePath = GetDirCharts() + this.Security.Code + "_" + tf.TimeFrame + "_dump.dat";
                tf.ReadCollectionFromFile(filePath);
            }
        }


        /// <summary> Индекс записываемой свечки. </summary>
        private int IndexWriteCandle = 0;
        private CandleLib.CandleData LastIndexCandle = null;
        /// <summary> Функция сохранения котировок </summary>
        /// <param name="sec"></param>
        public void SaveCharts(int timeFrame = 1, int limitSave = 20, int stepWait = 5)
        {
            string dir = GetDirCharts();
            var frame = this.CollectionTimeFrames.FirstOrDefault(t => t.TimeFrame == timeFrame);
            if (frame.IsNull()) return;

            int limit = 20; //Кол-ва сохраняемых свечек за раз
            while (limit > 0)
            {
                frame.LockCollection();
                var candle = frame.MainCollection.ToArray().FirstOrDefault(c => !c._write && c._lastUpdate < DateTime.Now.AddMinutes(-5));// this.IndexWriteCandle);
                frame.UnlockCollection();

                if (!candle.Empty())
                {
                    string filename = this.Security.Code + "." + this.Security.Class.Code + "_" + timeFrame + "_" + candle.Time.ToShortDateString() + ".charts";
                    filename = dir + filename;
                    if (this.SaveCandleInfile(filename, candle) != -1)
                    {
                        candle._write = true;
                        this.LastIndexCandle = candle;
                    }
                }
                limit--;
                Thread.Sleep(2);
            }
        }
        /// <summary> Получение последней сделки из файла, по последней строке. </summary>
        /// <param name="lastStr"></param>
        /// <returns></returns>
        private Trade GetLastTradeFromFile(string lastStr)
        {
            if (lastStr != "" && lastStr != null)
            {
                var allData = lastStr.Split('\t');
                if (allData.Length > 3)
                {
                    var t = new Trade();
                    t.Number = allData[6].ToLong();
                    t.DateTrade = Convert.ToDateTime(allData[7]);
                    t.Price = allData[3].ToDecimal();
                    t.Volume = 0;
                    return t;
                }
            }
            return null;
        }

        private CandleLib.CandleData LastSaveCandle = null;
        /// <summary>
        /// 
        /// </summary>
        private int SaveCandleInfile(string filename, CandleLib.CandleData candle)
        {
            FileLib.WFile file = new FileLib.WFile(filename);
            char split = '\t';
            string text = candle.Time.ToString() + split +
                candle.FirstId.ToString() + split +
                candle.Open.ToString().Replace(',', '.') + split +
                candle.Close.ToString().Replace(',', '.') + split + //3
                candle.High.ToString().Replace(',', '.') + split +
                candle.Low.ToString().Replace(',', '.') + split +
                candle.LastId.ToString() + split +                  //6
                candle.LastTime.ToString() + split;                 //7

            if (candle.HorVolumes.HVolCollection.Count > 0)
            {
                candle.HorVolumes.HVolCollection.CollectionArray.ForEach<ChartVol>((vol) =>
                {
                    if (vol.VolBuy > 0) text += "b:" + vol.Price.ToString().Replace(',', '.') + ":" + vol.VolBuy.ToString() + split;
                    if (vol.VolSell > 0) text += "s:" + vol.Price.ToString().Replace(',', '.') + ":" + vol.VolSell.ToString() + split;
                });
            }
            LastSaveCandle = candle;

            if (file.Append(text) == -1) return -1;
            return 0;
        }

        /// <summary> Проверяет наличие директории с историей </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        /*public static bool CheckDirHistory(Securities sec)
        {
            string dir = "./charts/" + sec.Code + "/";
            if (!Directory.Exists(dir)) return false;
            return true;
        }*/

        /// <summary> Загружает котировки из файла с историей. </summary>
        /// <param name="timeFrame">Загружаемый тайм фрейм</param>
        /// <param name="StepDayAgo">Кол-во дней назад, 0 - текущий день, 1 - вчера.</param>
        /*public void LoadHistoryCandle(int timeFrame, int StepDayAgo = -1)
        {
            if (this.Security.Empty()) return;
            string dir = "./charts/" + this.Security.Code + "/";
            if (!Directory.Exists(dir)) return;
            if (StepDayAgo < 0) return;

            string filename = this.Security.Code + "." + this.Security.Class.Code + "_" + timeFrame + "_" + DateTime.Now.AddDays(StepDayAgo * -1).ToShortDateString() + ".charts";
            filename = dir + filename;
            this.ReadHistoryFromFile(filename);
        }*/
        /// <summary> Загружает котировки из файла с историей. </summary>
        /// <param name="timeFrame">Загружаемый тайм фрейм</param>
        /// <param name="date">За указанную дату</param>
        /*public bool LoadHistoryCandle(int timeFrame, DateTime date)
        {
            if (this.Security.Empty()) return false;
            string dir = "./charts/" + this.Security.Code + "/";
            if (!Directory.Exists(dir)) return false;

            string filename = this.Security.Code + "." + this.Security.Class.Code + "_" + timeFrame + "_" + date.ToShortDateString() + ".charts";
            filename = dir + filename;
            this.ReadHistoryFromFile(filename);
            return true;
        }*/
        /// <summary> Чтение истории из файла </summary>
        /// <param name="filename"></param>
        /*private void ReadHistoryFromFile(string filename)
        {
            if (filename.Empty()) return;
            FileLib.WFile file = new FileLib.WFile(filename);
            if (!file.Exists()) return;
            var stringFile = file.ReadAllLines();
            if (stringFile.Length > 0)
            {
                foreach (var str in stringFile)
                {
                    if (str == "") continue;
                    var t = new Trade();
                    var data = str.Split('\t');
                    t.DateTrade = Convert.ToDateTime(data[0]);
                    t.Number = data[1].ToLong();
                    t.Sec = this.Security;
                    t.SecCode = this.Security.Code;
                    //Сделка открытия
                    t.Price = data[2].ToDecimal();
                    t.Volume = 0;
                    this.NewTradeHistory(t);

                    //index 8 volume
                    for (int k = 8; k < data.Length; k++)
                    {
                        var pv = data[k].Split(':');
                        if (pv.Length != 3) continue;
                        if (pv[0] == "b") t.Direction = OrderDirection.Buy;
                        else t.Direction = OrderDirection.Sell;

                        t.Price = pv[1].ToDecimal();
                        t.Volume = pv[2].ToInt32();

                        this.NewTradeHistory(t);
                        //Thread.Sleep(2);
                    }

                    //Сделка закрытия
                    t.Number = data[6].ToLong();
                    t.DateTrade = Convert.ToDateTime(data[7]);
                    t.Price = data[3].ToDecimal();
                    t.Volume = 0;
                    this.NewTradeHistory(t);
                }
            }
        }*/
        /// <summary> Запись новой сделки </summary>
        /// <param name="trade"></param>
        public void NewTrade(Trade trade)
        {
            MutexCollectionCandles.WaitOne();
            int checkTimeFrame = 5;
            var el = this.CollectionTimeFrames.FirstOrDefault(c => c.TimeFrame == checkTimeFrame);
            if (!el.IsNull())
            {
                //С проверкой дубликатов сделок
                if (!el.ExistTrade(trade))
                {
                    this.CollectionTimeFrames.ForEach((tf) =>
                    {
                        tf.AddNewTrade(trade);
                    });
                }
                else
                {
                    this.CollectionTimeFrames.ForEach((tf) =>
                    {
                        if (tf.TimeLastWrite < DateTime.Now.AddMinutes(-5))
                        {
                            tf.WriteCollectionInFile(this.GetFileCharts(tf.TimeFrame));
                        }
                    });
                }
            }
            MutexCollectionCandles.ReleaseMutex();
        }
        /// <summary> Запись новой исторической сделки </summary>
        /// <param name="trade"></param>
        /*public void NewTradeHistory(Trade trade)
        {
            MutexCollectionCandles.WaitOne();
            this.CollectionTimeFrames.ForEach((el) =>
            {
                el.AddNewTrade(trade, true);
            });
            MutexCollectionCandles.ReleaseMutex();
        }*/

    }
}

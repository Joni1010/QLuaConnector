using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileLib
{
    public class WFile
    {
        private string FileName = "";
        public WFile(string fileName)
        {
            this.FileName = fileName;
        }
        /// <summary> Размер файла в байтах </summary>
        /// <returns></returns>
        public long Size()
        {
            if (!this.Exists()) return 0;
            FileInfo file = new FileInfo(this.FileName);
            return file.Length;
        }

        public int Append(string TextWrite)
        {
            //try
            //{
            File.AppendAllText(this.FileName, TextWrite + Environment.NewLine);
            return 0;
            //}
            //catch (Exception e) { return -1; }
        }

        public string[] ReadAllLines()
        {
            if (File.Exists(this.FileName))
                return File.ReadAllLines(this.FileName);
            return null;
        }

        public string ReadLastString()
        {
            try
            {
                if (File.Exists(this.FileName))
                {
                    string[] tmp = File.ReadAllLines(this.FileName);
                    int countStr = tmp.Count();
                    if (countStr > 0) return tmp[countStr - 1] == "" ? tmp[countStr - 2] : tmp[countStr - 1];
                }
            }
            catch (Exception e) { return null; }
            return null;
        }

        public void WriteFileNew(string Text)
        {
            if (File.Exists(this.FileName))
                File.Delete(this.FileName);
            File.WriteAllText(this.FileName, Text);
        }

        public void WriteFileNew(string[] ArrayLines)
        {
            if (File.Exists(this.FileName))
                File.Delete(this.FileName);
            File.WriteAllLines(this.FileName, ArrayLines);
        }

        public void WriteBinary<T>(T obj)
        {
            FileStream fsser = new FileStream(this.FileName, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bfser = new BinaryFormatter();
            bfser.Serialize(fsser, obj);
            fsser.Close();
        }

        public T ReadBinary<T>()
        {
            FileStream fsdis = new FileStream(this.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter bfdis = new BinaryFormatter();
            T obj = (T)bfdis.Deserialize(fsdis);
            fsdis.Close();

            return obj;
        }

        public bool Exists()
        {
            return File.Exists(this.FileName);
        }

        public bool Delete()
        {
            if (this.FileName == "") return false;
            if (this.Exists())
            {
                File.Delete(this.FileName);
                return true;
            }
            return false;
        }

        public bool ClearFile()
        {
            if (this.FileName == "") return false;
            if (this.Delete())
            {
                File.WriteAllText(this.FileName, "");
                return true;
            }
            return false;
        }
    }

    public class WorkFile
    {
        private string PrefixFileLog = "log";
        private string AppendPrefixString = "_";
        private string Path = "";
        private DateTime? DateFile = null;
        public WorkFile(string PrefixFile = "log")
        {
            this.PrefixFileLog = PrefixFile;
            this.AppendPrefixString = "_";
        }
        //Получение и установка пути к файлу
        public string PathFile(string Path = null)
        {
            if (Path != null)
            {
                this.Path = Path;
            }
            return this.Path;
        }

        public void SetDate(DateTime Date)
        {
            this.DateFile = Date;
        }
        //Получить имя файла текущего лога
        protected string GetNameCurFileLog()
        {
            DateTime date = DateTime.Now;
            if (this.DateFile != null)
                date = (DateTime)this.DateFile;
            return this.Path + this.PrefixFileLog + this.AppendPrefixString + date.Year + "-" + date.Month + "-" + date.Day + ".txt";
        }
        public void Write(string TextLog)
        {
            string file = this.GetNameCurFileLog();
            DateTime date = DateTime.Now;
            if (this.PathFile() != "" && !Directory.Exists(this.PathFile()))
                Directory.CreateDirectory(this.PathFile());
            File.AppendAllText(file, date.ToString() + ": " + TextLog + Environment.NewLine);
        }

        public void Append(string TextLog, bool appendDate = false)
        {
            string file = this.GetNameCurFileLog();
            DateTime date = DateTime.Now;
            if (this.PathFile() != "" && !Directory.Exists(this.PathFile()))
                Directory.CreateDirectory(this.PathFile());
            File.AppendAllText(file, (appendDate == true ? date.ToString() + ": " : "") + TextLog + Environment.NewLine);
        }
        //Добавить к префиксу какую-либо информацию
        public void AppendPrefix(string appendPrefixName)
        {
            this.AppendPrefixString = appendPrefixName;
        }

        public string ReadAll()
        {
            string file = this.GetNameCurFileLog();
            return this.ReadAll(file);
        }
        public string ReadAll(string FileName)
        {
            string file = FileName;
            if (File.Exists(file))
                return File.ReadAllText(file);
            else return null;
        }

        //Очищает файл
        public bool ClearFile()
        {
            string file = this.GetNameCurFileLog();
            if (this.PathFile() != "" && !Directory.Exists(this.PathFile()))
                Directory.CreateDirectory(this.PathFile());
            File.Delete(file);
            File.WriteAllText(file, "");
            return true;
        }
    }
}

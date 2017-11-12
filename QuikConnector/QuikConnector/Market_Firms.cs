using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MarketObject
{
    public class Firm
    {
        /// <summary> ID фирмы </summary>
        public string Id = null;
        /// <summary> Название фирмы (биржы) </summary>
        public string Exchange = null;
        /// <summary> Статус фирмы </summary>
        public int Status = -1;
        /// <summary> Название фирмы </summary>
        public string Name = null;
    }
}

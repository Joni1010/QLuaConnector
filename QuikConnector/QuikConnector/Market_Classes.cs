using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketObject
{

    /// <summary> Рыночные классы </summary>
    public class MarketClass
    {
        /// <summary> Класс фирмы </summary>
        public Firm Firm = null;
        /// <summary> ID фирмы </summary>
        public string FirmId;
        /// <summary> Название Класса </summary>
        public string Name = null;
        /// <summary> Код класса </summary>
        public string Code = null;
        /// <summary> Кол-во параметров </summary>
        public int CountParams = 0;
        /// <summary> Кол-во инструментов в классе </summary>
        public int CountSecurities = 0;
    }
}

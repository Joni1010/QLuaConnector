using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketObject
{
    /// <summary> Класс счета</summary>
    public class Account
    {
        /// <summary>
        /// Список классов принадлежащих данному счету
        /// </summary>
        public List<MarketClass> AccClasses = new List<MarketClass>(); 
        /// <summary>
        /// Класс фирмы
        /// </summary>
        public Firm Firm;
        /// <summary>
        /// ID  счета
        /// </summary>
        public string AccID;
        /// <summary>
        /// Тип счета
        /// </summary>
        public int AccType;
    }
}

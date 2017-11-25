using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MarketObject
{
    /// <summary> Класс клиента</summary>
    public class Client
    {
        public string Code;
		/// <summary> Строковое представление клиента </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Code;
		}
    }
}

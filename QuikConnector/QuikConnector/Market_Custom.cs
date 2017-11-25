
using System;

namespace MarketObject
{
    /// <summary> Котировка цены и объема</summary>
    [Serializable]
    public class Chart
    {
        /// <summary> Цена </summary>
        public decimal Price = 0;
        /// <summary> Объем </summary>
        public long Volume = 0;
    }

    /// <summary> Котировка цены и объема buy/sell</summary>
    [Serializable]
    public class ChartVol
    {
        /// <summary> Цена </summary>
        public decimal Price = -1;
        /// <summary> Объем buy </summary>
        public long VolBuy = 0;
        /// <summary> Объем sell </summary>
        public long VolSell = 0;
    }
}

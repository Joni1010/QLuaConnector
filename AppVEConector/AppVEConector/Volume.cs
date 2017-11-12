using System;

namespace VolumeLib
{
    [Serializable]
    public class Volume
    {
        //public DateTime Time;
        public long SumBuy = 0;
        public long SumSell = 0;
        public HVolume HVolCollection = new HVolume();
        //public HVolume VBuy = new HVolume();
        //public HVolume VSell = new HVolume();
        public void AddBuy(decimal price, long volume)
        {
            this.HVolCollection.AddVolume(price, volume, true);
            //this.VBuy.AddVolume(price, volume, true);
            this.SumBuy += volume;
        }
        public void AddSell(decimal price, long volume)
        {
            this.HVolCollection.AddVolume(price, volume, false);
            //this.VSell.AddVolume(price, volume, false);
            this.SumSell += volume;
        }
    }
}

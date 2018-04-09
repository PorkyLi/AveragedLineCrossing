using System;
using System.Collections.Generic;

namespace DataAnalyst
{
    public class PriceList
    {
        public Period period;
        public int interval;
        public List<PriceItem> items = new List<PriceItem>();

        public PriceItem this[int index]
        {
            get { return index >= 0 && index < this.items.Count ? this.items[index] : null; }
        }

        public int Count
        {
            get { return items.Count; }
        }

        public int FindIndex(DateTime date)
        {
            return items.FindIndex(i => i.Date == date);
        }
    }
}

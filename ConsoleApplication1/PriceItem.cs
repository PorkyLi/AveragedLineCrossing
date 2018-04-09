using System;

namespace DataAnalyst
{
    public class PriceItem
    {
        public DateTime Date;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal Close;
        public decimal Volumn;
        public decimal Amount;
        public Period ItemPeriod;

        public PriceItem()
        {
        }

        public PriceItem(DateTime dateTime, decimal open, decimal high, decimal low, decimal close, decimal volumn, decimal amount, Period period)
        {
            Date = dateTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volumn = volumn;
            Amount = amount;
            ItemPeriod = period;
        }

        public PriceItem Clone()
        {
            return new PriceItem()
            {
                Date = this.Date,
                Open = this.Open,
                High = this.High,
                Low = this.Low,
                Close = this.Close,
                Volumn = this.Volumn,
                Amount = this.Amount,
                ItemPeriod = this.ItemPeriod
            };
        }
    }
}

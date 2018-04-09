using System;
using System.Collections.Generic;

namespace DataAnalyst
{
    public class StockData
    {
        private const int CrossRange = 1;
        private List<int> Intervals = new List<int>() { 5, 10, 20};
        public string Code { get; set; }
        public string Name { get; set; }
        public PriceList RawData = new PriceList();
        public List<PriceList> AveragedData = new List<PriceList>();
        public List<CrossData> StockCrossData = new List<CrossData>();

        private void AddAveragedData(Period period)
        {
            if (FindList(period, Intervals[0]) == null)
            {
                Intervals.ForEach(i => AddAveragedData(period, i));
            }
        }

        public void AddAveragedData(Period period, int interval)
        {
            AveragedData.Add(MathLib.GetAveragePrice(MathLib.ConvertPeriod(RawData, period), interval));
        }

        public PriceList FindList(Period period, int interval)
        {
            return AveragedData.Find(l => l.period == period && l.interval == interval);
        }

        //crossup: avgList1 crossup avgList2
        public bool AveragedPriceCrossed(PriceList avgList1, PriceList avgList2, DateTime date, bool crossUp)
        {
            if (avgList1 == null || avgList2 == null || avgList1.period != avgList2.period || avgList1.interval == avgList2.interval)
            {
                return false;
            }

            var adjustedDate = MathLib.GetDateForPeriod(date, avgList1.period);
                        
            var index1 = avgList1.FindIndex(adjustedDate);
            var index2 = avgList2.FindIndex(adjustedDate);
            var previousIndex1 = index1 - 1;
            var previousIndex2 = index2 - 1;
            
            if (previousIndex1 < 0 || previousIndex2 < 0)
            {
                return false;
            }

            if (avgList1[index1].Date != avgList2[index2].Date || avgList1[previousIndex1].Date != avgList2[previousIndex2].Date)
            {
                return false;
            }

            if (crossUp)
            {
                return avgList1[previousIndex1].Close <= avgList2[previousIndex2].Close &&
                    avgList1[index1].Close >= avgList2[index2].Close;
            }
            else
            {
                return avgList1[previousIndex1].Close >= avgList2[previousIndex2].Close &&
                    avgList1[index1].Close <= avgList2[index2].Close;
            }
        }

        public bool AveragedPriceCrossed(PriceList avgList1, PriceList avgList2, DateTime date, int intervalRange, bool crossUp)
        {
            if (avgList1 == null || avgList2 == null || avgList1.period != avgList2.period || avgList1.interval == avgList2.interval)
            {
                return false;
            }

            var adjustedDate = MathLib.GetDateForPeriod(date, avgList1.period);

            var index1 = avgList1.FindIndex(adjustedDate);
            var index2 = avgList2.FindIndex(adjustedDate);
            if (index1 < 0 || index2 < 0 || index1 < intervalRange || index2 < intervalRange)
            {
                return false;
            }

            if (crossUp)
            {
                if (avgList1[index1].Close >= avgList2[index2].Close)
                {
                    for(var i = 1; i <= intervalRange; i++)
                    {
                        if (avgList1[index1 - i].Date != avgList2[index2 - i].Date)
                        {
                            return false;
                        }
                        if (avgList1[index1 - i].Close <= avgList2[index2 - i].Close)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            else
            {
                if (avgList1[index1].Close <= avgList2[index2].Close)
                {
                    for (var i = 1; i < intervalRange; i++)
                    {
                        if (avgList1[index1 - i].Date != avgList2[index2 - i].Date)
                        {
                            return false;
                        }
                        if (avgList1[index1 - i].Close >= avgList2[index2 - i].Close)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public void FindCross(Period period, int range, bool crossUp, DateTime startDate)
        {
            AddAveragedData(period);
            startDate = MathLib.GetDateForPeriod(startDate, period);
            var dataShort = FindList(period, Intervals[0]);
            var dataMedium = FindList(period, Intervals[1]);
            var dataLong = FindList(period, Intervals[2]);
            var count = dataShort.Count;
            if (dataLong.Count < range)
            {
                return;
            }
            
            for (var i = 0; i < range; i++)
            {
                if (AveragedPriceCrossed(dataShort, dataMedium, dataShort[count - i - 1].Date, CrossRange, crossUp)
                    && AveragedPriceCrossed(dataShort, dataLong, dataShort[count - i - 1].Date, CrossRange, crossUp))
                {
                    //Console.WriteLine($"find {Code} in {dataShort[count - i - 1].Date}" 
                    //    + (dataShort[count - i - 1].Close < RawData[RawData.Count-1].Close ? "price up" : "price down"));
                    StockCrossData.Add(new CrossData {
                        Period = period, CrossDate = dataShort[count - i - 1].Date,
                        Direction = dataShort[count - i - 1].Close < RawData[RawData.Count - 1].Close ? CrossDirection.Up : CrossDirection.Down});
                }
            }
        }
    }
}

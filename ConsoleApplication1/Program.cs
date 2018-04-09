using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyst
{
    class Program
    {
        const string basePath = @"C:\zd_gfzq\T0002\export\";
        static void Main(string[] args)
        {
            var data = new List<StockData>();
            var files = Directory.GetFiles(basePath);
            files.ToList().ForEach(f => data.Add(ReadData(f, new DateTime(2017, 12, 22))));

            Console.WriteLine(data.Count.ToString());
            //var crossedData = data.Select(d => d.FindCross(Period.Day, 2, true))
            //    .Where(c => c.CrossDate != DateTime.MinValue)
            //    .ToList();

            //crossedData.ForEach(c => Console.WriteLine($"find {c.Code} in {c.CrossDate} {c.Direction}"));
        }

        private static StockData ReadData(string filePath, DateTime lastDay)
        {
            var stockData = new StockData();
            var sr = new StreamReader(filePath);
            var line = sr.ReadLine();
            stockData.Code = line.Split(' ')[0];
            stockData.RawData.period = Period.Day;
            stockData.RawData.interval = 1;

            sr.ReadLine();
            line = sr.ReadLine();
            while (line != null)
            {
                var fields = line.Split(',');
                if (fields.Length < 2)
                {
                    break;
                }

                var item = new PriceItem()
                {
                    Date = Convert.ToDateTime(fields[0]),
                    Open = decimal.Parse(fields[1]),
                    High = decimal.Parse(fields[2]),
                    Low = decimal.Parse(fields[3]),
                    Close = decimal.Parse(fields[4]),
                    Volumn = decimal.Parse(fields[5]),
                    Amount = decimal.Parse(fields[6]),
                    ItemPeriod = Period.Day
                };
                if (item.Date > lastDay)
                {
                    break;
                }
                stockData.RawData.items.Add(item);

                line = sr.ReadLine();
            }

            sr.Close();

            return stockData;
        }

        private static void FindCross(StockData stockData)
        {
            stockData.AddAveragedData(Period.Day, 5);
            stockData.AddAveragedData(Period.Day, 10);
            stockData.AddAveragedData(Period.Day, 20);
            stockData.AddAveragedData(Period.Week, 5);
            stockData.AddAveragedData(Period.Week, 10);
            stockData.AddAveragedData(Period.Week, 20);
            stockData.AddAveragedData(Period.Month, 5);
            stockData.AddAveragedData(Period.Month, 10);
            stockData.AddAveragedData(Period.Month, 20);

            var day5 = stockData.FindList(Period.Day, 5);
            var day10 = stockData.FindList(Period.Day, 10);
            var day20 = stockData.FindList(Period.Day, 20);
            var week5 = stockData.FindList(Period.Week, 5);
            var week10 = stockData.FindList(Period.Week, 10);
            var week20 = stockData.FindList(Period.Week, 20);

            var count = stockData.AveragedData.Find(l => l.interval == 5 && l.period == Period.Day).Count;
            for (var i = count-1; i > 20; i--)
            {
                if (stockData.AveragedPriceCrossed(day5, day10, day5[i].Date, 5, true) 
                    && stockData.AveragedPriceCrossed(day5, day20, day5[i].Date, 5, true))
                {
                    Console.WriteLine($"find {stockData.Code} in {day5[i].Date}");
                }
            }
        }
    }
}

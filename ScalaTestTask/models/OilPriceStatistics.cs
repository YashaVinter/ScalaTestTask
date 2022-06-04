using ScalaTestTask.services.implementations;
using System;
namespace ScalaTestTask.models
{
    public class OilPriceStatistics
    {
        private readonly AppDBContext _context;
        public OilPriceStatistics(AppDBContext context)
        {
            _context = context;
        }
        public OilPriceInfo[] AllOilRecords()
        {
            return _context.OilPrices;
        }
        public decimal? PriceByDate(DateTime date)
        {
            var priceQuery = (from oilInfo in _context.OilPrices
                              where (oilInfo.DateRange.Start < date) && (date < oilInfo.DateRange.End )
                              select oilInfo.AvgPrice);
            return priceQuery.Any() ? priceQuery.First() : null;
        }
        public decimal AveragePrice(DateTimeRange dateRange)
        {
            var oilPrices = (from oilInfo in _context.OilPrices
                             where IsTimeIntervalBetween(oilInfo.DateRange, dateRange)
                             select oilInfo)
                            .ToArray();
            int rangeElements = oilPrices.Count();
            if (rangeElements == 0)
            {
                throw new ArgumentException("There is no data about this period");
            }
            if (rangeElements == 1)
            {
                return oilPrices.First().AvgPrice;
            }
            return AveragePrice(oilPrices, dateRange);
        }
        public object MinMaxPrice(DateTimeRange dateRange)
        {

            var oilPrices = (from oilInfo in _context.OilPrices
                             where IsTimeIntervalBetween(oilInfo.DateRange, dateRange)
                             select oilInfo)
                            .ToArray();
            int rangeElements = oilPrices.Count();
            if (rangeElements == 0)
            {
                throw new ArgumentException("There is no data about this period");
            }
            if (rangeElements == 1)
            {
                return new { min = oilPrices.First().AvgPrice, max = oilPrices.First().AvgPrice };
            }
            return MinMaxPrice(oilPrices, dateRange);
        }
        private decimal PriceDispercion(DateTimeRange dateRange)
        {
            var oilPrices = (from oilInfo in _context.OilPrices
                             where IsTimeIntervalBetween(oilInfo.DateRange, dateRange)
                             select oilInfo)
                            .ToArray();
            int rangeElements = oilPrices.Count();
            if (rangeElements == 0)
            {
                throw new ArgumentException("There is no data about this period");
            }
            if (rangeElements == 1)
            {
                return 0;
            }
            return PriceDispercion(oilPrices.Select(o => o.AvgPrice));
            throw new NotImplementedException();
        }
        private static bool IsTimeIntervalBetween(DateTimeRange target, DateTimeRange border)
        {
            // the segment is partially within the boundaries, at the beginning
            bool b1 = target.Start.Ticks <= border.Start.Ticks && target.Start.Ticks <= border.End.Ticks &&
                target.End.Ticks >= border.Start.Ticks && target.End.Ticks <= border.End.Ticks;
            // the segment within the boundaries, at the middle
            bool b2 = target.Start.Ticks >= border.Start.Ticks && target.Start.Ticks <= border.End.Ticks &&
                target.End.Ticks >= border.Start.Ticks && target.End.Ticks <= border.End.Ticks;
            // the segment is partially within the boundaries, at the end
            bool b3 = target.Start.Ticks >= border.Start.Ticks && target.Start.Ticks <= border.End.Ticks &&
                target.End.Ticks >= border.Start.Ticks && target.End.Ticks >= border.End.Ticks;
            // the segment within the same range
            bool b4 = target.Start.Ticks <= border.Start.Ticks && target.Start.Ticks <= border.End.Ticks &&
                target.End.Ticks >= border.Start.Ticks && target.End.Ticks >= border.End.Ticks;
            return b1 || b2 || b3 || b4;
        }
        private decimal AveragePrice(IEnumerable<OilPriceInfo> oilPriceInfos, DateTimeRange dateRange) 
        {
            var firstRange = oilPriceInfos.First().DateRange;
            TimeSpan firstTimeSpan = firstRange.End - dateRange.Start;          
            TimeSpan allFirstTimeSpan = firstRange.End - firstRange.Start;
            decimal startWeight = (decimal) (firstTimeSpan / allFirstTimeSpan);

            if (startWeight > 1)
            {
                throw new ArgumentException("There is no data about the previous period");
            }
            var lastRange = oilPriceInfos.Last().DateRange;
            TimeSpan lastTimeSpan = firstRange.End - dateRange.Start;
            TimeSpan allLastTimeSpan = firstRange.End - firstRange.Start;
            decimal endWeight = (decimal)(lastTimeSpan / allLastTimeSpan);
            if (endWeight > 1)
            {
                throw new ArgumentException("There is no data about the further period");
            }
            decimal sumPrice = 0m;
            decimal sumWeight = 0m;
            foreach (var item in oilPriceInfos)
            {
                if (item.DateRange.Equals(firstRange))
                {
                    sumPrice += startWeight * item.AvgPrice;
                    sumWeight += startWeight;
                    continue;
                }
                if (item.DateRange.Equals(lastRange))
                {
                    sumPrice += endWeight * item.AvgPrice;
                    sumWeight += endWeight;
                    continue;
                }
                sumPrice += item.AvgPrice;
                sumWeight += 1m;
            }

            return sumPrice / sumWeight;
        }
        private object MinMaxPrice(IEnumerable<OilPriceInfo> oilPriceInfos, DateTimeRange dateRange)
        {
            return oilPriceInfos.Aggregate(new { Min = decimal.MaxValue, Max = decimal.MinValue },
                (accumulator, o) => new 
                    {
                        Min = Math.Min(o.AvgPrice, accumulator.Min),
                        Max = Math.Max(o.AvgPrice, accumulator.Max)
                    }
                );
        }
        private decimal PriceDispercion(IEnumerable<decimal> prices)
        {
            // Mathematical expectation
            decimal M = prices.Average();
            // Variance (of a discrete random variable)
            decimal D = 0m;
            foreach (var p in prices)
            {
                D += (p-M) * (p - M);
            }
            D /= (decimal)prices.Count();
            return D;
        }
        private double StandartDeviation(double d)
        {
            return Math.Sqrt(d);
        }
        private decimal Trend(IEnumerable<decimal> prices)
        {
            // returns the value k of the linear trend of the function y=kx+b
            throw new NotImplementedException();
        }
    }
}

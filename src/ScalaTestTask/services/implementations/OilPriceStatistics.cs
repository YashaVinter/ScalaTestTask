using System;

using ScalaTestTask.models;
using ScalaTestTask.extensions;

namespace ScalaTestTask.services.implementations
{
    public class OilPriceStatistics
    {
        private readonly AppDBContext _context;
        private readonly AccordStatistics _statistics;
        public OilPriceStatistics(AppDBContext context, AccordStatistics statistics)
        {
            _context = context;
            _statistics = statistics;
        }
        public object AllOilRecords()
        {
            return _context.OilPrices.Select(p => new {start_date = p.DateRange.Start,end_date = p.DateRange.End,price = p.AvgPrice });
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
        public decimal MathExpectationPrice(DateTimeRange dateRange)
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
            return MathExpectationPrice(oilPrices, dateRange);
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

        public decimal PriceVariance(DateTimeRange dateRange)
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
            return PriceVariance(oilPrices, dateRange);
        }
        public decimal PriceStandardDeviation(DateTimeRange dateRange)
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
            return PriceStandardDeviation(oilPrices, dateRange);
        }
        public object PriceLinearRegression(DateTimeRange dateRange)
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
            return PriceLinearRegression(oilPrices, dateRange);
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
            decimal[] prices = oilPriceInfos.Select(p => p.AvgPrice).ToArray();
            double[] weights = WeightsOfDateTimeRanges(oilPriceInfos, dateRange);

            decimal sumPrice = 0m;
            double sumWeight = 0d;
            for (int i = 0; i < prices.Length; i++)
            {
                sumPrice +=  ((decimal)weights[i]) * prices[i];
                sumWeight += weights[i];
            }
            return sumPrice / ((decimal)sumWeight);
        }
        private decimal MathExpectationPrice(IEnumerable<OilPriceInfo> oilPriceInfos, DateTimeRange dateRange)
        {
            double[] observations = oilPriceInfos.Select(p => (double)p.AvgPrice).ToArray();
            double[] weights = WeightsOfDateTimeRanges(oilPriceInfos, dateRange);
            return (decimal)_statistics.Mean(observations, weights);
        }
        private double[] WeightsOfDateTimeRanges (IEnumerable<OilPriceInfo> oilPriceInfos, DateTimeRange dateRange)
        {
            TimeSpan timeSpan = new TimeSpan();
            double[] weights = new double[oilPriceInfos.Count()];
            Array.Fill(weights, 1d);
            // first range
            var firstRange = oilPriceInfos.First().DateRange;
            TimeSpan firstTimeSpan = timeSpan.Diff(dateRange.Start, firstRange.End);
            TimeSpan allFirstTimeSpan = timeSpan.Diff(firstRange);
            double startWeight = (firstTimeSpan / allFirstTimeSpan);
            if (startWeight > 1)
            {
                throw new ArgumentException("There is no data about the previous period");
            }
            weights[0] = startWeight;

            // last range
            var lastRange = oilPriceInfos.Last().DateRange;
            //TimeSpan lastTimeSpan = lastRange.End - dateRange.Start;
            TimeSpan lastTimeSpan = timeSpan.Diff(lastRange.Start, dateRange.End);
            TimeSpan allLastTimeSpan = timeSpan.Diff(lastRange);
            double endWeight = (lastTimeSpan / allLastTimeSpan);
            if (endWeight > 1)
            {
                throw new ArgumentException("There is no data about the further period");
            }
            weights[weights.Length - 1] = endWeight;

            return weights;
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
        private decimal PriceVariance(IEnumerable<OilPriceInfo> oilPriceInfos, DateTimeRange dateRange)
        {
            double[] observations = oilPriceInfos.Select(p => (double)p.AvgPrice).ToArray();
            double[] weights = WeightsOfDateTimeRanges(oilPriceInfos, dateRange);
            return (decimal)_statistics.Variance(observations, weights);
            //
            //// Mathematical expectation
            //decimal M = prices.Average();
            //// Variance (of a discrete random variable)
            //decimal D = 0m;
            //foreach (var p in prices)
            //{
            //    D += (p - M) * (p - M);
            //}
            //D /= (decimal)prices.Count();
            //return D;
        }
        private decimal PriceStandardDeviation(IEnumerable<OilPriceInfo> oilPriceInfos, DateTimeRange dateRange)
        {
            double[] observations = oilPriceInfos.Select(p => (double)p.AvgPrice).ToArray();
            double[] weights = WeightsOfDateTimeRanges(oilPriceInfos, dateRange);
            return (decimal)_statistics.StandardDeviation(observations, weights);
        }

        private object PriceLinearRegression(IEnumerable<OilPriceInfo> oilPriceInfos, DateTimeRange dateRange)
        {
            double[] observations = oilPriceInfos.Select(p => (double)p.AvgPrice).ToArray();
            double[] weights = WeightsOfDateTimeRanges(oilPriceInfos, dateRange);
            return _statistics.LinearRegression(observations, weights);
        }
    }
}

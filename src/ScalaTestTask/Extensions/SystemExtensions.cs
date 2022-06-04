using ScalaTestTask.models;
using System;
namespace ScalaTestTask.extensions
{
    public static class SystemExtensions
    {
        /// <summary>
        /// End - start
        /// </summary>
        /// <param name="span"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static TimeSpan Diff(this TimeSpan span, DateTime start, DateTime end)
        {
            return end - start;
        }
        public static TimeSpan Diff(this TimeSpan span, DateTimeRange dateTimeRange)
        {
            return dateTimeRange.End - dateTimeRange.Start;
        }
    }
}

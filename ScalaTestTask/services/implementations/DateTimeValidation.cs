using ScalaTestTask.models;
using System;
namespace ScalaTestTask.services.implementations
{
    public class DateTimeValidation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FormatException">when things go wrong.</exception>
        public DateTime Validate(string dateString)
        {
            return DateTime.Parse(dateString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FormatException">when things go wrong.</exception>
        /// <exception cref="ArgumentException">when things go wrong.</exception>
        public DateTimeRange Validate(string startDateString, string endDateString)
        {
            DateTime startDate = DateTime.Parse(startDateString);
            DateTime endDate = DateTime.Parse(endDateString);
            if (startDate > endDate)
            {
                throw new ArgumentException("Incorrect date order");
            }
            return new DateTimeRange (startDate, endDate );
        }
    }
}
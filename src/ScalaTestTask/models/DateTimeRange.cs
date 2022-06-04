using System;
namespace ScalaTestTask.models
{
    public class DateTimeRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Diff => End - Start;

    }
}

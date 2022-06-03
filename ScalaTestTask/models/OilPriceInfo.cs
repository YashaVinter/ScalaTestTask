namespace ScalaTestTask.models
{
    //public record OilPriceInfo(DateTime StartDate, DateTime EndDate, decimal AvgPrice);
    public record OilPriceInfo(DateTimeRange DateRange, decimal AvgPrice);

    // DateTimeRange

}
//public class OilPriceInfo
//{
//    public DateTime StartDate { get; set; }
//    public DateTime EndDate { get; set; }
//    public decimal AvgPrice { get; set; }
//}
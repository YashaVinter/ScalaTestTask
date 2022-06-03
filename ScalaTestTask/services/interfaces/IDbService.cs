using ScalaTestTask.models;

namespace ScalaTestTask.services.interfaces
{
    public interface IDbService
    {
        OilPriceInfo[] OilPrices { get; }
    }
}

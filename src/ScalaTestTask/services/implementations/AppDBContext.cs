using ScalaTestTask.models;
using ScalaTestTask.services.interfaces;
using System;

namespace ScalaTestTask.services.implementations
{
    public class AppDBContext : IDbService
    {
        public OilPriceInfo[] OilPrices { get; private set; } = null!;
        public void Init(IEnumerable<OilPriceInfo> oilPriceInfos)
        {
            OilPrices ??= oilPriceInfos.ToArray();            
        }
    }
}

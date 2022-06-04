using ScalaTestTask.models;
using ScalaTestTask.services.interfaces;
using System;
namespace ScalaTestTask.services.implementations
{
    public class OilPriceInfoRepository : IOilPriceInfoRepository
    {
        private readonly AppDBContext _context;
        public OilPriceInfoRepository(AppDBContext context)
        {
            _context = context;
        }
        public OilPriceInfo[] AllPrices()
        {

            return _context.OilPrices;
        }
    }
}

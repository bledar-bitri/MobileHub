using System.Collections.Generic;


namespace Contracts
{
    public class BestRouteContract
    {
        public string RequestId { get; set; }
        public List<CityContract> Route { get; set; }
    }
}

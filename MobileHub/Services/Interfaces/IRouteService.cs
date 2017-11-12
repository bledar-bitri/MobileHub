using System;
using RouteModel;
using System.Collections.Generic;
using Contracts;
using Logging.Interfaces;

namespace Services
{
    public interface IRouteService : IDisposable
    {

        #region Data Access

        RoadInfo GetRoadInfo(int fromLatitude, int fromLongitude, int toLatitude, int toLongitude);
        List<RoadInfo> GetAllRoadInfos();

        #endregion

        #region Route Generation

         List<CityContract> CalculateRouteForUserId(string clientId, int userId, ILogger logger);
     
        #endregion

    }
}

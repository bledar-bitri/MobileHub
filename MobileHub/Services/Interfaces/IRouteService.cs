using RouteModel;
using System.Collections.Generic;

namespace Services
{
    public interface IRouteService
    {
        #region Data Access

        RoadInfo GetRoadInfo(int fromLatitude, int fromLongitude, int toLatitude, int toLongitude);
        List<RoadInfo> GetAllRoadInfos();

        #endregion
    }
}

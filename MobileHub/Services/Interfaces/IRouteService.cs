﻿using System;
using RouteModel;
using System.Collections.Generic;
using TspWithTimeWindows;

namespace Services
{
    public interface IRouteService : IDisposable
    {
    #region Data Access

    RoadInfo GetRoadInfo(int fromLatitude, int fromLongitude, int toLatitude, int toLongitude);
    List<RoadInfo> GetAllRoadInfos();

    #endregion

    #region Route Generation

    List<City> GetRouteForUserId(int userId);

    #endregion

    }
}

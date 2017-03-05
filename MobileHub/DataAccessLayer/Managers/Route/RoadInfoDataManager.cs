using System.Collections.Generic;
using System.Linq;
using DataAccessLayer.HelperClasses;
using RouteModel;

namespace DataAccessLayer.Managers.Route
{
    /// <summary>
    /// Datamanager for RoadInfo entities
    /// </summary>
    public class RoadInfoDataManager : DataManagerBase<MobileHubRouteContext>
    {
        
        /// <summary>
        /// Contructor used by the business logic
        /// </summary>
        public RoadInfoDataManager(bool preloading = false, bool lazyLoadingDefault = false, bool tracking = false)
         : this(preloading, lazyLoadingDefault, tracking, null)
        {

        }

        /// <summary>
        /// This constructor must be internal otherwise we need a EF Reference in the busines logic
        /// </summary>
        internal RoadInfoDataManager(bool preloading = true, bool lazyLoadingDefault = false, bool tracking = false, MobileHubRouteContext routeContext = null)
        : base(lazyLoadingDefault, tracking, routeContext)
        {
            if (preloading) ctx.RoadInfos.ToList();
        }
        
        public RoadInfo GetRoadInfo(int fromLatitude, int fromLongitude, int toLatitude, int toLongitude, bool includeInverse = false, bool? tracking = null)
        {
            var query = from r in Query<RoadInfo>(tracking)
                        where r.FromLatitude == fromLatitude && r.FromLongitude == fromLongitude && r.ToLatitude == toLatitude && r.ToLongitude == toLongitude
                        select r;

            if(includeInverse)
                query = from r in Query<RoadInfo>(tracking)
                        where (r.FromLatitude == fromLatitude && r.FromLongitude == fromLongitude && r.ToLatitude == toLatitude && r.ToLongitude == toLongitude) ||
                                (r.FromLatitude == toLatitude && r.FromLongitude == toLongitude && r.ToLatitude == fromLatitude && r.ToLongitude == fromLongitude)
                        select r;
            
            return query.SingleOrDefault();
        }


        public List<RoadInfo> GetAllRoadInfos(bool? tracking = null)
        {
            return  ctx.RoadInfos.ToList();
        }

        public List<RoadInfo> GetAllRoadsFromThisPoint(int fromLatitude, int fromLongitude)
        {
            IQueryable<RoadInfo> query = ctx.RoadInfos;
            query = query.Where(r => r.FromLatitude == fromLatitude && r.FromLongitude == fromLongitude);
            
            return query.ToList();
        }

        public void SaveRoadInfo(RoadInfo roadInfo, out string statistics)
        {
            SaveRoadInfo(new List<RoadInfo>{ roadInfo }, out statistics);
        }
        public void SaveRoadInfo(List<RoadInfo> roadInfoList, out string statistics)
        {
            statistics = "NO STATS DEFINED";
            foreach (var o in roadInfoList)
            {
                ctx.AddRoadInfo((int)o.FromLatitude, (int)o.FromLongitude, (int)o.ToLatitude, (int)o.ToLongitude, (int)o.Distance, (int)o.TimeInSeconds);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Contracts;
using Logging.Interfaces;
using TspWithTimeWindows;

namespace RoutePlanner
{
    public class RoutePlanerManager
    {
        public static int TimeToSleepBetweenRefreshes = 1000;
        public static int MaximumRoadsPerThread = 5000;
        
        private ILogger logger;


        public List<City> Cities { get; set; }
        public List<Road> Roads { get; set; }
        public List<AddressWithID> Addresses { get; set; }

        public Map map { get; set; }
        public List<City> BestTour { get; set; }
        protected bool IsDebugMode;
        public TaskStatus TskStatus;

        public bool IsReverse { get; set; }
        public bool IsOverview { get; set; }
        public string Source { get; set; }

        public int UserId { get; }

        public int RouteUser { get; set; }

        public string RouteName;
        public bool IsRouteCalculationDone;
        public long MaxTravelTime;
        public DateTime FirstAppointmentTime = DateTime.MinValue;

        public TimeSpan _tspRunTime;
        public double _tspBestTourValue;
        public City currentLocation;

        #region Constructors
        
        public RoutePlanerManager(List<AddressContract> addresses, List<RoadInfoContract> roads, bool isDebugMode, ILogger logger, int userId)
        {
            IsDebugMode = isDebugMode;
            this.logger = logger;
            this.UserId = userId;

            Init();
            foreach (var address in addresses)
            {
                if (address.Latitude.HasValue && address.Longitude.HasValue)
                    Cities.Add(new City(
                        new AddressLocation(new AddressWithID
                            {
                                ID = address.Id,
                                Address = address.Address
                            },
                            new[]
                            {
                                new GeocodeLocation
                                {
                                    Latitude = address.Latitude.Value,
                                    Longitude = address.Longitude.Value
                                }
                            })
                        , address.Address)
                    {
                        Id = address.Id
                    });
            }

            foreach (var road in roads)
            {
                Roads.Add(
                    new Road(Cities.FirstOrDefault(c=>c.Id == road.FromAddressId), Cities.FirstOrDefault(c => c.Id == road.ToAddressId))
                    {
                        Distance = road.Distance,
                        Duration = road.TimeInSeconds
                    }
                    );
            }
        }

        #endregion

        protected void Init()
        {
            Cities = new List<City>();
            Roads = new List<Road>();
            Addresses = new List<AddressWithID>();
            BestTour = new List<City>();
            map = new Map();
        }

        public void Test()
        {
            RemoveZeroLengthRoads();
            CalcBestTourUsingThread(true);
        }

        public void Run()
        {
            try
            {
                IsRouteCalculationDone = false;
                if (Cities.Count <= 0)
                    return;
                
                UpdateProgressStatus(60, "Removing Zero-Length Roads");
                RemoveZeroLengthRoads();
                UpdateProgressStatus(80, "Calculating Tour");
                CalcBestTour();
                
				// TODO: save route

                UpdateProgressStatus(100, "DONE!");
                IsRouteCalculationDone = true;
            }
            catch(Exception e)
            {
                UpdateProgressStatus(10, e.Message+"<br/>" + e.StackTrace.Replace(Environment.NewLine, "<br/>"));
            }
        }

        public void Run(object eventHandler)
        {
            Run();
            var handler = eventHandler as IRouteEventHandler;
            handler?.RouteCalculationDone();
        }

        public void Clear()
        {
            map.Clear();
            Cities.Clear();
            Roads.Clear();
            Addresses.Clear();
        }

        public void CalculateBestRoute()
        {
            if (IsDebugMode) Console.WriteLine("Loading Addresses");
            //LoadGeocodeAddresses();
			
            if (IsDebugMode) DebugAddresses();
            if (IsDebugMode) Console.WriteLine("\n\nLoading Distances");

            //LoadDistances();
			
            RemoveZeroLengthRoads();

            if (IsDebugMode) DebugRoutes();
            if (IsDebugMode) Console.WriteLine("\n\nCalculating Tour");

            CalcBestTour();

            if (IsDebugMode) Console.WriteLine("DONE!");
        }

        #region Getters

        public string GetDebugBestTour()
        {
            return map.GetTourString(BestTour);
        }

        public string GetJsWaypoints(int startPoint, int totalPoints)
        {
            return IsReverse ? map.GetJsReversedWaypoints(BestTour, startPoint, totalPoints) : map.GetJsWaypoints(BestTour, startPoint, totalPoints);
        }

        public string GetJsOverwiewWaypoints(int totalPoints)
        {
            return IsReverse ? map.GetJsReversedOverwiewWaypoints(BestTour, totalPoints) : map.GetJsOverwiewWaypoints(BestTour, totalPoints);
        }

        public List<int> GetAllWaypoints()
        {
            return map.GetAllWaypointIDs(BestTour, IsReverse);
        }

        public List<int> GetAllIDs()
        {
            return map.GetAllIDs(BestTour, IsReverse);
        }

        public List<AddressWithID> GetTourAddresses()
        {
            return map.GetTourAddresses(BestTour, IsReverse);
        }

        public static string GetRouteFilePath(int userId, string routeName)
        {
			// TODO: figure out where the route is going to be saved
            throw new NotImplementedException();
            //return RouteFolder + "/" + RouteFileName.Replace("<UserID>", userId.ToString()).Replace("<RouteName>", routeName) + "." + RouteExtension;
        }

        #endregion

        protected void DebugAddresses()
        {
            Console.WriteLine("LOCATIONS\n===========================\n");
            foreach (City city in Cities)
            {
                Console.WriteLine(city.Location.Address.Address + "\tLAT:" + city.Location.Locations[0].Latitude + "\tLGN:" + city.Location.Locations[0].Longitude);
            }
        }

        protected void DebugRoutes()
        {
            Console.WriteLine("\nROADS\n===========================\n");
            foreach (Road road in Roads)
            {
                Console.WriteLine(road.From.Location.Address.Address + "\t=====>\t" + road.To.Location.Address.Address + "\t" + road.Distance);
            }
        }

        #region Addresses

        public void AddAddress(AddressWithID address, bool ifNotExists = true)
        {
            if (ifNotExists)
            {
                AddressWithID addr = GetAddressByAddress(address.Address);
                if (addr != null)
                {
                    addr.OtherIds.Add(address.ID);
                }
                else
                {
                    Addresses.Add(address);
                }
                /*   
                if (!IsAddressLoaded(address))
                {
                    Addresses.Add(address);
                }
                  */
            }
            else
            {
                Addresses.Add(address);
            }
        }

        public void ReplaceAddress(AddressWithID address)
        {
            var addr = GetAddressByID(address.ID);
            if (addr != null)
            {
                addr.Address = address.Address;
            }
        }

        protected bool IsAddressLoaded(AddressWithID address)
        {
            return Addresses.Any(a => a.Equals(address));
        }

        protected AddressWithID GetAddressByID(int id)
        {
            return Addresses.FirstOrDefault(addr => addr.ID == id);
        }

        protected AddressWithID GetAddressByAddress(string address)
        {
            return Addresses.FirstOrDefault(addr => addr.Address == address);
        }

        #endregion

        #region Load Loactions and Distances 
        
        protected void RemoveZeroLengthRoads()
        {
            foreach (Road r in Roads)
            {
                if (r.Distance <= 0.05) // 50 meeters (the person would walk)
                {
                    RemoveRoadByAddress(r.To.Location.Address.Address);
                    Cities.Remove(r.To);
                    
                    // Add Ids (AktId) from the city we are removing to the original one. This way we have all the ids sorted during our print
                    City city = GetCityByAddressId(r.From.Location.Address.ID);
                    if (city != null)
                    {
                        city.Location.Address.OtherIds.Add(r.To.Location.Address.ID);
                        foreach (var otherIds in r.To.Location.Address.OtherIds)
                            city.Location.Address.OtherIds.Add(otherIds);
                    }
                    RemoveZeroLengthRoads();
                    break;
                }
            }
        }
        protected void RemoveRoadByAddress(string address)
        {
            for (int i = 0; i < Roads.Count; i++)
            {
                if (Roads[i].From.Location.Address.Address == address || Roads[i].To.Location.Address.Address == address)
                {
                    Roads.Remove(Roads[i]);
                    i--;
                }
            }
        }
        public Road GetRoadBetweenAddresses(string from, string to)
        {
            return Roads.FirstOrDefault(t => (t.From.Location.Address.Address == from && t.To.Location.Address.Address == to) || (t.From.Location.Address.Address == to && t.To.Location.Address.Address == from));
        }

        public List<Road> GetRoads()
        {
            var roads = new List<Road>();
            for (int i = 0; i < Cities.Count; i++)
                for (int j = i + 1; j < Cities.Count; j++)
                    roads.Add(new Road(Cities[i], Cities[j]));

            return roads;
        }
        protected void CalcBestTour()
        {
            //World.MaxOptimationTime = Convert.ToInt32(CommonUtils.GetConfigValue("ACO_MaxOptimationTime_Seconds")) * 1000;
            map.Clear();
            foreach (City city in Cities)
                map.AddCity(city);
            foreach (Road road in Roads)
                map.AddRoad(road);

            var w = map.ConstructTsp();
            int startingIndex = 0;
            /*
            for (int i = 0; i < Cities.Count; i++)
            {
                if (Cities[i].Location.Address.Equals(Addresses[0]))
                {
                    startingIndex = i;
                    break;
                }
            }*/
            //w.MaxTravelTime = MaxTravelTime;
            BestTour = w.FindTour(startingIndex);
        }
        protected void CalcBestTourUsingThread(bool isTask = false)
        {
           // World.MaxOptimationTime = Convert.ToInt32(CommonUtils.GetConfigValue("ACO_MaxOptimationTime_Seconds")) * 1000;
            var manualEvent = new ManualResetEvent(false);
            var mapStateInfo = new MapState(map, Cities, Roads, Addresses, UserId, MaxTravelTime, this, manualEvent)
                                   {
                                       NumerOfThreadsNotYetCompleted = 1
                                   };
            MapStateStaticsAccess.GetMapStateStatic(UserId).RpManager = this;
            ThreadPool.QueueUserWorkItem(MapCalculator.CalculateMap, mapStateInfo);
            var dot = "";
            while (mapStateInfo.NumerOfThreadsNotYetCompleted > 0)
            {
                if (isTask)
                {
                    var msg = String.Format("Calculating Tour: [Best: {0}] [Time: {1} of {2}]" + dot, (int) _tspBestTourValue, CommonUtils.FormatTimeSpan(_tspRunTime), CommonUtils.FormatTimeSpan(new TimeSpan(0, 0, 0, 0, World.MaxOptimationTime)));
                    UpdateProgressStatus(80, msg);
                    dot += ".";
                    if (dot == ".....") dot = "";
                    Thread.Sleep(TimeToSleepBetweenRefreshes);
//                    TextWriter tw = new StreamWriter("C:/temp/RoutePlanerManager.txt");

//                    tw.WriteLine(msg);
//
//                    tw.Flush();
//                    tw.Close();
//                    tw.Dispose();
                }
            }
            manualEvent.WaitOne();
            BestTour = mapStateInfo.BestTour;
        }
        protected City GetCityByAddressId(int id)
        {
            return Cities.FirstOrDefault(city => city.Location.Address.ID == id);
        }
        protected bool ExistsInCities(City c)
        {
            return Cities.FirstOrDefault(city => city.Equals(c)) != null;
        }
        #endregion

        public void UpdateProgressStatus(int pct, string message)
        {
            if(TskStatus != null)
            {
                //TskStatus.Progress = pct;
                //TskStatus.ProgressText = message;
            }
            logger.LogMessage(UserId, $"{pct} {message}", pct == 100);
        }

        public void UpdateProgresLongMessage(string longMessage)
        {
            if (TskStatus != null)
            {
                //TskStatus.ProgressTextLong = longMessage;
            }
        }

        public List<CityContract> BestTourContract()
        {
            var list = new List<CityContract>();
            BestTour.ForEach(c => list.Add(new CityContract{ Name = c.Name}));
            return list;
        }
        #region ISerializable
        protected RoutePlanerManager(SerializationInfo info, StreamingContext context)
        {
            Cities = (List<City>)info.GetValue("Cities", typeof(List<City>));
            Roads = (List<Road>)info.GetValue("Roads", typeof(List<Road>));
            Addresses = (List<AddressWithID>)info.GetValue("Addresses", typeof(List<AddressWithID>));
            BestTour = (List<City>)info.GetValue("BestTour", typeof(List<City>));
            TskStatus = (TaskStatus)info.GetValue("TskStatus", typeof(TaskStatus));
            
            map = (Map)info.GetValue("map", typeof(Map));

            IsDebugMode = info.GetBoolean("_debug");
            IsReverse = info.GetBoolean("IsReverse");
            IsOverview = info.GetBoolean("IsOverview");
            Source = info.GetString("Source");
            UserId = info.GetInt32("_userId");
            RouteName = info.GetString("RouteName");
            RouteUser = info.GetInt32("RouteUser");
            try
            {
                // backward compatibility
                FirstAppointmentTime = info.GetDateTime("FirstAppointmentTime");
                MaxTravelTime = info.GetInt64("MaxTravelTime");
            }
            catch
            {
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Cities", Cities, typeof(List<City>));
            info.AddValue("Roads", Roads, typeof(List<Road>));

            info.AddValue("Addresses", Addresses, typeof(List<AddressWithID>));
            
            info.AddValue("BestTour", BestTour, typeof(List<City>));
            info.AddValue("TskStatus", TskStatus, typeof(TaskStatus));
            info.AddValue("map", map, typeof(Map));
            
            info.AddValue("_debug", IsDebugMode);
            info.AddValue("IsReverse", IsReverse);
            info.AddValue("IsOverview", IsOverview);
            info.AddValue("Source", Source);
            info.AddValue("_userId", UserId);
            info.AddValue("RouteName", RouteName);
            info.AddValue("RouteUser", RouteUser);
            info.AddValue("MaxTravelTime", MaxTravelTime);
            info.AddValue("FirstAppointmentTime", FirstAppointmentTime);
            
        }
        #endregion

        public static int GetIntFromCoordinate(double coordinate)
        {
            int ret = 0;
            try
            {
                var corString = coordinate.ToString(CultureInfo.InvariantCulture).Replace(".", "");
                if (corString.Length > 7)
                    corString = corString.Substring(0, 7);

                ret = Convert.ToInt32(corString);
            }
            catch
            {
            }
            return ret;
        }

        public string GetTourString(IEnumerable<City> tour)
        {
            return "<br/>Die beste Route bisher: <br/>"+map.GetProgressTour(tour);
        }
        public void DumpRoads(StreamWriter writer)
        {

            writer.WriteLine("=====================Roads===========================");
            foreach (Road r in Roads)
            {
                writer.WriteLine(r.DebuggerDisplay);
            }
            writer.WriteLine("=====================END Roads===========================");
        }

        public void SetTotalTime(TimeSpan ts)
        {
            _tspRunTime = ts;
        }

        public void SetBestTourValue(double value)
        {
            _tspBestTourValue = value;
        }
    }

    #region Threads

   
    #region Map
    internal class MapState
    {
        public readonly List<City> Cities;
        public readonly List<Road> Roads;
        public readonly Map Map;
        public readonly List<AddressWithID> Addresses;
        public readonly ManualResetEvent ManualEvent;
        public int NumerOfThreadsNotYetCompleted;
        public List<City> BestTour;
        public int UserId = -1;
        public long MaxTravelTime;
        public RoutePlanerManager rpManager;
        public MapState(Map map, List<City> cities, List<Road> roads, List<AddressWithID> addresses, int userId, long maxTravelTime, RoutePlanerManager rpm, ManualResetEvent manualEvent)
        {
            Map = map;
            Cities = cities;
            Roads = roads;
            Addresses = addresses;
            UserId = userId;
            MaxTravelTime = maxTravelTime;
            rpManager = rpm;
            ManualEvent = manualEvent;
        }
    }
    
    internal static class MapCalculator
    {
        public static void CalculateMap(object state)
        {
            var mapStateInfo = (MapState) state;

            mapStateInfo.Map.Clear();
            foreach (City city in mapStateInfo.Cities)
                mapStateInfo.Map.AddCity(city);
            foreach (Road road in mapStateInfo.Roads)
                mapStateInfo.Map.AddRoad(road);

            var w = mapStateInfo.Map.ConstructTsp();
            //w.AddTspProgressListener(mapStateInfo.rpManager);
           // w.UserID = mapStateInfo.UserId;
//            w.Update += UpdateInfo;
            int startingIndex = 0;
            for (int i = 0; i < mapStateInfo.Cities.Count; i++)
            {
                if (mapStateInfo.Cities[i].Location.Address.Equals(mapStateInfo.Addresses[0]))
                {
                    startingIndex = i;
                    break;
                }
            }
            //w.MaxTravelTime = mapStateInfo.MaxTravelTime;
            mapStateInfo.BestTour = w.FindTour(startingIndex);
            mapStateInfo.NumerOfThreadsNotYetCompleted = 0;
            mapStateInfo.ManualEvent.Set();
             
//            if (mapStateInfo.rpManager.RouteUser > 0 && !string.IsNullOrEmpty(mapStateInfo.rpManager.RouteName))
//            {
//                mapStateInfo.rpManager.BestTour = mapStateInfo.BestTour;
//                string routeFilePath = RoutePlanerManager.GetRouteFilePath(mapStateInfo.rpManager.RouteUser, mapStateInfo.rpManager.RouteName);
//                FileSerializer<RoutePlanerManager>.Serialize(routeFilePath, mapStateInfo.rpManager);
//            }
        }
    }

    internal class MapStateStatics
    {
        public RoutePlanerManager RpManager;
    }

    internal static class MapStateStaticsAccess
    {
        private static readonly Dictionary<int, MapStateStatics> AccessDictionary = new Dictionary<int, MapStateStatics>();
        public static MapStateStatics GetMapStateStatic(int id)
        {
            if (AccessDictionary.ContainsKey(id))
                return AccessDictionary[id];

            var value = new MapStateStatics();
            AccessDictionary.Add(id, value);
            return value;
        }
    }
    #endregion

    #endregion

}
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

using Google.Api.Maps.Service;
using Google.Api.Maps.Service.Geocoding;
using HTB.Database;
using HTB.Database.StoredProcs;
using HTB.GeocodeService;
using HTB.v2.intranetx.progress;
using HTB.v2.intranetx.util;
using HTBAntColonyTSP;
using HTBUtilities;
using System.Collections;
using HTB.RouteService;
using System.Reflection;
using GeocodeLocation = HTB.GeocodeService.GeocodeLocation;
using System.Runtime.CompilerServices;

namespace HTB.v2.intranetx.routeplanter
{
    [XmlRoot(ElementName = "RoutePlannerManager", IsNullable = false)]
    [Serializable]
    public class RoutePlanerManager : Task, ISerializable, ITspProgressListener
    {
        public static int TimeToSleepBetweenRefreshes = 1000;
        public static int MaximumRoadsPerThread = 5000;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly string BingMapsKey = HTBUtils.GetConfigValue("BingMapsKey");
        public static readonly int BingMapMaxWaypoints = Convert.ToInt32(HTBUtils.GetConfigValue("BingMapsMaxWaypoints"));

        public static readonly string RouteFolder = HTBUtils.GetConfigValue("RouteFolder");
        public static readonly string RouteFileName = HTBUtils.GetConfigValue("RouteFileName");
        public static readonly string RouteExtension = HTBUtils.GetConfigValue("RouteExtension");
        public static readonly int ADGegnerStopMinutes = Convert.ToInt32(HTBUtils.GetConfigValue("ADGegnerStopMinutes"));

        public List<City> Cities { get; set; }
        public List<Road> Roads { get; set; }
        public List<AddressWithID> Addresses { get; set; }
        public List<AddressWithID> BadAddresses { get; set; }
        public List<AddressWithID> MultipleLocationsAddresses { get; set; }

        public Map map { get; set; }
        public List<TspCity> BestTour { get; set; }
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

        public RoutePlanerManager(int userId) : this(userId, "")
        {

        }

        public RoutePlanerManager(int userId, TaskStatus tskStatus) : this(userId, "")
        {
            TskStatus = tskStatus;
            Init();
        }

        public RoutePlanerManager(int userId, string taskID) : this(userId, false, taskID)
        {
            Init();
        }

        public RoutePlanerManager(int userId, bool isDebugMode, string taskID) : base(taskID)
        {
            UserId = userId;
            IsDebugMode = isDebugMode;
            Init();
        }

        #endregion

        protected void Init()
        {
            Cities = new List<City>();
            Roads = new List<Road>();
            Addresses = new List<AddressWithID>();
            BadAddresses = new List<AddressWithID>();
            MultipleLocationsAddresses = new List<AddressWithID>();
            BestTour = new List<TspCity>();
            map = new Map();
        }

        public void Test()
        {
            RemoveZeroLengthRoads();
            CalcBestTourUsingThread(true);
//            var sw = new StreamWriter("C:/temp/RotuePlannerManagerDump.txt");
//            DumpRoads(sw);
//            sw.Flush();
//            sw.Close();
//            sw.Dispose();
//            CalcBestTour();
        }

        public override void Run()
        {
            try
            {
                IsRouteCalculationDone = false;
                if (Addresses.Count <= 0)
                    return;
                UpdateProgressStatus(10, "Loading Addresses");
                LoadGeocodeAddresses(true);
                if (BadAddresses.Count == 0)
                {
                    UpdateProgressStatus(20, "Loading Distances");
                    LoadDistances(null, true);
                    UpdateProgressStatus(40, "Saving Distances");
                    SaveDistances();
                    UpdateProgressStatus(60, "Removing Zero-Length Roads");
                    RemoveZeroLengthRoads();
                    UpdateProgressStatus(80, "Calculating Tour");
                    CalcBestTour();
                    //CalcBestTourUsingThread(true);
                }
                if (RouteUser > 0 && !string.IsNullOrEmpty(RouteName))
                {
                    string routeFilePath = GetRouteFilePath(RouteUser, RouteName);
                    FileSerializer<RoutePlanerManager>.Serialize(routeFilePath, this);
                }
                UpdateProgressStatus(100, "DONE!");
                IsRouteCalculationDone = true;
            }
            catch(Exception e)
            {
                UpdateProgressStatus(10, e.Message+"<br/>" + e.StackTrace.Replace(Environment.NewLine, "<br/>"));
            }
        }

        public override void Run(object eventHandler)
        {
            Run();
            if (eventHandler is IRouteEventHandler)
                ((IRouteEventHandler) eventHandler).RouteCalculationDone();
        }

        public void Clear()
        {
            map.Clear();
            Cities.Clear();
            Roads.Clear();
            Addresses.Clear();
            BadAddresses.Clear();
            MultipleLocationsAddresses.Clear();
        }

        public void CalculateBestRoute()
        {
            if (IsDebugMode) Console.WriteLine("Loading Addresses");
            Log.Info("Loading Addresses");
            LoadGeocodeAddresses();

            Log.Info(Addresses.Count + " Addresses: Loading Distances");

            if (IsDebugMode) DebugAddresses();
            if (IsDebugMode) Console.WriteLine("\n\nLoading Distances");

            LoadDistances();

            Log.Info("Saving Distances");
            SaveDistances();

            Log.Info("Removing Zero-Length Roads");
            RemoveZeroLengthRoads();

            Log.Info("Calculating Tour");
            if (IsDebugMode) DebugRoutes();
            if (IsDebugMode) Console.WriteLine("\n\nCalculating Tour");

            CalcBestTour();

            Log.Info("DONE");
            if (IsDebugMode) Console.WriteLine("DONE!");
        }

        #region Getters

        public string GetDebugBestTour()
        {
            return map.GetTourString(BestTour);
        }

        public string GetJsWaypoints(int startPoint, int totalPoints)
        {
            Log.Error("BestTourCount " + BestTour.Count);
            return IsReverse ? map.GetJsReversedWaypoints(BestTour, startPoint, totalPoints) : map.GetJsWaypoints(BestTour, startPoint, totalPoints);
        }

        public string GetJsOverwiewWaypoints(int totalPoints)
        {
            Log.Error("BestTourCount "+BestTour.Count);
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
            return RouteFolder + "/" + RouteFileName.Replace("<UserID>", userId.ToString()).Replace("<RouteName>", routeName) + "." + RouteExtension;
        }

        #endregion

        protected void DebugAddresses()
        {
            Console.WriteLine("LOCATIONS\n===========================\n");
            foreach (City city in Cities)
            {
                Console.WriteLine(city.Address.Address + "\tLAT:" + city.Location.Locations[0].Latitude + "\tLGN:" + city.Location.Locations[0].Longitude);
            }
        }

        protected void DebugRoutes()
        {
            Console.WriteLine("\nROADS\n===========================\n");
            foreach (Road road in Roads)
            {
                Console.WriteLine(road.From.Address + "\t=====>\t" + road.To.Address + "\t" + road.Distance);
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

        public void LoadGeocodeAddresses(bool isTask = false, string programToRestart = null)
        {
            var manualEvent = new ManualResetEvent(false);
            AddressLookupStateStaticsAccess.GetAddressStateStatic(UserId).Clear();
            // Queue the work items.
            var threads = new List<AddressLookupThread>();
            int id = 0;
            foreach (AddressWithID addressWithID in Addresses)
            {
                var adrLookupStateInfo = new AddressLookupState(addressWithID, manualEvent, UserId);
                var adrLookupThread = new AddressLookupThread(id++);
                var thread = new Thread(() => AddressLookupThreadMonitor.LoadGeocodeAddress(adrLookupStateInfo, adrLookupThread));
                threads.Add(adrLookupThread);
                thread.Start();
            }

            var dot = "";
//            var tmp = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
//            var tmpIdx = 0;
            Console.WriteLine("AddressLookup: [" + programToRestart + "] " + AddressLookupThreadMonitor.GetRunningThreadsCount(threads) + " Running Threads");
            Log.Info("AddressLookup: [" + programToRestart + "] " + AddressLookupThreadMonitor.GetRunningThreadsCount(threads) + " Running Threads");
            while (AddressLookupThreadMonitor.GetRunningThreadsCount(threads) > 0)
            {

//                if (IsDebugMode)
//                {
                    // Turn On debuger if number of running threads does not change in three (tmp.length) iterations
//                    tmp[tmpIdx] = AddressLookupThreadMonitor.GetRunningThreadsCount(threads);
//                    if (tmp[0] > 0 &&
//                        (tmp[0] == tmp[1] &&
//                         tmp[0] == tmp[2] &&
//                         tmp[0] == tmp[3] &&
//                         tmp[0] == tmp[4] &&
//                         tmp[0] == tmp[5] &&
//                         tmp[0] == tmp[6] &&
//                         tmp[0] == tmp[7] &&
//                         tmp[0] == tmp[8] &&
//                         tmp[0] == tmp[8] &&
//                         tmp[0] == tmp[9]))
//                    {
//                        Console.WriteLine("AddressLookup: DEBUG TRUE");
//                        if (!string.IsNullOrEmpty(programToRestart))
//                        {
//                            try
//                            {
                                /** restart the program if there are still running threads **/
                                //run the program again and close this one
//                                if (IsDebugMode)
//                                    Console.WriteLine("***Restarting Program!!!***");
//                                Process.Start(programToRestart);

                                //close this one
//                                Process.GetCurrentProcess().Kill();
//                            }
//                            catch
//                            {
//                            }
//                        }
//                    }
//                    tmpIdx++;
//                    if (tmpIdx >= tmp.Length)
//                        tmpIdx = 0;
//                }
                if (isTask)
                {
                    UpdateProgressStatus(10, "Loading Addresses: " + AddressLookupThreadMonitor.GetRunningThreadsCount(threads) + " of " + Addresses.Count + dot);
                    dot += ".";
                    if (dot == ".....") dot = "";

                }
                // wait for the work items to signal before exiting.
                Thread.Sleep(TimeToSleepBetweenRefreshes);
            }
            Cities = AddressLookupStateStaticsAccess.GetAddressStateStatic(UserId).Addresses;
            BadAddresses = AddressLookupStateStaticsAccess.GetAddressStateStatic(UserId).BadAddresses;
            MultipleLocationsAddresses = AddressLookupStateStaticsAccess.GetAddressStateStatic(UserId).MultipleAddresses;
            //Console.WriteLine(string.Format("cities: {0}  bad: {1}  multi: {2}", Cities.Count, BadAddresses.Count, MultipleLocationsAddresses.Count));
            foreach (var addressWithId in Cities)
            {
                Log.Info(string.Format("Address:{0} ", addressWithId.DebuggerDisplay));
            }
            foreach (var addressWithId in BadAddresses)
            {
                Log.Warn("Bad address:" + addressWithId.DebuggerDisplay);
            }
            
            /*
             * Add the location from tablet as the first city on the road
             */
            if (currentLocation != null)
            {
                bool found = false;
                foreach (City c in Cities)
                {
                    if(c.Address.Address.Equals(currentLocation.Address.Address))
                    {
                        c.Location = currentLocation.Location;
                        found = true;
                        Log.Info(string.Format("Current Location:{0} ", c.Location.DebuggerDisplay));
                        break;
                    }
                }
                if (!found && currentLocation.Location.Locations.Any() &&
                    !HTBUtils.IsZero(currentLocation.Location.Locations[0].Latitude) &&
                    !HTBUtils.IsZero(currentLocation.Location.Locations[0].Longitude)
                    )
                {
                    foreach (AddressWithID address in BadAddresses)
                    {
                        if (address.Address.Equals(currentLocation.Address.Address))
                        {
                            Cities.Add(currentLocation);
                            BadAddresses.Remove(address);
                            break;
                        }
                    }
                }
            }
        }

        public void LoadDistances(List<Road> roads = null, bool isTask = false)
        {
            if (roads == null)
                roads = GetRoads();

            string msg;
            int roadCount = 0;
            var roadsToProcess = new List<Road>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (Road r in roads)
            {
                roadsToProcess.Add(r);
                if (++roadCount % MaximumRoadsPerThread == 0)
                {
                    msg = string.Format("Loading Distances: {0} of {1}", roadCount, roads.Count);
                    UpdateProgressStatus(20, msg+" ["+HTBUtils.FormatTimeSpan(stopwatch.Elapsed)+"]");
                    LoadRoadDistances(roadsToProcess, msg, stopwatch, isTask);
                    SaveDistances();
                    roadsToProcess.Clear();
                }
            }
            msg = string.Format("Loading Distances : {0} of {1}", roads.Count, roads.Count);
            UpdateProgressStatus(20, msg + " [" + HTBUtils.FormatTimeSpan(stopwatch.Elapsed) + "]");
            LoadRoadDistances(roadsToProcess, msg, stopwatch, isTask);
            stopwatch.Stop();
            SaveDistances();
            Roads = roads;
        }
        public void LoadRoadDistances(List<Road> roads, string msg, Stopwatch stopwatch, bool isTask = false)
        {
            if (roads == null)
                return;

            var manualEvent = new ManualResetEvent(false);
            var threads = new List<RoadDistanceCalculatorThread>();
            int id = 0;
            Log.Info("Distance Threads " + roads.Count);

            foreach (Road t in roads)
            {
                var roadDistanceStateInfo = new RoadDistanceState(UserId, t, manualEvent);
                var rdct = new RoadDistanceCalculatorThread(id++);
                var thread = new Thread(() => RoadDistanceCalculatorThreadMonitor.LoadDistances(roadDistanceStateInfo, rdct));
                threads.Add(rdct);
                thread.Start();
                //                Thread.Sleep(99999999);
            }
            var dot = "";
            while (RoadDistanceCalculatorThreadMonitor.GetRunningThreadsCount(threads) > 0)
            {
                Log.Info("RoadDistance: " + RoadDistanceCalculatorThreadMonitor.GetRunningThreadsCount(threads) + " Running Threads");
                
                if (isTask)
                {
                    UpdateProgressStatus(20, string.Format(msg + " [" + HTBUtils.FormatTimeSpan(stopwatch.Elapsed) + "]" + " {0} alive ", RoadDistanceCalculatorThreadMonitor.GetRunningThreadsCount(threads)) + dot);
                    dot += ".";
                    if (dot == ".....") dot = "";
                }
                Thread.Sleep(TimeToSleepBetweenRefreshes);
            }
        }

        public void LoadDistances_NoRoutePlanner(int secondsToWaitTillAbort, Stopwatch stopwatch, List<Road> roads = null, bool isTask = false, string programToRestart = null)
        {
            if (roads == null)
                roads = GetRoads();

            //Console.WriteLine("Load Distances Latitude: {0}", roads[0].From.Location.Locations[0].Latitude);
            var manualEvent = new ManualResetEvent(false);
            RoadDistanceStateStaticsAccess.GetDistanceStatestatic(UserId).ResultsList.Clear();
            Log.Info("Distance Threads " + roads.Count);
            var threads = new List<RoadDistanceCalculatorThread>();
            int id = 0;
            foreach (Road t in roads)
            {
                var roadDistanceStateInfo = new RoadDistanceState(UserId, t, manualEvent);
                var rdct = new RoadDistanceCalculatorThread(id++);
                var thread = new Thread(() => RoadDistanceCalculatorThreadMonitor.LoadDistances(roadDistanceStateInfo, rdct));
                threads.Add(rdct);
                thread.Start();
            }
            var dot = "";
            var watch = new Stopwatch();
            watch.Start();
            while (RoadDistanceCalculatorThreadMonitor.GetRunningThreadsCount(threads) > 0)
            {
                Thread.Sleep(TimeToSleepBetweenRefreshes);
                if (isTask)
                {
                    
                    UpdateProgressStatus(20, "Loading Distances: " + RoadDistanceCalculatorThreadMonitor.GetRunningThreadsCount(threads) + " of " + roads.Count + dot);
                    dot += ".";
                    if (dot == ".....") dot = "";
                }
                
                if (IsDebugMode)
                {
//                    var sb = new StringBuilder("RoadDistance: [" + programToRestart + "] ");
                    var sb = new StringBuilder("RoadDistance: [running: ");
                    List<RoadDistanceCalculatorThread> runningThreads = RoadDistanceCalculatorThreadMonitor.GetRunningThreads(threads);
                    sb.Append(runningThreads.Count);
                    //sb.Append(" Running Threads [");
                    //foreach (var runningThread in runningThreads)
                    //{
                    //    sb.Append(runningThread.ID);
                    //    sb.Append(", ");
                    //}
                    //sb.Remove(sb.Length - 2, 2);
                    //sb.Append("]");
                    Console.Write(sb + " " + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("] [Consumed: {0} Of: {1}] [Total Time: {2}]", HTBUtils.FormatTimeSpan(watch.Elapsed), secondsToWaitTillAbort, HTBUtils.FormatTimeSpan(stopwatch.Elapsed));
                }
                if (watch.ElapsedMilliseconds > secondsToWaitTillAbort * 1000)
                {
                    Console.Write(" A");
                    if (IsDebugMode)
                    {
                        Console.WriteLine("***Aborting Threads: [Running Time: {0}] [Limit: {1}] [Alive Treads: {2}]", secondsToWaitTillAbort, watch.Elapsed, RoadDistanceCalculatorThreadMonitor.GetRunningThreadsCount(threads));
                    }
                    RoadDistanceCalculatorThreadMonitor.AbortAllThreads(threads);
                    int abortTries = 0;
                    while (RoadDistanceCalculatorThreadMonitor.GetRunningThreadsCount(threads) > 0)
                    {
                        var sb = new StringBuilder("Trying to abort: ");
                        List<RoadDistanceCalculatorThread> runningThreads = RoadDistanceCalculatorThreadMonitor.GetRunningThreads(threads);
                        sb.Append(runningThreads.Count);
                        //sb.Append(" Threads [");
                        //foreach (var runningThread in runningThreads)
                        //{
                        //    sb.Append(runningThread.ID);
                        //    sb.Append(", ");
                        //}
                        //sb.Remove(sb.Length - 2, 2);
                        //sb.Append("]");
                        Console.WriteLine(sb + " " + DateTime.Now.ToShortTimeString() +" Consumed "+abortTries+" Of "+100);

                        RoadDistanceCalculatorThreadMonitor.AbortAllThreads(threads, abortTries++ > 100);
                        Thread.Sleep(TimeToSleepBetweenRefreshes);
                    }
                    if (IsDebugMode)
                    {
                        Console.WriteLine("***Load-Distances Aborted [or skipped]!!!***");
                    }
                    if (!string.IsNullOrEmpty(programToRestart))
                    {
//                        if (RoadDistanceCalculatorThreadMonitor.GetRunningThreadsCount(threads) > 0)
                        {
                            try
                            {
                                /** restart the program if there are still running threads **/
                                //run the program again and close this one
                                if (IsDebugMode)
                                    Console.WriteLine("***Restarting Program!!!***");
//                                Process.Start(programToRestart);
                                
                                //close this one
//                                Process.GetCurrentProcess().Kill();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }
        
        public void SaveDistances_old()
        {
            try
            {
                var set = new RecordSet(DbConnection.ConnectionType_DB2);
//                set.ExcecuteNonQuery(RoadDistanceState.DistanceUpdateCommands.ToString());
                set.ExecuteNonQuery(RoadDistanceStateStaticsAccess.GetDistanceStatestatic(UserId).DistanceUpdateCommands.ToString());
            }
            catch
            {
                // Ignore Any SQL ERRORS for now
            }
        }

        public void SaveDistances()
        {
            try
            {
                if (RoadDistanceCalculatorThreadMonitor.DistanceUpdateCommands.ToString().Trim().Length > 0)
                {
                    var set = new RecordSet(DbConnection.ConnectionType_DB2);
                    set.ExecuteNonQuery(RoadDistanceCalculatorThreadMonitor.DistanceUpdateCommands.ToString());
                    RoadDistanceCalculatorThreadMonitor.DistanceUpdateCommands.Clear();
                }
            }
            catch (Exception e)
            {
                UpdateProgressStatus(20, "Save Distances Error: "+e.Message);
            }
        }

        protected void RemoveZeroLengthRoads()
        {
            foreach (Road r in Roads)
            {
                if (r.Distance <= 0.05) // 50 meeters (the person would walk)
                {
                    RemoveRoadByAddress(r.To.Address.Address);
                    Cities.Remove(r.To);
                    
                    // Add Ids (AktId) from the city we are removing to the original one. This way we have all the ids sorted during our print
                    City city = GetCityByAddressId(r.From.Address.ID);
                    if (city != null)
                    {
                        city.Address.OtherIds.Add(r.To.Address.ID);
                        foreach (var otherIds in r.To.Address.OtherIds)
                            city.Address.OtherIds.Add(otherIds);
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
                if (Roads[i].From.Address.Address == address || Roads[i].To.Address.Address == address)
                {
                    Roads.Remove(Roads[i]);
                    i--;
                }
            }
        }
        public Road GetRoadBetweenAddresses(string from, string to)
        {
            return Roads.FirstOrDefault(t => (t.From.Address.Address == from && t.To.Address.Address == to) || (t.From.Address.Address == to && t.To.Address.Address == from));
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
            World.MaxOptimationTime = Convert.ToInt32(HTBUtils.GetConfigValue("ACO_MaxOptimationTime_Seconds")) * 1000;
            map.Clear();
            foreach (City city in Cities)
                map.AddCity(city);
            foreach (Road road in Roads)
                map.AddRoad(road);

            var w = map.ConstructTsp();
            int startingIndex = 0;
            for (int i = 0; i < Cities.Count; i++)
            {
                if (Cities[i].Address.Equals(Addresses[0]))
                {
                    startingIndex = i;
                    break;
                }
            }
            w.MaxTravelTime = MaxTravelTime;
            BestTour = w.FindTour(startingIndex);
        }
        protected void CalcBestTourUsingThread(bool isTask = false)
        {
            World.MaxOptimationTime = Convert.ToInt32(HTBUtils.GetConfigValue("ACO_MaxOptimationTime_Seconds")) * 1000;
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
                    var msg = String.Format("Calculating Tour: [Best: {0}] [Time: {1} of {2}]" + dot, (int) _tspBestTourValue, HTBUtils.FormatTimeSpan(_tspRunTime), HTBUtils.FormatTimeSpan(new TimeSpan(0, 0, 0, 0, World.MaxOptimationTime)));
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
            return Cities.Where(city => city.Address.ID == id).FirstOrDefault();
        }
        protected bool ExistsInCities(City c)
        {
            return Cities.Where(city => city.Equals(c)).FirstOrDefault() != null;
        }
        #endregion

        public void UpdateProgressStatus(int pct, string message)
        {
            if(TskStatus != null)
            {
                TskStatus.Progress = pct;
                TskStatus.ProgressText = message;
            }
        }

        public void UpdateProgresLongMessage(string longMessage)
        {
            if (TskStatus != null)
            {
                TskStatus.ProgressTextLong = longMessage;
            }
        }
        
        #region ISerializable
        protected RoutePlanerManager(SerializationInfo info, StreamingContext context)
            : base("")
        {
            Cities = (List<City>)info.GetValue("Cities", typeof(List<City>));
            Roads = (List<Road>)info.GetValue("Roads", typeof(List<Road>));
            Addresses = (List<AddressWithID>)info.GetValue("Addresses", typeof(List<AddressWithID>));
            BadAddresses = (List<AddressWithID>)info.GetValue("BadAddresses", typeof(List<AddressWithID>));
            MultipleLocationsAddresses = (List<AddressWithID>)info.GetValue("MultipleLocationsAddresses", typeof(List<AddressWithID>));
            BestTour = (List<TspCity>)info.GetValue("BestTour", typeof(List<TspCity>));
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
            info.AddValue("BadAddresses", BadAddresses, typeof(List<AddressWithID>));
            info.AddValue("MultipleLocationsAddresses", MultipleLocationsAddresses, typeof(List<AddressWithID>));
            
            info.AddValue("BestTour", BestTour, typeof(List<TspCity>));
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

        public string GetTourString(IEnumerable<TspCity> tour)
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

    #region Addresses
    internal class AddressLookupState
    {
        public readonly AddressWithID Address;
        public readonly ManualResetEvent ManualEvent;
        public readonly int UserId;
        
        public AddressLookupState(AddressWithID addr, ManualResetEvent manualEvent, int userId)
        {
            Address = addr;
            ManualEvent = manualEvent;
            UserId = userId;
        }
    }
    internal class AddressLookupStateStatics
    {
        public readonly List<City> Addresses = new List<City>();
        public readonly List<AddressWithID> BadAddresses = new List<AddressWithID>();
        public readonly List<AddressWithID> MultipleAddresses = new List<AddressWithID>();
        
        public void Clear()
        {
            Addresses.Clear();
            BadAddresses.Clear();
            MultipleAddresses.Clear();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCityToAddresses(City c)
        {
            Addresses.Add(c);
        }
    }

    internal static class AddressLookupStateStaticsAccess
    {
        private static readonly Dictionary<int, AddressLookupStateStatics> AccessDictionary = new Dictionary<int, AddressLookupStateStatics>();
        public static AddressLookupStateStatics GetAddressStateStatic(int id)
        {
            if (AccessDictionary.ContainsKey(id))
                return AccessDictionary[id];

            var value = new AddressLookupStateStatics();
            AccessDictionary.Add(id, value);
            return value;

        }
    }
    internal class AddressLookupThread
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string spName = "spGetGegnerAddressLatAndLgnByAktID";
        public volatile bool IsRunning;
        public volatile bool Stop = false;
        public int ID;
        
        public AddressLookupThread(int id)
        {
            ID = id;
        }
        public void LoadGeocodeAddress(object state)
        {
            IsRunning = true;
            Stop = false;
            var stateInfo = (AddressLookupState)state;
            try
            {
                if (stateInfo.Address.ID > 0 && !Stop)
                {
                    var list = new ArrayList
                           {
                               new StoredProcedureParameter("aktID", SqlDbType.Int, stateInfo.Address.ID),  
                           };
                    var latLgn = (spGetGegnerAddressLatAndLgnByAktID)HTBUtils.GetStoredProcedureSingleRecord(spName, list, typeof(spGetGegnerAddressLatAndLgnByAktID));
                    if (latLgn != null && !HTBUtils.IsZero(latLgn.GegnerLatitude) && !HTBUtils.IsZero(latLgn.GegnerLongitude))
                    {
                        Log.Info(string.Format("Akt: {0}   Lat: {1}   Lgn: {2}", latLgn.AktIntID, latLgn.GegnerLatitude, latLgn.GegnerLongitude));
                        var location = new GeocodeLocation
                                           {
                                               Latitude = latLgn.GegnerLatitude,
                                               Longitude = latLgn.GegnerLongitude
                                           };
//                        lock (typeof(AddressLookup))
//                        {
//                            AddressLookupState.Addresses.Add(new City(new AddressLocation(addressLookupStateInfo.address, new GeocodeLocation[] {location}), null));
//                        }
                        AddressLookupStateStaticsAccess.GetAddressStateStatic(stateInfo.UserId).AddCityToAddresses(new City(new AddressLocation(stateInfo.Address, new GeocodeLocation[] { location }), null));
                    }
                    else
                    {
                        if(!Stop)
                            LoadGeocodeAddressFromWeb(state);
                    }
                }
                else
                {
                    if (!Stop)
                        LoadGeocodeAddressFromWeb(state);
                }
            }
            catch
            {
            }
            IsRunning = false;
        }
        
        /*
         * Try Bing First and then GoogleMaps
         */
        private void LoadGeocodeAddressFromWeb(object state)
        {
            var stateInfo = (AddressLookupState)state;
            var geocodeRequest = new GeocodeRequest
            {
                // Set the credentials using a valid Bing Maps key
                Credentials = new GeocodeService.Credentials { ApplicationId = RoutePlanerManager.BingMapsKey },
                // Set the full address query
                Query = stateInfo.Address.Address
            };

            // Set the options to only return high confidence results 
            var filters = new ConfidenceFilter[1];
            filters[0] = new ConfidenceFilter { MinimumConfidence = GeocodeService.Confidence.High };
            // Add the filters to the options
            geocodeRequest.Options = new GeocodeOptions { Filters = filters };
            // Make the geocode request
            var geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
            GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);
            if (geocodeResponse.Results.Length == 1)
            {
                AddressLookupStateStaticsAccess.GetAddressStateStatic(stateInfo.UserId).Addresses.Add(new City(new AddressLocation(stateInfo.Address, geocodeResponse.Results[0].Locations), null));
                SaveGegnerLocation(stateInfo.Address.ID, geocodeResponse.Results[0].Locations[0].Latitude, geocodeResponse.Results[0].Locations[0].Longitude);
            }
            else if (geocodeResponse.Results.Length > 1)
            {
                foreach (GeocodeResult r in geocodeResponse.Results)
                    stateInfo.Address.SuggestedAddresses.Add(new AddressLocation(new AddressWithID(stateInfo.Address.ID, r.DisplayName), r.Locations));
                AddressLookupStateStaticsAccess.GetAddressStateStatic(stateInfo.UserId).MultipleAddresses.Add(stateInfo.Address);
            }
            else
            {
                LoadGeocodeAddressFromGoogleMaps(state);
            }
        }

        private void LoadGeocodeAddressFromGoogleMaps(object state)
        {

            var stateInfo = (AddressLookupState)state;
            var request = new GeocodingRequest
                              {
                                  Address = stateInfo.Address.Address,
                                  Sensor = "false"
                              };
            var response = GeocodingService.GetResponse(request);
            if (response.Status == ServiceResponseStatus.Ok)
            {
                if (response.Results == null || response.Results.Length <= 0)
                {
                    AddressLookupStateStaticsAccess.GetAddressStateStatic(stateInfo.UserId).BadAddresses.Add(stateInfo.Address);
                }
                else
                {
                    var location = new GeocodeLocation
                                       {
                                           Latitude = Convert.ToDouble(response.Results[0].Geometry.Location.Latitude), 
                                           Longitude = Convert.ToDouble(response.Results[0].Geometry.Location.Longitude)
                                       };
                    AddressLookupStateStaticsAccess.GetAddressStateStatic(stateInfo.UserId).Addresses.Add(new City(new AddressLocation(stateInfo.Address, new GeocodeLocation[] { location }), null));
                    SaveGegnerLocation(stateInfo.Address.ID, location.Latitude, location.Longitude);
                }
            }
            else
            {
                AddressLookupStateStaticsAccess.GetAddressStateStatic(stateInfo.UserId).BadAddresses.Add(stateInfo.Address);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SaveGegnerLocation(int aktId, double lat, double lgn)
        {
            if (aktId > 0 && !Stop)
            {
                var akt = HTBUtils.GetInterventionAktQry(aktId);
                if (akt != null)
                {
                    var set = new RecordSet();
                    set.ExecuteNonQuery("UPDATE tblGegner SET GegnerLatitude = " + lat.ToString().Replace(",", ".") + ", GegnerLongitude = " + lgn.ToString().Replace(",", ".") + " WHERE GegnerID = " + akt.GegnerID);
                }
            }
        }
    }

    internal static class AddressLookupThreadMonitor
    {
        public static readonly StringBuilder AddressLookupUpdateCommands = new StringBuilder();
        public static int NumberOfSQLCommands { get; set; }

        public static void LoadGeocodeAddress(object state, AddressLookupThread thread)
        {
            thread.IsRunning = true;
            thread.LoadGeocodeAddress(state);
        }
        public static int GetRunningThreadsCount(IEnumerable<AddressLookupThread> threads)
        {
            return threads.Count(thread => thread.IsRunning);
        }
        public static bool HasRunningThreads(IEnumerable<AddressLookupThread> threads)
        {
            return threads.Any(thread => thread.IsRunning);
        }

        public static List<AddressLookupThread> GetRunningThreads(IEnumerable<AddressLookupThread> threads)
        {
            return threads.Where(thread => thread.IsRunning).ToList();
        }

        public static void AbortAllThreads(IEnumerable<AddressLookupThread> threads, bool setIsRunningToFalse = false)
        {
            foreach (var thread in threads.Where(thread => thread.IsRunning))
            {
                thread.Stop = true;
                if (setIsRunningToFalse)
                    thread.IsRunning = false;
            }
        }
    }
    #endregion

    #region Distance
    internal class RoadDistanceState
    {
        public Road road;
        public ManualResetEvent ManualEvent;
        public int UserId;
        
        public RoadDistanceState(int userId, Road rd, ManualResetEvent manualEvent)
        {
            UserId = userId;
            road = rd;
            ManualEvent = manualEvent;
        }
    }

    internal class RoadDistanceStateStatics
    {
        public readonly ArrayList ResultsList = new ArrayList();
        public readonly StringBuilder DistanceUpdateCommands = new StringBuilder();
    }

    internal static class RoadDistanceStateStaticsAccess
    {
        private static readonly Dictionary<int, RoadDistanceStateStatics> AccessDictionary = new Dictionary<int, RoadDistanceStateStatics>();
        public static RoadDistanceStateStatics GetDistanceStatestatic(int id)
        {
            if(AccessDictionary.ContainsKey(id))
                return AccessDictionary[id];
            
            var value = new RoadDistanceStateStatics();
            AccessDictionary.Add(id, value);
            return value;
            
        }
    }

    internal class RoadDistanceCalculatorThread
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public volatile bool IsRunning = false;
        public volatile bool Stop = false;
        public int ID;
        public static object lockObj = new object();
        public RoadDistanceCalculatorThread(int id)
        {
            ID = id;
        }
        public void LoadDistances(object state)
        {
            IsRunning = true;
            Stop = false;
            var stateInfo = (RoadDistanceState)state;
            try
            {
                if (stateInfo.road.From != null &&
                    stateInfo.road.From != null &&
                    stateInfo.road.From.Location != null &&
                    stateInfo.road.From.Location.Locations != null &&
                    stateInfo.road.From.Location.Locations.Length > 0 &&

                    stateInfo.road.To != null &&
                    stateInfo.road.To.Location != null &&
                    stateInfo.road.To.Location.Locations != null &&
                    stateInfo.road.To.Location.Locations.Length > 0)
                    
                    if(!Stop)
                        SetDistance(stateInfo.road, 3);
            }
            catch(ThreadAbortException)
            {
                Log.Info("Cought ThreadAbortException [LoadDistances]");
                IsRunning = false;
                return;
            }
            catch(Exception ex)
            {
                Console.WriteLine("GRRR: "+ex.Message);
                Log.Error(ex);
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void SetDistance(Road road, int tries)
        {
            string qry = "SELECT * FROM {0}tblRoad WHERE (FromLatitude = {1} AND FromLongitude = {2} AND ToLatitude = {3} AND ToLongitude = {4})";
            var query = "";
            if (road.From.Location.Locations[0].Latitude >= road.To.Location.Locations[0].Latitude)

                query = String.Format(qry, 
                                        RecordSet.GetDB2SchemaName()
                                        ,road.From.Location.Locations[0].Latitude.ToString().Replace(",", "")
                                        ,road.From.Location.Locations[0].Longitude.ToString().Replace(",", "")
                                        ,road.To.Location.Locations[0].Latitude.ToString().Replace(",", "")
                                        ,road.To.Location.Locations[0].Longitude.ToString().Replace(",", "")
                                        );
            else

                query = String.Format(qry,
                                        RecordSet.GetDB2SchemaName()
                                        , road.To.Location.Locations[0].Latitude.ToString().Replace(",", "")
                                        , road.To.Location.Locations[0].Longitude.ToString().Replace(",", "")
                                        , road.From.Location.Locations[0].Latitude.ToString().Replace(",", "")
                                        , road.From.Location.Locations[0].Longitude.ToString().Replace(",", "")
                                        );
            var roadDistance = (tblRoad)HTBUtils.GetSqlSingleRecord(query, typeof(tblRoad), DbConnection.ConnectionType_DB2);
            
            if (roadDistance == null)
            {
                if (!Stop)
                    SetGeocodeDistance(road, tries);
            }
            // Lookup distance online if older than 6 months (maybe roads changed...)
            else if (DateTime.Now.Subtract(roadDistance.LookupDate).TotalDays > 180)
            {
                if (!Stop)
                    SetGeocodeDistance(road, tries, true);
            }
            else
            {
                if (!Stop)
                {
                    road.Distance = roadDistance.Distance;
                    road.TravelTimeInSeconds = roadDistance.TimeInSeconds;
                }
            }
        }

        private void SetGeocodeDistance(Road road, int tries, bool updateDistance = false)
        {
            try
            {
                var routeRequest = new RouteRequest
                {
                    Credentials = new RouteService.Credentials
                    {
                        // Set the credentials using a valid Bing Maps key
                        ApplicationId = RoutePlanerManager.BingMapsKey
                    }
                };
                //Parse user data to create array of waypoints

                var waypoints = new Waypoint[2];

                waypoints[0] = new Waypoint { Location = new RouteService.Location { Latitude = road.From.Location.Locations[0].Latitude, Longitude = road.From.Location.Locations[0].Longitude } };
                waypoints[1] = new Waypoint { Location = new RouteService.Location { Latitude = road.To.Location.Locations[0].Latitude, Longitude = road.To.Location.Locations[0].Longitude } };
                Log.Info(string.Format("Setting Geocode Distance FROM: Latitude {0}, Longitude {1}", road.From.Location.Locations[0].Latitude, road.From.Location.Locations[0].Longitude));
                routeRequest.Waypoints = waypoints;
                routeRequest.Options = new RouteOptions { Optimization = RouteOptimization.MinimizeDistance };
                
                // Make the calculate route request
                var routeService = new RouteServiceClient("BasicHttpBinding_IRouteService");
                if (!Stop)
                {
                    RouteResponse routeResponse = routeService.CalculateRoute(routeRequest);
                    road.Distance = routeResponse.Result.Summary.Distance;
                    road.TravelTimeInSeconds = routeResponse.Result.Summary.TimeInSeconds;
                    if (updateDistance)
                    {
                        if (!Stop)
                            UpdateDistance(road);
                    }
                    else
                    {
                        if (!Stop)
                            InsertDistance(road);
                    }
                }
            }
            catch (ThreadAbortException tae)
            {
                Log.Error("Cought ThreadAbortException [SetGeocodeDistance]");
                IsRunning = false;
                return;
            }
            catch (Exception e)
            {
//                Console.WriteLine("Trying to Recalculate Distance " + tries);
                if (!Stop)
                {
                    if (tries > 0)
                    {
                        SetGeocodeDistance(road, tries - 1, updateDistance);
                    }
                    Log.Error(string.Format("Could Not Calculate Distance [FROM: {0}, {1}] TO [{2}, {3}] " +
                        e.Message, 
                        road.From.Location.Locations[0].Latitude.ToString().Replace(",", "."), 
                        road.From.Location.Locations[0].Longitude.ToString().Replace(",", "."),
                        road.To.Location.Locations[0].Latitude.ToString().Replace(",", "."), 
                        road.To.Location.Locations[0].Longitude.ToString().Replace(",", "."))
                        , e);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void InsertDistance(Road road)
        {
            if (!Stop)
            {
                var rec = new tblRoad
                              {
                                  Distance = road.Distance,
                                  TimeInSeconds = road.TravelTimeInSeconds,
                                  LookupDate = DateTime.Now
                              };
                if (road.From.Location.Locations[0].Latitude >= road.To.Location.Locations[0].Latitude)
                {
                    rec.FromLatitude = RoutePlanerManager.GetIntFromCoordinate(road.From.Location.Locations[0].Latitude);
                    rec.FromLongitude = RoutePlanerManager.GetIntFromCoordinate(road.From.Location.Locations[0].Longitude);
                    rec.ToLatitude = RoutePlanerManager.GetIntFromCoordinate(road.To.Location.Locations[0].Latitude);
                    rec.ToLongitude = RoutePlanerManager.GetIntFromCoordinate(road.To.Location.Locations[0].Longitude);
                }
                else
                {
                    rec.FromLatitude = RoutePlanerManager.GetIntFromCoordinate(road.To.Location.Locations[0].Latitude);
                    rec.FromLongitude = RoutePlanerManager.GetIntFromCoordinate(road.To.Location.Locations[0].Longitude);
                    rec.ToLatitude = RoutePlanerManager.GetIntFromCoordinate(road.From.Location.Locations[0].Latitude);
                    rec.ToLongitude = RoutePlanerManager.GetIntFromCoordinate(road.From.Location.Locations[0].Longitude);
                }
                //AppendToUpdateDistanceCommand(new RecordSet(DbConnection.ConnectionType_DB2).GetDbInsertStatement(rec));
                AppendToUpdateDistanceCommand(rec);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void UpdateDistance(Road road)
        {
            if (!Stop)
            {
                var rec = new tblRoad
                              {
                                  Distance = road.Distance,
                                  TimeInSeconds = road.TravelTimeInSeconds,
                                  LookupDate = DateTime.Now
                              };
                if (road.From.Location.Locations[0].Latitude >= road.To.Location.Locations[0].Latitude)
                {
                    rec.FromLatitude = RoutePlanerManager.GetIntFromCoordinate(road.From.Location.Locations[0].Latitude);
                    rec.FromLongitude = RoutePlanerManager.GetIntFromCoordinate(road.From.Location.Locations[0].Longitude);
                    rec.ToLatitude = RoutePlanerManager.GetIntFromCoordinate(road.To.Location.Locations[0].Latitude);
                    rec.ToLongitude = RoutePlanerManager.GetIntFromCoordinate(road.To.Location.Locations[0].Longitude);
                }
                else
                {
                    rec.FromLatitude = RoutePlanerManager.GetIntFromCoordinate(road.To.Location.Locations[0].Latitude);
                    rec.FromLongitude = RoutePlanerManager.GetIntFromCoordinate(road.To.Location.Locations[0].Longitude);
                    rec.ToLatitude = RoutePlanerManager.GetIntFromCoordinate(road.From.Location.Locations[0].Latitude);
                    rec.ToLongitude = RoutePlanerManager.GetIntFromCoordinate(road.From.Location.Locations[0].Longitude);
                }
                AppendToUpdateDistanceCommand(rec);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void AppendToUpdateDistanceCommand(tblRoad road)
        {

            // TOTO incorporate the following statement:
            var command = string.Format("MERGE INTO {0}tblRoad AS mt USING ( " +
                                        "SELECT * FROM TABLE ( " +
                                        "   VALUES  " +
                                        "        ( {1}, {2}, {3}, {4}, {5}, {6}, '{7}') " +
                                        ") " +
                                        ") AS vt(FromLatitude, FromLongitude, ToLatitude, ToLongitude, TimeInSeconds, Distance, LookupDate) ON (" +
                                        "(mt.FromLatitude = vt.FromLatitude AND mt.FromLongitude = vt.FromLongitude AND mt.ToLatitude = vt.ToLatitude AND mt.ToLongitude = vt.ToLongitude) " +
                                        "OR  (mt.ToLatitude = vt.FromLatitude AND mt.ToLongitude = vt.FromLongitude AND mt.FromLatitude = vt.ToLatitude AND mt.FromLongitude = vt.ToLongitude) " +
                                        ") " +
                                        "WHEN MATCHED THEN " +
                                        "    UPDATE SET LookupDate = vt.LookupDate, Distance = vt.Distance " +
                                        "WHEN NOT MATCHED THEN " +
                                        "    INSERT (FromLatitude, FromLongitude, ToLatitude, ToLongitude, TimeInSeconds, Distance, LookupDate) VALUES (vt.FromLatitude, vt.FromLongitude, vt.ToLatitude, vt.ToLongitude, vt.TimeInSeconds, vt.Distance, vt.LookupDate) " +
                                        "; ", 
                                        RecordSet.GetDB2SchemaName(),
                                        road.FromLatitude,
                                        road.FromLongitude,
                                        road.ToLatitude,
                                        road.ToLongitude,
                                        road.TimeInSeconds,
                                        road.Distance,
                                        string.Format("{0:u}", road.LookupDate).Replace("Z", ""));
            
            if (Stop) return;
            if (RoadDistanceCalculatorThreadMonitor.DistanceUpdateCommands.ToString().IndexOf(command, StringComparison.Ordinal) < 0)
            {
                if (!Stop)
                {
                    lock (lockObj)
                    {
                        RoadDistanceCalculatorThreadMonitor.DistanceUpdateCommands.Append(command);
                        RoadDistanceCalculatorThreadMonitor.NumberOfSQLCommands++;
                    }
                }
            }

            if (Stop) return;

            
            if (RoadDistanceCalculatorThreadMonitor.NumberOfSQLCommands < 100) return;
            try
            {
                lock (lockObj)
                {
                    var set = new RecordSet(DbConnection.ConnectionType_DB2);
                    if (!Stop)
                        set.ExecuteNonQuery(RoadDistanceCalculatorThreadMonitor.DistanceUpdateCommands.ToString());
                }
            }
            catch (ThreadAbortException tae)
            {
                Log.Error("Cought ThreadAbortException [AppendToUpdateDistanceCommand]", tae);
                IsRunning = false;
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                Log.Error(RoadDistanceCalculatorThreadMonitor.DistanceUpdateCommands.ToString());
            }
            finally
            {
                lock (lockObj)
                {
                    RoadDistanceCalculatorThreadMonitor.DistanceUpdateCommands.Clear();
                    RoadDistanceCalculatorThreadMonitor.NumberOfSQLCommands = 0;
                }
            }
        }

        private tblRoad GetSwitchedFromAndToRoad(tblRoad rec)
        {

            return new tblRoad
            {
                ToLatitude = rec.FromLatitude,
                ToLongitude = rec.FromLongitude,
                FromLatitude = rec.ToLatitude,
                FromLongitude = rec.ToLongitude,
                Distance = rec.Distance,
                LookupDate = rec.LookupDate,
                TimeInSeconds = rec.TimeInSeconds,
            };
        }
        public void Kill()
        {
            
        }
    }

    internal static class RoadDistanceCalculatorThreadMonitor
    {
        public static readonly StringBuilder DistanceUpdateCommands = new StringBuilder();
        public static int NumberOfSQLCommands { get; set; }
        
        public static void LoadDistances(object state, RoadDistanceCalculatorThread thread)
        {
            thread.IsRunning = true;
            thread.LoadDistances(state);
        }
        public static int GetRunningThreadsCount(IEnumerable<RoadDistanceCalculatorThread> threads)
        {
            return threads.Count(thread => thread.IsRunning);
        }
        public static bool HasRunningThreads(IEnumerable<RoadDistanceCalculatorThread> threads)
        {
            return threads.Any(thread => thread.IsRunning);
        }

        public static List<RoadDistanceCalculatorThread> GetRunningThreads(IEnumerable<RoadDistanceCalculatorThread> threads)
        {
            return threads.Where(thread => thread.IsRunning).ToList();
        }

        public static void AbortAllThreads(IEnumerable<RoadDistanceCalculatorThread> threads, bool setIsRunningToFalse = false)
        {
            foreach (var thread in threads.Where(thread => thread.IsRunning))
            {
                thread.Stop = true;
                if (setIsRunningToFalse)
                    thread.IsRunning = false;
            }
        }
    }
    #endregion

    #region Map
    internal class MapState
    {
        public readonly List<City> Cities;
        public readonly List<Road> Roads;
        public readonly Map Map;
        public readonly List<AddressWithID> Addresses;
        public readonly ManualResetEvent ManualEvent;
        public int NumerOfThreadsNotYetCompleted;
        public List<TspCity> BestTour;
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
            w.AddTspProgressListener(mapStateInfo.rpManager);
            w.UserID = mapStateInfo.UserId;
//            w.Update += UpdateInfo;
            int startingIndex = 0;
            for (int i = 0; i < mapStateInfo.Cities.Count; i++)
            {
                if (mapStateInfo.Cities[i].Address.Equals(mapStateInfo.Addresses[0]))
                {
                    startingIndex = i;
                    break;
                }
            }
            w.MaxTravelTime = mapStateInfo.MaxTravelTime;
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

        private static void UpdateInfo(World sender, UpdateEventArgs e)
        {
            if (e != null)
            {
                if (e.BestTour != null)
                {
                    MapStateStaticsAccess.GetMapStateStatic(e.UserID).RpManager.UpdateProgresLongMessage(GetTourInfo(e.BestTour, e.UserID));
                }
                if (MapStateStaticsAccess.GetMapStateStatic(e.UserID).RpManager.IsRouteCalculationDone)
                {
                    sender.Update -= UpdateInfo;
                }
            }
        }

        private static String GetTourInfo(IEnumerable<TspCity> tour, int userId)
        {
            return MapStateStaticsAccess.GetMapStateStatic(userId).RpManager.GetTourString(tour);
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
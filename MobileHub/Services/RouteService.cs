using System;
using RouteModel;
using DataAccessLayer.Managers.Route;
using System.Collections.Generic;
using Utilities;

namespace Services
{
    public class RouteService : IRouteService, IDisposable
    {
        private readonly RoadInfoDataManager _roadInfoManager = new RoadInfoDataManager();

        public RoadInfo GetRoadInfo(int fromLatitude, int fromLongitude, int toLatitude, int toLongitude)
        {
            return _roadInfoManager.GetRoadInfo(fromLatitude, toLatitude, fromLongitude, toLongitude);
        }

        public List<RoadInfo> GetAllRoadInfos()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _roadInfoManager.Dispose();
        }

        #region Road Distance


        public void LoadDistances(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            try
            {
                SetDistance(fromLatitude, fromLongitude, toLatitude, toLongitude, 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GRRR: " + ex.Message);
            }
        }

        private void SetDistance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, int tries)
        {

            var roadDistance = _roadInfoManager.GetRoadInfo(
                GeoCodeConverter.ToInteger(fromLatitude),
                GeoCodeConverter.ToInteger(fromLongitude),
                GeoCodeConverter.ToInteger(toLatitude),
                GeoCodeConverter.ToInteger(toLongitude));

            if (roadDistance == null)
            {
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

        private void SetGeocodeDistance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, int tries, bool updateDistance = false)
        {
            try
            {
                var query = string.Format("{0}, {1}, {2}, {3}", address.Street, address.Zip, address.City, address.Country.Name);
                Uri geocodeRequest = new Uri(string.Format("http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}", query, CommonConfigValues.BingMapsKey));
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
        private void InsertRoadInfo(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, double distance, int travelTimeInSeconds)
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

        private void UpdateRoadInfo(Road road)
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
            catch (Exception ex)
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
        #endregion
    }
}

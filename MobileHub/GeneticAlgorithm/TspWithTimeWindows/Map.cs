using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace TspWithTimeWindows
{

    public class Map
    {
        private static readonly string[] m_PushPinColors = new string[]
        {
            "red",
            "green",
            "blue",
        };

        private readonly Random random = new Random();

        public const string BingJsStart = "directionsManager.addWaypoint(new Microsoft.Maps.Directions.Waypoint({ location: new Microsoft.Maps.Location(";
        public const string BingJsEnd = "), exactLocation: true })); ";

        private List<City> m_Cities = new List<City>();
        private List<Road> m_Roads = new List<Road>();
        private Dictionary<City, City> m_CityMap = new Dictionary<City, City>();
        

        public Map()
        {
        }

        public void AddCity(City city)
        {
            m_Cities.Add(city);
        }

        public int CityCount => m_Cities.Count;

        public void AddRoad(Road road)
        {
            m_Roads.Add(road);
        }

        public City FindCity(AddressLocation location)
        {
            return m_Cities.Find(c => c.Location.Address == location.Address);
        }

        public void Clear()
        {
            m_Cities.Clear();
            m_Roads.Clear();
        }

        public World ConstructTsp(bool returnToStartingCity = true)
        {
            var wb = new WorldBuilder();

            m_CityMap.Clear();
            m_CityMap = new Dictionary<City, City>(m_Cities.Count);

            foreach (var c in m_Cities)
            {
                c.ClearRoads();
                wb.AddCity(c);
                m_CityMap.Add(c, c);
            }
            int cnt = 0;
            foreach (var road in m_Roads)
            {
                try
                {
                    wb.AddRoad(road.Distance, road.Duration, road.From, road.To);
                    cnt++;
                }
                catch(Exception ex)
                {
                    int i = 0;
                }
            }

            return new World(wb);
        }

        public string GetTourString(IEnumerable<City> tour)
        {
            if (tour == null)
            {
                return null;
            }

            return PrintTour(tour);
        }

        public String GetProgressTour(IEnumerable<City> tour)
        {
            var sb = new StringBuilder();
            var i = tour.GetEnumerator();
            int counter = 1;
            while (i.MoveNext())
            {
                sb.Append(m_CityMap[i.Current].Location.Address.ID);
                sb.Append(" > ");
                //                if (counter++ % 10 == 0)
                //                    sb.Append(" <br/> ");                
            }
            return sb.ToString();
        }

        private String PrintTour(IEnumerable<City> tour)
        {
            var sb = new StringBuilder();
            var i = tour.GetEnumerator();
            i.MoveNext();
            var first = m_CityMap[i.Current];
            var c1 = first;

            while (i.MoveNext())
            {
                var c2 = m_CityMap[i.Current];
                sb.Append(c1.Location.Address.ID);
                sb.Append(" : ");
                sb.Append(c1.Location.Address.Address);
                sb.Append(" =====> ");
                sb.Append(c2.Location.Address.ID);
                sb.Append(" : ");
                sb.Append(c2.Location.Address.Address);
                sb.Append(" <BR> ");
                c1 = c2;
            }
            sb.Append(" LASTLY: ");
            sb.Append(c1.Location.Address.ID);
            sb.Append(" : ");
            sb.Append(c1.Location.Address.Address);
            sb.Append(" =====> ");
            sb.Append(first.Location.Address.ID);
            sb.Append(" : ");
            sb.Append(first.Location.Address.ID);
            sb.Append(" <BR> ");
            return sb.ToString();
        }

        public String GetJsWaypoints(IEnumerable<City> tour, int startPoint, int totalPoints)
        {
            var sb = new StringBuilder();
            var i = tour.GetEnumerator();
            int idx = 1;
            i.MoveNext();

            //            DebugTourAndCity(tour, "GetJsWaypoints");

            var first = m_CityMap[i.Current];
            var c1 = first;

            if (idx >= startPoint && idx <= startPoint + totalPoints)
            {
                sb.Append(GetAddressLocationOnTheMap(c1, idx, "green"));
            }

            while (i.MoveNext())
            {
                idx++;
                if (idx >= startPoint && idx <= startPoint + totalPoints)
                {
                    c1 = m_CityMap[i.Current];
                    sb.Append(idx == startPoint + totalPoints
                        ? GetAddressLocationOnTheMap(c1, idx, "red")
                        : GetAddressLocationOnTheMap(c1, idx));
                }
            }
            return sb.ToString();
        }

        public String GetJsWaypoints_OLD(IEnumerable<City> tour, int startPoint, int totalPoints)
        {
            var sb = new StringBuilder();
            var i = tour.GetEnumerator();
            int idx = 1;
            i.MoveNext();

            //            DebugTourAndCity(tour, "GetJsWaypoints");

            var first = m_CityMap[i.Current];
            var c1 = first;

            if (idx >= startPoint && idx <= startPoint + totalPoints)
            {
                sb.Append(BingJsStart);
                sb.Append(c1.Location.Locations[0].Latitude.ToString().Replace(",", "."));
                sb.Append(", ");
                sb.Append(c1.Location.Locations[0].Longitude.ToString().Replace(",", "."));
                sb.Append(BingJsEnd);
                sb.Append("  //");
                sb.Append(c1.Location.Address);
                sb.Append(Environment.NewLine);
            }

            while (i.MoveNext())
            {
                idx++;
                if (idx >= startPoint && idx <= startPoint + totalPoints)
                {
                    c1 = m_CityMap[i.Current];
                    sb.Append(BingJsStart);
                    sb.Append(c1.Location.Locations[0].Latitude.ToString().Replace(",", "."));
                    sb.Append(", ");
                    sb.Append(c1.Location.Locations[0].Longitude.ToString().Replace(",", "."));
                    sb.Append(BingJsEnd);
                    sb.Append("  //");
                    sb.Append(c1.Location.Address);
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        public String GetJsOverwiewWaypoints(IEnumerable<City> tour, int totalPoints)
        {
            var sb = new StringBuilder();
            var totalCount = tour.Count();
            var count = 0;
            var i = tour.GetEnumerator();

            int idx = 1;
            i.MoveNext();

            var first = m_CityMap[i.Current];
            var c1 = first;

            sb.Append(GetAddressLocationOnTheMap(c1, idx, "green"));

            count = 1;

            while (i.MoveNext())
            {
                idx++;
                if (idx % (totalCount / totalPoints) == 0 || idx == totalCount)
                {
                    if (count == totalCount - 1 && idx < totalCount)
                    {
                        continue;
                    }
                    count++;
                    sb.Append(idx == totalCount
                        ? GetAddressLocationOnTheMap(c1, idx, "red")
                        : GetAddressLocationOnTheMap(c1, idx));
                }
            }
            return sb.ToString();
        }

        public String GetJsOverwiewWaypoints_OLD(IEnumerable<City> tour, int totalPoints)
        {
            var sb = new StringBuilder();
            var totalCount = tour.Count();
            var count = 0;
            var i = tour.GetEnumerator();

            int idx = 1;
            i.MoveNext();

            var first = m_CityMap[i.Current];
            var c1 = first;

            sb.Append(BingJsStart);
            sb.Append(c1.Location.Locations[0].Latitude.ToString().Replace(",", "."));
            sb.Append(", ");
            sb.Append(c1.Location.Locations[0].Longitude.ToString().Replace(",", "."));
            sb.Append(BingJsEnd);
            sb.Append("  //");
            sb.Append(c1.Location.Address);
            sb.Append(Environment.NewLine);
            count = 1;

            while (i.MoveNext())
            {
                idx++;
                if (idx % (totalCount / totalPoints) == 0 || idx == totalCount)
                {
                    if (count == totalCount - 1 && idx < totalCount)
                    {
                        continue;
                    }
                    count++;
                    c1 = m_CityMap[i.Current];
                    sb.Append(BingJsStart);
                    sb.Append(c1.Location.Locations[0].Latitude.ToString().Replace(",", "."));
                    sb.Append(", ");
                    sb.Append(c1.Location.Locations[0].Longitude.ToString().Replace(",", "."));
                    sb.Append(BingJsEnd);
                    sb.Append("  //");
                    sb.Append(c1.Location.Address);
                    sb.Append(Environment.NewLine);

                }
            }
            return sb.ToString();
        }

        public String GetJsReversedWaypoints(IEnumerable<City> tour, int startPoint, int totalPoints)
        {
            return GetJsWaypoints(GetReversedTour(tour), startPoint, totalPoints);
        }

        public String GetJsReversedOverwiewWaypoints(IEnumerable<City> tour, int totalPoints)
        {
            return GetJsOverwiewWaypoints(GetReversedTour(tour), totalPoints);
        }

        private IEnumerable<City> GetReversedTour(IEnumerable<City> tour)
        {
            List<City> original = GetAllWaypoints(tour);
            var reversed = new List<City>
                               {
                                   original[0]
                               };
            for (int i = original.Count - 1; i > 0; i--)
            {
                reversed.Add(original[i]);
            }
            return reversed;
        }

        public List<City> GetAllWaypoints(IEnumerable<City> tour)
        {
            var list = new List<City>();
            var i = tour.GetEnumerator();

            while (i.MoveNext())
                list.Add(m_CityMap[i.Current]);

            return list;
        }

        public List<int> GetAllWaypointIDs(IEnumerable<City> tour, bool isReverse)
        {
            var list = new List<int>();
            var i = isReverse ? GetReversedTour(tour).GetEnumerator() : tour.GetEnumerator();

            while (i.MoveNext())
                list.Add(m_CityMap[i.Current].Location.Address.ID);

            return list;
        }

        public List<int> GetAllIDs(IEnumerable<City> tour, bool isReverse)
        {
            var list = new List<int>();
            var i = isReverse ? GetReversedTour(tour).GetEnumerator() : tour.GetEnumerator();

            while (i.MoveNext())
            {
                list.Add(m_CityMap[i.Current].Location.Address.ID);
                list.AddRange(m_CityMap[i.Current].Location.Address.OtherIds);
            }


            return list;
        }

        public List<AddressWithID> GetTourAddresses(IEnumerable<City> tour, bool isReverse)
        {
            var list = new List<AddressWithID>();
            var i = isReverse ? GetReversedTour(tour).GetEnumerator() : tour.GetEnumerator();

            while (i.MoveNext())
                list.Add(m_CityMap[i.Current].Location.Address);

            return list;
        }

        private string GetAddressLocationOnTheMap(City city, int idx, string pinColor = "blue")
        {
            var sb = new StringBuilder();

            //lock (lockObj)
            {

                sb.Append($"var location{idx} = new Microsoft.Maps.Location({city.Location.Locations[0].Latitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".")}, {city.Location.Locations[0].Longitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".")});");
                sb.Append(Environment.NewLine);
                //sb.Append("var pin" + idx + " = new Microsoft.Maps.Pushpin(location" + idx + ", { text: '" + idx + "', color: '" + $"#{random.Next(0x1000000):X6}" + "'});");
                sb.Append("var pin" + idx + " = new Microsoft.Maps.Pushpin(location" + idx + ", { text: '" + idx + "', color: '" + pinColor + "'});");
                sb.Append(Environment.NewLine);
                //sb.Append("var wayPoint" + idx + " = new Microsoft.Maps.Directions.Waypoint({ address: '" + city.Address.Address + "' });");
                sb.Append("var wayPoint" + idx + " = new Microsoft.Maps.Directions.Waypoint({ location: location" + idx + ",  address: '" + city.Location.Address.Address + "' });");
                sb.Append(Environment.NewLine);
                sb.Append($"map.entities.push(pin{idx});");
                sb.Append($"directionsManager.addWaypoint (wayPoint{idx});");
            }
            return sb.ToString();
        }
        #region ISerializable
        //note: this is private to control access;
        //the serializer can still access this constructor
        private Map(SerializationInfo info, StreamingContext ctxt)
        {
            m_Cities = (List<City>)info.GetValue("Cities", typeof(List<City>));
            m_Roads = (List<Road>)info.GetValue("Roads", typeof(List<Road>));
            m_CityMap = (Dictionary<City, City>)info.GetValue("CityMap", typeof(Dictionary<City, City>));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Cities", m_Cities, typeof(List<City>));
            info.AddValue("Roads", m_Roads, typeof(List<Road>));
            info.AddValue("CityMap", m_CityMap, typeof(Dictionary<City, City>));
        }
        #endregion

        public void DebugTourAndCityX(IEnumerable<City> tour, string msg)
        {
            string fname = "C:/temp/MapTourAndCityLog.txt";
            var i = tour.GetEnumerator();
            i.MoveNext();

            File.AppendAllText(fname, msg + " start" + Environment.NewLine + Environment.NewLine);

            File.AppendAllText(fname, i == null ? "i is null" : "i is NOT null" + Environment.NewLine + Environment.NewLine);
            File.AppendAllText(fname, i.Current == null ? "i.Current is null" : "i.Current is NOT null" + Environment.NewLine + Environment.NewLine);
            File.AppendAllText(fname, m_CityMap == null ? "m_CityMap is null" : "m_CityMap is NOT null COUNT " + m_CityMap.Count + Environment.NewLine + Environment.NewLine);

            File.AppendAllText(fname, msg + " END" + Environment.NewLine + Environment.NewLine);
        }

        public static void DebugInfoX(string msg)
        {
            string fname = "C:/temp/MapLog.txt";
            File.AppendAllText(fname, msg + Environment.NewLine);
        }

        public static void DebugTourX(IEnumerable<City> tour, string msg)
        {
            string fname = "C:/temp/MapTourLog.txt";
            var i = tour.GetEnumerator();
            i.MoveNext();

            File.AppendAllText(fname, msg + " start" + Environment.NewLine + Environment.NewLine);

            File.AppendAllText(fname, i == null ? "i is null" : "i is NOT null" + Environment.NewLine + Environment.NewLine);
            File.AppendAllText(fname, i.Current == null ? "i.Current is null" : "i.Current is NOT null" + Environment.NewLine + Environment.NewLine);

            File.AppendAllText(fname, msg + " END" + Environment.NewLine + Environment.NewLine);
        }
    }
}

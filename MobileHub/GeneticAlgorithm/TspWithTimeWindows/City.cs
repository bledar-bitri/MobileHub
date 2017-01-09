using System.Collections.Generic;
using System.Diagnostics;

namespace TspWithTimeWindows
{
    [DebuggerDisplay("City {Name} {Id}")]
    public class City
    {
        public const int Radius = 6;

        public int Id { get; set; }

        public AddressLocation Location { get; }

        public string Name { get; }

        public int DesiredArivalTime { get; set; }

        public int ArivalTime { get; set; }

        private readonly Dictionary<City, Road> _mRoads;

        public IEnumerable<City> NeighbourCities => _mRoads.Keys;

        public City(AddressLocation location, string name)
        {
            Location = location;
            Name = name;
            _mRoads = new Dictionary<City, Road>();
        }
        
        public Road GetRoad(City to)
        {
            Road ret;
            _mRoads.TryGetValue(to, out ret);
            return ret;
        }

        internal void AddRoad(Road road, City otherCity)
        {
            _mRoads.Add(otherCity, road);
        }

        public Road Roads(City to)
        {
            Road ret;
            _mRoads.TryGetValue(to, out ret);
            return ret;
        }

        public void ClearRoads()
        {
            _mRoads.Clear();
        }
    }
}

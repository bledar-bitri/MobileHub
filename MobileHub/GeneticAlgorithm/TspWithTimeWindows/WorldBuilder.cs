using System;
using System.Collections.Generic;

namespace TspWithTimeWindows
{
    public class WorldBuilder
    {

        private readonly Dictionary<string, City> m_Cities = new Dictionary<string, City>();

        private readonly List<Road> m_Roads = new List<Road>();
        internal IEnumerable<City> Cities => m_Cities.Values;

        internal IEnumerable<Road> Roads
        {
            get { return m_Roads; }
        }

        public void AddCity(City city)
        {
            m_Cities.Add(city.Name, city);
        }

        public void AddCities(IEnumerable<City> cities)
        {
            foreach (var city in cities)
            {
                AddCity(city);
            }
        }

        public void AddCities(params City[] cities)
        {
            foreach (var city in cities)
            {
                AddCity(city);
            }
        }

        public Road AddRoad(double distance, double duration, City from, City to)
        {
            var road = new Road(from, to)
            {
                Distance = distance,
                Duration = duration
            };
            try
            {
                from.AddRoad(road, to);
            }
            catch (ArgumentException)
            {
            }
            try
            {
                to.AddRoad(road, from);
            }
            catch (ArgumentException)
            {
            }
            try
            {
                m_Roads.Add(road);
            }
            catch (ArgumentException)
            {
            }
            return road;
        }
    }
}

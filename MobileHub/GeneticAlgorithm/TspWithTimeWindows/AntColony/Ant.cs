using System;
using System.Collections.Generic;
using System.Linq;

namespace TspWithTimeWindows.AntColony
{
    public class Ant
    {

        public static bool EnforceRestrictionOrder = true;
        public static int MaxLateTolerance = 2500;
        public static int LatePenaltyQuotient = 1000000;
        public static int EarlyPenaltyQuotient = 1000;

        private static Random Random { get; } = new Random();

        public List<City> VisitedCities { get; }

        private City CurrentPosition { get; set; }

        public double Pheromones { get; set; }

        public double TourValue { get; private set; }
        public long TimeTraveled { get; private set; }
        private int CityArivalTime { get; set; }

        public Ant(City startPosition, double pheromones)
        {
            CurrentPosition = startPosition;
            Pheromones = pheromones;
            VisitedCities = new List<City> { startPosition };
        }

        public bool SearchTour(long maxTravelTime, bool returnToStart = true)
        {
            TourValue = 0.0;
            TimeTraveled = 0;
            CityArivalTime = 0;

            while (TravelOn(maxTravelTime))
            {
            }

            if (!returnToStart) return true;

            var closingRoad = CurrentPosition.Roads(VisitedCities.First());
            if (closingRoad == null) return false;

            var enforcedRestrictionOrderValue = GetEnforcedRestrictionOrderValue();

            TourValue += closingRoad.Distance;
            TimeTraveled += (int)closingRoad.Duration;

            return true;
        }

        private bool TravelOn(long maxTravelTime)
        {
            var nextCity = GetNextCity();
            if (nextCity == null)
            {
                return false;
            }

            CurrentPosition = nextCity;

            var timeWindowRestrictionValue = GetTimeWindowAdjustmentValue(CurrentPosition);
            CityArivalTime += (int)VisitedCities.Last().Roads(CurrentPosition).Duration;
            CurrentPosition.ArivalTime = CityArivalTime;

            TourValue += VisitedCities.Last().Roads(CurrentPosition).Distance + timeWindowRestrictionValue;
            TimeTraveled += (int)VisitedCities.Last().Roads(CurrentPosition).Duration + timeWindowRestrictionValue;
            //            if(TimeTraveled > maxTravelTime)
            //                return false;
            VisitedCities.Add(CurrentPosition);
            return true;
        }

        private City GetNextCity()
        {
            var cityWeights = new Dictionary<City, double>();
            var sumOfWeights = 0.0;

            foreach (var city in CurrentPosition.NeighbourCities)
            {
                if (!VisitedCities.Contains(city))
                {
                    var weight = CurrentPosition.Roads(city).WeighedValue;
                    cityWeights.Add(city, weight);
                    sumOfWeights += Convert.ToDouble(weight);
                }
            }

            var rnd = Random.NextDouble();
            var sum = 0.0;

            foreach (var pair in cityWeights)
            {
                sum += Convert.ToDouble(pair.Value / sumOfWeights);
                if (sum >= rnd)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        public static int GetTimeWindowAdjustmentValue(City city)
        {
            if (city.DesiredArivalTime == 0 || city.ArivalTime == city.DesiredArivalTime) return 0;
            var difference = Math.Abs(city.ArivalTime - city.DesiredArivalTime);
            if (city.ArivalTime > city.DesiredArivalTime)
            {
                if (difference > MaxLateTolerance)
                    return int.MaxValue;

                return difference * LatePenaltyQuotient;

            }
            return difference * EarlyPenaltyQuotient;
        }


        private int GetEnforcedRestrictionOrderValue()
        {
            if (!EnforceRestrictionOrder) return 0;


            double desiredArivalTime = 0;

            // calculate the path length from the first city to the last
            for (int i = 0, n = VisitedCities.Count; i < n; i++)
            {
                // get current city
                var city = VisitedCities.ElementAt(i);
                if (city.DesiredArivalTime > 0)
                {
                    if (desiredArivalTime > city.DesiredArivalTime)
                    {
                        return int.MaxValue;
                    }
                    desiredArivalTime = city.DesiredArivalTime;
                }
            }

            return 0;
        }

    }
}

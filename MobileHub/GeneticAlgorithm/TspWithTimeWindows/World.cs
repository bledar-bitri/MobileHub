using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TspWithTimeWindows.AntColony;

namespace TspWithTimeWindows
{
    public class World
    {
        public const int MinimumCitiesNeededToCalculateRoute = 4;

        public event UpdateEventHandler Update;
        public delegate void UpdateEventHandler(World sender, UpdateEventArgs e);

        public List<City> Cities { get; }
        public List<Road> Roads { get; }

        #region Ant Colony Properties
        private const int NumberOfAnts = 100;
        private const double InitialPheromoneValue = 1;
        private const double PheromoneDecayFactor = 0.1;
        private const int MaxIterationsWithoutChange = 20;

        public static int MaxOptimationTime = 10 * 60 * 1000;
        private static readonly Random Random = new Random();

        private long maxTravelTime { get; set; }
        public double BestTourValue { get; private set; }
        private double worstTourValue;
        #endregion

        public World(WorldBuilder prototype) : this(prototype.Cities, prototype.Roads)
        {
        }

        public World(IEnumerable<City> cities, IEnumerable<Road> roads)
        {
            Cities = new List<City>(cities);
            Roads = new List<Road>(roads);

            // needed only for Ant Colony
            worstTourValue = Convert.ToDouble((from road in roads select road).Sum(road => road.Distance));
        }

        protected void RaiseUpdate(UpdateEventArgs args)
        {
            if (Update != null)
            {
                Update(this, args);
            }
        }

        #region Ant Colony Functions
        public List<City> FindTour(int startIndex = -1, bool returnToStart = true)
        {

            if (Cities.Count < MinimumCitiesNeededToCalculateRoute)
            {
                if (startIndex < 0)
                    return Cities.ToList();

                var tour = Cities.Where((t, i) => i == startIndex).ToList();
                tour.AddRange(Cities.Where((t, i) => i != startIndex).ToList());
                return tour;
            }

            List<City> bestTour = null;
            BestTourValue = double.MaxValue;
            var iterations = 0;
            var iterationsWithoutChange = 0;
            var numberOfFailures = 0;
            double lastBestTourValue = -1;

            foreach (var road in Roads)
            {
                road.PheromoneLevel = Convert.ToDouble(InitialPheromoneValue);
            }

            const double antPheromoneCapacity = 0.2;
            var overallDecayValue = PheromoneDecayFactor * InitialPheromoneValue * Roads.Count;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (iterationsWithoutChange < MaxIterationsWithoutChange)
            {
                iterations++;
                var ants = new List<Ant>(NumberOfAnts);
                var antSuccess = new Dictionary<Ant, bool>(NumberOfAnts);

                for (var i = 1; i <= NumberOfAnts; i++)
                {
                    // We have to pick a random starting city (IF NO STARTING CITY IS PASSED) in order to distribute
                    // the pheromones for the last route of a tour randomly!
                    if (startIndex == -1)
                        startIndex = Random.Next(Cities.Count);
                    ants.Add(new Ant(Cities[startIndex], antPheromoneCapacity));
                }

                double lastValue = 0;
                int numSuccess = 0;

                foreach (var ant in ants)
                {
                    var success = ant.SearchTour(maxTravelTime, returnToStart) &&
                        (ant.VisitedCities.Count == Cities.Count || (!returnToStart && ant.VisitedCities.Count == Cities.Count - 1));
                    antSuccess[ant] = success;

                    if (success)
                    {
                        #region successfull ant

                        // We use a delta to compensate mathematical instabilities in
                        // floating point operations.
                        var delta = ant.TourValue - BestTourValue;

                        // We're talking about km here! 0.5 is small enough (pixels would be 0.01).
                        if (Math.Abs(delta) <= 0.5)
                        {
                            iterationsWithoutChange++;
                        }
                        else if (delta < 0)
                        {
                            BestTourValue = ant.TourValue;
                            bestTour = ant.VisitedCities;
                            iterationsWithoutChange = 0;
                        }
                        else if (delta <= BestTourValue * 0.01)
                        {
                            // The difference is too small to yield correct phermone
                            // values (that is: the tour is nearly as good as the
                            // current best one): We give up.
                            iterationsWithoutChange++;
                        }
                        else
                        {
                            iterationsWithoutChange = 0;
                        }

                        lastValue += ant.TourValue;
                        // we need to make sure we adjust the worst tour value to allow for time windows.
                        if (worstTourValue < ant.TourValue)
                            worstTourValue = ant.TourValue;
                        numSuccess++;

                        #endregion
                    }
                    else
                    {
                        iterationsWithoutChange = 0;
                        numberOfFailures++;
                    }
                }

                lastValue /= numSuccess;

                foreach (var road in Roads)
                {
                    // No decay for the currently best tour!
                    // This is an extension to the algorithm to ensure its termination.

                    var roadIsInBestTour = false;

                    if (bestTour != null && bestTour.Count > 0)
                    {
                        var first = bestTour[0];

                        for (var i = 1; i <= bestTour.Count - 1; i++)
                        {
                            if (first.Roads(bestTour[i]) == road)
                            {
                                roadIsInBestTour = true;
                                break;
                            }

                            first = bestTour[i];
                        }

                        if (bestTour[bestTour.Count - 1].Roads(bestTour[0]) == road)
                        {
                            roadIsInBestTour = true;
                        }
                    }

                    if (!roadIsInBestTour)
                    {
                        UpdatePheromoneLevel(road);
                    }
                }

                var individualPheromoneLevel = overallDecayValue;

                foreach (var annotatedAnt in antSuccess)
                {
                    if (annotatedAnt.Value)
                    {
                        annotatedAnt.Key.Pheromones = individualPheromoneLevel;
                        var cities = annotatedAnt.Key.VisitedCities;
                        var tourBonus = TourPheromoneBonus(annotatedAnt.Key);
                        for (var i = 1; i <= Cities.Count - 1; i++)
                        {
                            var road = cities[i - 1].Roads(cities[i]);
                            road.PheromoneLevel += tourBonus;
                        }

                        var lastRoad = cities[Cities.Count - 1].Roads(cities[0]);
                        lastRoad.PheromoneLevel += tourBonus;
                    }
                }

                RaiseUpdate(new UpdateEventArgs(iterations, iterationsWithoutChange, numberOfFailures, BestTourValue, lastValue, bestTour, null));

                /* restart stopwatch the first pass */
                if (lastBestTourValue < 0)
                {
                    lastBestTourValue = BestTourValue;
                    stopwatch.Restart();
                }
                //                File.AppendAllText("C:/temp/WorldLog.txt", string.Format("Total Run time: {0} of {1}" + Environment.NewLine, 
                //                    stopwatch.ElapsedMilliseconds,
                //                    MaxOptimationTime));
                if (lastBestTourValue > BestTourValue)
                {
                    // found new best tour, restart the stopwatch
                    //                    File.AppendAllText("C:/temp/WorldLog.txt", 
                    //                        String.Format("New best tour: from:{0} TO: {1} Restarting Time from: {2}"+Environment.NewLine, 
                    //                        lastBestTourValue.ToString("N2"), 
                    //                        BestTourValue.ToString("N2"), 
                    //                        stopwatch.ElapsedMilliseconds));
                    stopwatch.Restart();
                    lastBestTourValue = BestTourValue;
                }
                else if (stopwatch.ElapsedMilliseconds >= MaxOptimationTime)
                {
                    // if we search without change for a set amount of time [MaxOptimationTime] break the loop
                    //                    File.AppendAllText("C:/temp/WorldLog.txt", "STOPING LOOP DONE!");
                    iterationsWithoutChange = MaxIterationsWithoutChange; // break the loop and get out of here
                }
            }
            return bestTour;
        }

        public List<City> FindTourWithTimeWindow(int startIndex = -1, bool returnToStart = true)
        {

            if (Cities.Count < MinimumCitiesNeededToCalculateRoute)
            {
                if (startIndex < 0)
                    return Cities.ToList();

                var tour = Cities.Where((t, i) => i == startIndex).ToList();
                tour.AddRange(Cities.Where((t, i) => i != startIndex).ToList());
                return tour;
            }

            List<City> bestTour = null;
            BestTourValue = worstTourValue;
            var iterations = 0;
            var iterationsWithoutChange = 0;
            var numberOfFailures = 0;
            double lastBestTourValue = -1;

            foreach (var road in Roads)
            {
                road.PheromoneLevel = Convert.ToDouble(InitialPheromoneValue);
            }

            const double antPheromoneCapacity = 0.2;
            var overallDecayValue = PheromoneDecayFactor * InitialPheromoneValue * Roads.Count;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (iterationsWithoutChange < MaxIterationsWithoutChange)
            {
                iterations++;
                var ants = new List<Ant>(NumberOfAnts);
                var antSuccess = new Dictionary<Ant, bool>(NumberOfAnts);

                for (var i = 1; i <= NumberOfAnts; i++)
                {
                    // We have to pick a random starting city (IF NO STARTING CITY IS PASSED) in order to distribute
                    // the pheromones for the last route of a tour randomly!
                    if (startIndex == -1)
                        startIndex = Random.Next(Cities.Count);
                    ants.Add(new Ant(Cities[startIndex], antPheromoneCapacity));
                }

                double lastValue = 0;
                int numSuccess = 0;

                foreach (var ant in ants)
                {
                    var success = ant.SearchTour(maxTravelTime, returnToStart) &&
                        (ant.VisitedCities.Count == Cities.Count || (!returnToStart && ant.VisitedCities.Count == Cities.Count - 1));
                    antSuccess[ant] = success;

                    if (success)
                    {
                        // We use a delta to compensate mathematical instabilities in
                        // floating point operations.
                        var delta = ant.TourValue - BestTourValue;

                        // We're talking about km here! 0.5 is small enough (pixels would be 0.01).
                        if (Math.Abs(delta) <= 0.5)
                        {
                            iterationsWithoutChange++;
                        }
                        else if (delta < 0)
                        {
                            BestTourValue = ant.TourValue;
                            bestTour = ant.VisitedCities;
                            iterationsWithoutChange = 0;
                        }
                        else if (delta <= BestTourValue * 0.01)
                        {
                            // The difference is too small to yield correct phermone
                            // values (that is: the tour is nearly as good as the
                            // current best one): We give up.
                            iterationsWithoutChange++;
                        }
                        else
                        {
                            iterationsWithoutChange = 0;
                        }

                        lastValue += ant.TourValue;
                        numSuccess++;
                    }
                    else
                    {
                        iterationsWithoutChange = 0;
                        numberOfFailures++;
                    }
                }

                lastValue /= numSuccess;

                foreach (var road in Roads)
                {
                    // No decay for the currently best tour!
                    // This is an extension to the algorithm to ensure its termination.

                    var roadIsInBestTour = false;

                    if (bestTour != null && bestTour.Count > 0)
                    {
                        var first = bestTour[0];

                        for (var i = 1; i <= bestTour.Count - 1; i++)
                        {
                            if (first.Roads(bestTour[i]) == road)
                            {
                                roadIsInBestTour = true;
                                break;
                            }

                            first = bestTour[i];
                        }

                        if (bestTour[bestTour.Count - 1].Roads(bestTour[0]) == road)
                        {
                            roadIsInBestTour = true;
                        }
                    }

                    if (!roadIsInBestTour)
                    {
                        UpdatePheromoneLevel(road);
                    }
                }

                var individualPheromoneLevel = overallDecayValue;

                foreach (var annotatedAnt in antSuccess)
                {
                    if (annotatedAnt.Value)
                    {
                        annotatedAnt.Key.Pheromones = individualPheromoneLevel;
                        var cities = annotatedAnt.Key.VisitedCities;
                        var tourBonus = TourPheromoneBonus(annotatedAnt.Key);
                        for (var i = 1; i <= Cities.Count - 1; i++)
                        {
                            var road = cities[i - 1].Roads(cities[i]);
                            road.PheromoneLevel += tourBonus;
                        }

                        var lastRoad = cities[Cities.Count - 1].Roads(cities[0]);
                        lastRoad.PheromoneLevel += tourBonus;
                    }
                }

                RaiseUpdate(new UpdateEventArgs(iterations, iterationsWithoutChange, numberOfFailures, BestTourValue, lastValue, bestTour, null));

                /* restart stopwatch the first pass */
                if (lastBestTourValue < 0)
                {
                    lastBestTourValue = BestTourValue;
                    stopwatch.Restart();
                }
                //                File.AppendAllText("C:/temp/WorldLog.txt", string.Format("Total Run time: {0} of {1}" + Environment.NewLine, 
                //                    stopwatch.ElapsedMilliseconds,
                //                    MaxOptimationTime));
                if (lastBestTourValue > BestTourValue)
                {
                    // found new best tour, restart the stopwatch
                    //                    File.AppendAllText("C:/temp/WorldLog.txt", 
                    //                        String.Format("New best tour: from:{0} TO: {1} Restarting Time from: {2}"+Environment.NewLine, 
                    //                        lastBestTourValue.ToString("N2"), 
                    //                        BestTourValue.ToString("N2"), 
                    //                        stopwatch.ElapsedMilliseconds));
                    stopwatch.Restart();
                    lastBestTourValue = BestTourValue;
                }
                else if (stopwatch.ElapsedMilliseconds >= MaxOptimationTime)
                {
                    // if we search without change for a set amount of time [MaxOptimationTime] break the loop
                    //                    File.AppendAllText("C:/temp/WorldLog.txt", "STOPING LOOP DONE!");
                    iterationsWithoutChange = MaxIterationsWithoutChange; // break the loop and get out of here
                }
            }
            return bestTour;
        }

        private void UpdatePheromoneLevel(Road road)
        {
            const double remainingPheromoneFactor = 1.0 - PheromoneDecayFactor;
            road.PheromoneLevel = road.PheromoneLevel * remainingPheromoneFactor;
        }

        private double TourPheromoneBonus(Ant ant)
        {
            // We penalize long tours and try to get the worst possible tour
            // to yield 0 pheromone.
            // The square tries to "stretch" the range of possible bonuses.
            if (worstTourValue > ant.TourValue * 1.5) return 0.1;
            return Math.Pow((ant.Pheromones * (worstTourValue / ant.TourValue - 1)), 2);
        }

        #endregion

    }
}

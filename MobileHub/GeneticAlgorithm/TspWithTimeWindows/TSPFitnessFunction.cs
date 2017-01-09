// AForge Framework
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

using System;
using System.Collections.Generic;
using AForge.Genetic;
using TspWithTimeWindows;

/// <summary>
/// Fitness function for TSP task (Travaling Salasman Problem)
/// </summary>
public class TSPFitnessFunction : IFitnessFunction
{
    public static bool EnforceRestrictionOrder = true;
    public static int MaxLateTolerance = 2500;
    public static int LatePenaltyQuotient = 1000000;
    public static int EarlyPenaltyQuotient = 1000;
    
    private readonly City _startCity;
    private readonly City _endCity;

    // cities
    private readonly List<City> _cities;

    // Constructor
    public TSPFitnessFunction(City startCity, City endCity, List<City> cities)
    {
        _startCity = startCity;
        _endCity = endCity;
        _cities = cities;
    }

    /// <summary>
    /// Evaluate chromosome - calculates its fitness value
    /// </summary>
    public double Evaluate(IChromosome chromosome)
    {
        return 1/(PathLength(chromosome) + 1);
    }

    /// <summary>
    /// Translate genotype to phenotype
    /// </summary>
    public object Translate(IChromosome chromosome)
    {
        return chromosome.ToString();
    }

    /// <summary>
    /// Calculate path length represented by the specified chromosome 
    /// </summary>
    public double PathLength(IChromosome chromosome)
    {
        // salesman path
        ushort[] path = ((PermutationChromosome) chromosome).Value;

        // check path size
        if (path.Length != _cities.Count)
        {
            throw new ArgumentException("Invalid path specified - not all cities are visited");
        }

        // path length
        
        int prev = path[0];
        int curr = path[path.Length - 1];
        
        City c1 = _cities[prev];
        City c2 = null;

        double pathLength = _startCity.GetRoad(c1).Distance;
        double arivalTime = _startCity.GetRoad(c1).Duration;


        // calculate the path length from the first city to the last
        for (int i = 1, n = path.Length; i < n; i++)
        {
            // get current city
            curr = path[i];
            
            c1 = _cities[curr];
            c2 = _cities[prev];

            // calculate duration
            c2.ArivalTime = (int)arivalTime;
            arivalTime += (int)c1.GetRoad(c2).Duration;
            c1.ArivalTime = (int) arivalTime;
            
            // calculate distance
            pathLength += c1.GetRoad(c2).Distance;

            // calculate time windows
            pathLength += GetTimeWindowAdjustmentValue(c1);
            
            // put current city as previous
            prev = curr;
        }
        pathLength += _endCity.GetRoad(c1).Distance;
        _endCity.ArivalTime = (int)(arivalTime + _endCity.GetRoad(c1).Duration);


        pathLength += GetTimeWindowAdjustmentValue(_endCity);

        pathLength += GetEnforcedRestrictionOrderValue(chromosome);

        return pathLength;
    }

    private int GetTimeWindowAdjustmentValue(City city)
    {
        if (city.DesiredArivalTime == 0) return 0;
        int difference;
        if (city.ArivalTime > city.DesiredArivalTime)
        {
            difference = city.ArivalTime - city.DesiredArivalTime;

            if (difference > MaxLateTolerance)
                return int.MaxValue;

            return difference*LatePenaltyQuotient;

        }
        if (city.ArivalTime >= city.DesiredArivalTime) return 0;

        difference = city.ArivalTime - city.DesiredArivalTime;
        return difference*EarlyPenaltyQuotient;
    }

    private int GetEnforcedRestrictionOrderValue(IChromosome chromosome)
    {
        if (!EnforceRestrictionOrder) return 0;

        // salesman path
        ushort[] path = ((PermutationChromosome)chromosome).Value;

        
        // path length
        int curr;
        
        double desiredArivalTime = 0;
        
        // calculate the path length from the first city to the last
        for (int i = 0, n = path.Length; i < n; i++)
        {
            // get current city
            curr = path[i];
            var city = _cities[curr];
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

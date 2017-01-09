using System;
using System.Diagnostics;

namespace TspWithTimeWindows
{
    [DebuggerDisplay("Distance: {Distance}, Duration: {Duration}")]
    public class Road
    {
        public double Distance { get; }

        public double Duration { get; set; }

        #region Ant Colony Properties

        private const double Alpha = -1.5;
        private const double Beta = 1.5;
        public double PheromoneLevel { get; set; }
        #endregion

        public double WeighedValue
        {
            get
            {
                return Math.Pow(Distance, Alpha) * Math.Pow(PheromoneLevel, Beta);
            }
        }
        public Road(double distance)
        {
            Distance = distance;
            Duration = (int)(distance * 2);
        }
    }
}

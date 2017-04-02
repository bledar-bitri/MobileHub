using System;
using System.Diagnostics;

namespace TspWithTimeWindows
{
    [DebuggerDisplay("Distance: {Distance}, Duration: {Duration}")]
    public class Road
    {
        public double Distance { get; set; }

        public double Duration { get; set; }

        public City From { get; set; }
        public City To { get; set; }

        #region Ant Colony Properties

        private const double Alpha = -1.5;
        private const double Beta = 1.5;
        public double PheromoneLevel { get; set; }
        #endregion
        
        public double WeighedValue => Math.Pow(Distance, Alpha) * Math.Pow(PheromoneLevel, Beta);

        public Road(City from, City to)
        {
            From = from;
            To = to;
        }

        public bool Equals(Road r)
        {
            return (From.Equals(r.From) && To.Equals(r.To)) || (From.Equals(r.To) && To.Equals(r.From));
        }

        public string DebuggerDisplay
        {
            get
            {
                Trace.TraceInformation($"[FROM: {From.DebuggerDisplay}]");
                Trace.TraceInformation($"[TO: {To.DebuggerDisplay}]");

                return
                    $"[FROM: {From.DebuggerDisplay}] [TO: {To.DebuggerDisplay}] [Distance: {Distance}] [TravelTime: {Duration}]";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class GeoCodeConverter
    {
        private const double ToIntegerMultiplier = 1000000;
        private const double ToGeoCoordinateMultiplier = 0.000001;

        public static int ToInteger(double geoCoordinate)
        {
            return (int) (geoCoordinate * ToIntegerMultiplier);
        }

        public static double ToGeoCoordinate(long intValue)
        {
            return intValue * ToGeoCoordinateMultiplier;
        }
    }
}

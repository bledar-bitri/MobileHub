using System;

namespace Common
{
    public class CommonUtils
    {
        private const double Epsilon = 0.001;

        public static string FormatTimeSpan(TimeSpan ts)
        {
            return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }

        public static bool IsZero(double val)
        {
            return AlmostEqual(val, 0);
        }

        public static bool AlmostEqual(double val1, double val2, double epsilon = Epsilon)
        {

            return Math.Abs(val1 - val2) < epsilon;
        }
    }
}

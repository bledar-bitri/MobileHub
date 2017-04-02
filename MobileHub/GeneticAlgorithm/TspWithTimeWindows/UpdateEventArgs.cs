using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TspWithTimeWindows
{
    public class UpdateEventArgs : EventArgs
    {
        public int CurrentIteration { get; }

        public int SuccessfulIterations { get; }

        public int Failures { get; }

        public double CurrentBestValue { get; }

        public double LastValue { get; }

        public IEnumerable<City> BestTour { get; }

        public ushort[] BestPath { get; }

        public bool ShowBestTourValue { get; }

        public UpdateEventArgs(int current_iteration, int successful_iterations, int failures, double current_best_value, double last_value, IEnumerable<City> best_tour, ushort[] best_path, bool show_best_tour_value = true)
        {
            CurrentIteration = current_iteration;
            SuccessfulIterations = successful_iterations;
            Failures = failures;
            CurrentBestValue = current_best_value;
            LastValue = last_value;
            BestTour = best_tour;
            BestPath = best_path;
            ShowBestTourValue = show_best_tour_value;

        }

        public override string ToString()
        {
            return string.Format("Iteration: {1}{0}w/o change: {2}{0}failed: {3}{0}value: {4:f1}{0}last: {5:f1}", "\t", CurrentIteration, SuccessfulIterations, Failures, CurrentBestValue, LastValue);
        }

    }
}

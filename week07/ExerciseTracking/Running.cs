
using System;

namespace ExerciseTracking
{
    public sealed class Running : Activity
    {
        private readonly double _distanceKm;

        public Running(DateTime date, int minutes, double distanceKm)
            : base(date, minutes)
        {
            if (distanceKm < 0)
                throw new ArgumentOutOfRangeException(nameof(distanceKm), "Distance must be non-negative.");
            _distanceKm = distanceKm;
        }

        protected override string ActivityType => "Running";

        public override double GetDistanceKm() => _distanceKm;

        public override double GetSpeedKph()
        {
            return _distanceKm <= 0 ? 0 : (_distanceKm / Minutes) * 60.0;
        }

        public override double GetPaceMinPerKm()
        {

            return _distanceKm <= 0 ? 0 : Minutes / _distanceKm;
        }
    }
}

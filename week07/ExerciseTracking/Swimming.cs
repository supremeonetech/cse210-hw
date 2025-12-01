
using System;

namespace ExerciseTracking
{
    public sealed class Swimming : Activity
    {
        private readonly int _laps;
        private const double MetersPerLap = 50.0;

        public Swimming(DateTime date, int minutes, int laps)
            : base(date, minutes)
        {
            if (laps < 0)
                throw new ArgumentOutOfRangeException(nameof(laps), "Laps must be non-negative.");
            _laps = laps;
        }

        protected override string ActivityType => "Swimming";

        public override double GetDistanceKm()
        {
            return (_laps * MetersPerLap) / 1000.0;
        }

        public override double GetSpeedKph()
        {
            double km = GetDistanceKm();
        
            return km <= 0 ? 0 : (km / Minutes) * 60.0;
        }

        public override double GetPaceMinPerKm()
        {
            double km = GetDistanceKm();
        
            return km <= 0 ? 0 : Minutes / km;
        }
    }
}

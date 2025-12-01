
using System;

namespace ExerciseTracking
{
    public sealed class Cycling : Activity
    {
        private readonly double _speedKph;

        public Cycling(DateTime date, int minutes, double speedKph)
            : base(date, minutes)
        {
            if (speedKph < 0)
                throw new ArgumentOutOfRangeException(nameof(speedKph), "Speed must be non-negative.");
            _speedKph = speedKph;
        }

        protected override string ActivityType => "Cycling";

        public override double GetDistanceKm()
        {
            return _speedKph <= 0 ? 0 : _speedKph * (Minutes / 60.0);
        }

        public override double GetSpeedKph() => _speedKph;

        public override double GetPaceMinPerKm()
        {
            return _speedKph <= 0 ? 0 : 60.0 / _speedKph;
        }
    }
}

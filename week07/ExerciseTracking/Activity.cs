
using System;

namespace ExerciseTracking
{
    public abstract class Activity
    {
        private readonly DateTime _date;
        private readonly int _minutes;

        protected Activity(DateTime date, int minutes)
        {
            if (minutes <= 0)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be greater than zero.");

            _date = date;
            _minutes = minutes;
        }

        public DateTime Date => _date;
        public int Minutes => _minutes;

        public abstract double GetDistanceKm();
        public abstract double GetSpeedKph();
        public abstract double GetPaceMinPerKm();

        protected abstract string ActivityType { get; }

        public virtual string GetSummary()
        {
            string dateStr = _date.ToString("dd MMM yyyy");

            double distance = GetDistanceKm();
            double speed = GetSpeedKph();
            double pace = GetPaceMinPerKm();

            string distanceStr = distance > 0 ? distance.ToString("F1") : "N/A";
            string speedStr = speed > 0 ? speed.ToString("F1") : "N/A";
            string paceStr = pace > 0 ? pace.ToString("F2") : "N/A";

            return $"{dateStr} {ActivityType} ({_minutes} min): " +
                   $"Distance {distanceStr} km, Speed: {speedStr} kph, Pace: {paceStr} min per km";
        }
    }
}
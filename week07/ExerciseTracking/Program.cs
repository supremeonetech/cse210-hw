
using System;
using System.Collections.Generic;

namespace ExerciseTracking
{
    internal static class Program
    {
        static void Main()
        {
            var activities = new List<Activity>
            {
                new Running(new DateTime(2022, 11, 3), minutes: 30, distanceKm: 4.8),

                new Cycling(new DateTime(2022, 11, 3), minutes: 30, speedKph: 15.0),

                new Swimming(new DateTime(2022, 11, 3), minutes: 30, laps: 40)
            };

            foreach (var activity in activities)
            {
                Console.WriteLine(activity.GetSummary());
            }
        }
    }
}

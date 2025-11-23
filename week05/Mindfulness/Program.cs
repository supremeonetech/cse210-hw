
// Program.cs
// CSE 210 - W05 Mindfulness Program
// Exceeds ideas you could enable:
// - Log sessions to a file
// - Non-repeating prompts per session
// - Enhanced breathing animation (ease-in/out)
// Document any extras you implement here.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

#nullable enable

namespace MindfulnessApp
{
    class Program
    {
        static void Main()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Mindfulness Program");
                Console.WriteLine("-------------------");
                Console.WriteLine("1) Breathing Activity");
                Console.WriteLine("2) Reflection Activity");
                Console.WriteLine("3) Listing Activity");
                Console.WriteLine("4) Quit");
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine();
                Activity? activity = choice switch
                {
                    "1" => new BreathingActivity(),
                    "2" => new ReflectionActivity(),
                    "3" => new ListingActivity(),
                    "4" => null,
                    _ => null
                };

                if (choice == "4" || activity is null && choice != null)
                {
                    Console.WriteLine("\nGoodbye!");
                    break;
                }

                Console.Clear();
                try
                {
                    activity!.Start(); // common start flow
                    activity.Run();    // activity-specific logic wrapped with duration control
                    activity.End();    // common end flow
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nAn error occurred: {ex.Message}");
                    Console.WriteLine("Returning to menu...");
                    Activity.ShowSpinner(2);
                }

                Console.Clear();
            }
        }
    }

    abstract class Activity
    {
        public string Name { get; protected set; } = "";
        public string Description { get; protected set; } = "";
        private int DurationSeconds { get; set; } = 0;

        public void Start()
        {
            Console.WriteLine($"{Name}");
            Console.WriteLine($"{Description}\n");
            DurationSeconds = AskDurationSeconds();
            Console.WriteLine("\nGet ready to begin...");
            ShowSpinner(3);
        }

        public void Run()
        {
            var total = TimeSpan.FromSeconds(DurationSeconds);
            RunCore(total);
        }

        public void End()
        {
            Console.WriteLine("\nGreat job!");
            ShowSpinner(2);
            Console.WriteLine($"You have completed the {Name} for {DurationSeconds} seconds.");
            ShowSpinner(3);
        }

        protected abstract void RunCore(TimeSpan total);

        protected static int AskDurationSeconds()
        {
            while (true)
            {
                Console.Write("Enter duration (seconds): ");
                if (int.TryParse(Console.ReadLine(), out int s) && s > 0)
                    return s;
                Console.WriteLine("Please enter a positive integer.");
            }
        }

        // Spinner animation: | / - \ cycles
        public static void ShowSpinner(int seconds)
        {
            char[] frames = new[] { '|', '/', '-', '\\' };
            var stop = DateTime.Now.AddSeconds(seconds);
            int i = 0;
            while (DateTime.Now < stop)
            {
                Console.Write(frames[i % frames.Length]);
                Thread.Sleep(120);
                Console.Write("\b \b");
                i++;
            }
        }

        // Countdown animation: 3..2..1..
        public static void ShowCountdown(int seconds)
        {
            for (int s = seconds; s > 0; s--)
            {
                Console.Write($"{s}");
                Thread.Sleep(1000);
                Console.Write("\b \b");
            }
        }
    }

    class BreathingActivity : Activity
    {
        public BreathingActivity()
        {
            Name = "Breathing Activity";
            Description = "This activity will help you relax by walking you through breathing in and out slowly. Clear your mind and focus on your breathing.";
        }

        protected override void RunCore(TimeSpan total)
        {
            var end = DateTime.Now.Add(total);
            bool inhale = true;
            while (DateTime.Now < end)
            {
                if (inhale)
                {
                    Console.Write("\nBreathe in... ");
                    Activity.ShowCountdown(4); // you can tweak durations
                }
                else
                {
                    Console.Write("\nBreathe out... ");
                    Activity.ShowCountdown(4);
                }
                inhale = !inhale;
            }
        }
    }

    class ReflectionActivity : Activity
    {
        private readonly string[] _prompts = new[]
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        };

        private readonly string[] _questions = new[]
        {
            "Why was this experience meaningful to you?",
            "Have you ever done anything like this before?",
            "How did you get started?",
            "How did you feel when it was complete?",
            "What made this time different than other times when you were not as successful?",
            "What is your favorite thing about this experience?",
            "What could you learn from this experience that applies to other situations?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?"
        };

        public ReflectionActivity()
        {
            Name = "Reflection Activity";
            Description = "This activity will help you reflect on times in your life when you have shown strength and resilience. This will help you recognize the power you have and how you can use it in other aspects of your life.";
        }

        protected override void RunCore(TimeSpan total)
        {
            var rand = new Random();
            Console.WriteLine($"\nPrompt: {_prompts[rand.Next(_prompts.Length)]}");
            Console.WriteLine("Consider the following questions:");
            var end = DateTime.Now.Add(total);

            while (DateTime.Now < end)
            {
                string q = _questions[rand.Next(_questions.Length)];
                Console.Write($"\n- {q} ");
                Activity.ShowSpinner(5); // pause to reflect
            }
        }
    }

    class ListingActivity : Activity
    {
        private readonly string[] _prompts = new[]
        {
            "Who are people that you appreciate?",
            "What are personal strengths of yours?",
            "Who are people that you have helped this week?",
            "When have you felt the Holy Ghost this month?",
            "Who are some of your personal heroes?"
        };

        public ListingActivity()
        {
            Name = "Listing Activity";
            Description = "This activity will help you reflect on the good things in your life by having you list as many things as you can in a certain area.";
        }

        protected override void RunCore(TimeSpan total)
        {
            var rand = new Random();
            Console.WriteLine($"\nPrompt: {_prompts[rand.Next(_prompts.Length)]}");
            Console.Write("Get ready to list items in...");
            Activity.ShowCountdown(3);
            Console.WriteLine("\nStart listing (press Enter after each item).");

            var end = DateTime.Now.Add(total);
            var items = new List<string>();
            while (DateTime.Now < end)
            {
                // Non-blocking check: since ReadLine blocks, we budget reads by time.
                // Simple approach: check remaining time before accepting another line.
                if (DateTime.Now >= end) break;
                Console.Write("> ");
                // If time expires during typing, ReadLine may still complete—acceptable for this assignment.
                string? line = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                    items.Add(line.Trim());
            }

            Console.WriteLine($"\nYou listed {items.Count} item(s).");
        }
    }
}

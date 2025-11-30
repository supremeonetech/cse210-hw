
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EternalQuest
{
    // =========================
    // Base Goal abstraction
    // =========================
    abstract class Goal
    {
        private readonly string _name;
        private readonly string _description;
        private readonly int _points;

        protected Goal(string name, string description, int points)
        {
            _name = name;
            _description = description;
            _points = points;
        }

        public string Name => _name;
        public string Description => _description;
        public int Points => _points;

        /// <summary>Perform one occurrence; return points gained this time.</summary>
        public abstract int RecordEvent();

        /// <summary>Whether this goal is fully complete.</summary>
        public abstract bool IsComplete { get; }

        /// <summary>Status text with checkbox/progress for listing.</summary>
        public abstract string GetStatus();

        /// <summary>Serialize to a single line.</summary>
        public abstract string GetStringRepresentation();

        /// <summary>Deserialize from a single line (factory).</summary>
        public static Goal FromString(string line)
        {
            var parts = line.Split('|');
            var type = parts[0];

            switch (type)
            {
                case "SimpleGoal":
                    {
                        string name = parts[1];
                        string desc = parts[2];
                        int points = int.Parse(parts[3]);
                        bool isComplete = bool.Parse(parts[4]);
                        var g = new SimpleGoal(name, desc, points);
                        if (isComplete) g.MarkComplete();
                        return g;
                    }
                case "EternalGoal":
                    {
                        string name = parts[1];
                        string desc = parts[2];
                        int points = int.Parse(parts[3]);

                        // New format includes timesRecorded at index 4; handle old format gracefully.
                        int timesRecorded = parts.Length >= 5 ? int.Parse(parts[4]) : 0;
                        return new EternalGoal(name, desc, points, timesRecorded);
                    }
                case "ChecklistGoal":
                    {
                        string name = parts[1];
                        string desc = parts[2];
                        int points = int.Parse(parts[3]);
                        int current = int.Parse(parts[4]);
                        int target = int.Parse(parts[5]);
                        int bonus = int.Parse(parts[6]);
                        return new ChecklistGoal(name, desc, points, target, bonus, current);
                    }
                default:
                    throw new InvalidOperationException($"Unknown goal type: {type}");
            }
        }
    }

    // =========================
    // SimpleGoal
    // =========================
    class SimpleGoal : Goal
    {
        private bool _isComplete;

        public SimpleGoal(string name, string description, int points)
            : base(name, description, points)
        {
            _isComplete = false;
        }

        public void MarkComplete() => _isComplete = true;

        public override int RecordEvent()
        {
            if (_isComplete) return 0;
            _isComplete = true;
            return Points;
        }

        public override bool IsComplete => _isComplete;

        public override string GetStatus()
        {
            string box = _isComplete ? "[X]" : "[ ]";
            return $"{box} {Name} ({Description})";
        }

        public override string GetStringRepresentation()
        {
            return $"SimpleGoal|{Name}|{Description}|{Points}|{_isComplete}";
        }
    }

    // =========================
    // EternalGoal (never completes)
    // =========================
    class EternalGoal : Goal
    {
        private int _timesRecorded;

        public EternalGoal(string name, string description, int points, int timesRecorded = 0)
            : base(name, description, points)
        {
            _timesRecorded = timesRecorded;
        }

        public int TimesRecorded => _timesRecorded;

        public override int RecordEvent()
        {
            _timesRecorded++;
            return Points;
        }

        public override bool IsComplete => false;

        public override string GetStatus()
        {
            return $"[ ] {Name} ({Description}) – Eternal (x{_timesRecorded})";
        }

        public override string GetStringRepresentation()
        {
            return $"EternalGoal|{Name}|{Description}|{Points}|{_timesRecorded}";
        }
    }

    // =========================
    // ChecklistGoal
    // =========================
    class ChecklistGoal : Goal
    {
        private int _currentCount;
        private readonly int _targetCount;
        private readonly int _bonusPoints;

        public ChecklistGoal(string name, string description, int points, int targetCount, int bonusPoints, int currentCount = 0)
            : base(name, description, points)
        {
            _targetCount = targetCount;
            _bonusPoints = bonusPoints;
            _currentCount = currentCount;
        }

        public override int RecordEvent()
        {
            if (IsComplete) return 0;
            _currentCount++;
            bool justCompleted = IsComplete;
            return Points + (justCompleted ? _bonusPoints : 0);
        }

        public override bool IsComplete => _currentCount >= _targetCount;

        public override string GetStatus()
        {
            string box = IsComplete ? "[X]" : "[ ]";
            return $"{box} {Name} ({Description}) — Completed {_currentCount}/{_targetCount}";
        }

        public override string GetStringRepresentation()
        {
            return $"ChecklistGoal|{Name}|{Description}|{Points}|{_currentCount}|{_targetCount}|{_bonusPoints}";
        }
    }

    class Badge
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public bool Earned { get; private set; }

        public Badge(string id, string name, string description)
        {
            Id = id; Name = name; Description = description; Earned = false;
        }

        public void Earn()
        {
            Earned = true;
        }

        public override string ToString()
        {
            return $"{(Earned ? "[🏅]" : "[ ]")} {Name} — {Description}";
        }
    }

      class BadgeService
    {
        private readonly Dictionary<string, Badge> _badges = new()
        {
            { "FIRST_EVENT",       new Badge("FIRST_EVENT", "First Steps", "Recorded your first event.") },
            { "SIMPLE_FINISHER",   new Badge("SIMPLE_FINISHER", "Simple Finisher", "Completed a simple one-time goal.") },
            { "CHECKLIST_CHAMPION",new Badge("CHECKLIST_CHAMPION", "Checklist Champion", "Completed a checklist goal.") },
            { "EVERGREEN_10",      new Badge("EVERGREEN_10", "Evergreen x10", "Recorded the same eternal goal 10 times.") },
            { "MILESTONE_1000",    new Badge("MILESTONE_1000", "Milestone 1000", "Reached 1000 total points.") },
            { "MILESTONE_5000",    new Badge("MILESTONE_5000", "Milestone 5000", "Reached 5000 total points.") },
        };

        public IEnumerable<Badge> All => _badges.Values;

        public void EvaluateAfterEvent(Goal goal, int gainedPoints, int score, int totalEvents)
        {
            if (totalEvents == 1) Earn("FIRST_EVENT");

            if (goal is SimpleGoal sg && sg.IsComplete) Earn("SIMPLE_FINISHER");
            if (goal is ChecklistGoal cg && cg.IsComplete) Earn("CHECKLIST_CHAMPION");

            if (goal is EternalGoal eg && eg.TimesRecorded >= 10) Earn("EVERGREEN_10");

            if (score >= 1000) Earn("MILESTONE_1000");
            if (score >= 5000) Earn("MILESTONE_5000");
        }

        private void Earn(string id)
        {
            if (_badges.TryGetValue(id, out var b) && !b.Earned)
            {
                b.Earn();
                Console.WriteLine($"🎉 Badge earned: {b.Name}!");
            }
        }

        public void ShowBadges()
        {
            Console.WriteLine("=== Badges ===");
            foreach (var b in _badges.Values)
                Console.WriteLine(b.ToString());
        }

        public string Serialize()
        {
            var earnedIds = _badges.Values.Where(b => b.Earned).Select(b => b.Id);
            return "BADGES|" + string.Join(",", earnedIds);
        }

        public void Deserialize(string line)
        {
            var parts = line.Split('|');
            if (parts.Length < 2) return;
            var ids = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in ids)
            {
                if (_badges.TryGetValue(id, out var b)) b.Earn();
            }
        }
    }
        class GoalService
    {
        private readonly List<Goal> _goals = new();
        private int _score = 0;
        private int _totalEvents = 0;
        private int _level = 1;

        private readonly BadgeService _badgeService = new();

        public IReadOnlyList<Goal> Goals => _goals;
        public int Score => _score;
        public int Level => _level;
        public int TotalEvents => _totalEvents;
        public BadgeService Badges => _badgeService;

        public void AddGoal(Goal g) => _goals.Add(g);

        public void ListGoals()
        {
            if (_goals.Count == 0)
            {
                Console.WriteLine("No goals yet. Create one!");
                return;
            }
            for (int i = 0; i < _goals.Count; i++)
                Console.WriteLine($"{i + 1}. {_goals[i].GetStatus()}");
        }

        public void RecordEvent(int index)
        {
            if (index < 0 || index >= _goals.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            int gained = _goals[index].RecordEvent();
            _totalEvents++;

            int prevLevel = _level;
            _score += gained;
            _level = CalculateLevel(_score);

            Console.WriteLine(gained > 0
                ? $"Event recorded! You gained {gained} points."
                : "No points gained (goal already complete).");

            if (_level > prevLevel)
                Console.WriteLine($"⬆️ Level Up! You are now Level {_level}.");

            // Evaluate badges
            _badgeService.EvaluateAfterEvent(_goals[index], gained, _score, _totalEvents);
        }

        private int CalculateLevel(int score) => Math.Max(1, (score / 1000) + 1);

        public void Save(string filename)
        {
            using var writer = new StreamWriter(filename);
            writer.WriteLine(_score);
            writer.WriteLine($"EVENTS|{_totalEvents}");
            writer.WriteLine($"LEVEL|{_level}");

            foreach (var g in _goals)
                writer.WriteLine(g.GetStringRepresentation());

            writer.WriteLine(_badgeService.Serialize());
            Console.WriteLine($"Saved {_goals.Count} goal(s), score {_score}, level {_level} to '{filename}'.");
        }

        public void Load(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("No file found to load.");
                return;
            }

            var lines = File.ReadAllLines(filename);
            _goals.Clear();
            _score = 0;
            _totalEvents = 0;
            _level = 1;

            if (lines.Length == 0)
            {
                Console.WriteLine("File was empty.");
                return;
            }

            int lineIndex = 0;

            _score = int.Parse(lines[lineIndex++]);

            for (; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("EVENTS|"))
                {
                    _totalEvents = int.Parse(line.Split('|')[1]);
                }
                else if (line.StartsWith("LEVEL|"))
                {
                    _level = int.Parse(line.Split('|')[1]);
                }
                else if (line.StartsWith("BADGES|"))
                {
                    _badgeService.Deserialize(line);
                }
                else
                {
       
                    var g = Goal.FromString(line);
                    _goals.Add(g);
                }
            }

            Console.WriteLine($"Loaded {_goals.Count} goal(s). Current score: {_score}. Level: {_level}. Events: {_totalEvents}.");
        }
    }

    class Program
    {
        static void Main()
        {
            var service = new GoalService();
            const string defaultFile = "goals.txt";

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== Eternal Quest ===");
                Console.WriteLine($"Score: {service.Score}   Level: {service.Level}   Events: {service.TotalEvents}");
                Console.WriteLine("1) Create New Goal");
                Console.WriteLine("2) List Goals");
                Console.WriteLine("3) Record Event");
                Console.WriteLine("4) View Badges");
                Console.WriteLine("5) Save");
                Console.WriteLine("6) Load");
                Console.WriteLine("7) Exit");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": CreateGoal(service); break;
                    case "2": service.ListGoals(); break;
                    case "3": RecordEvent(service); break;
                    case "4": service.Badges.ShowBadges(); break;
                    case "5": service.Save(defaultFile); break;
                    case "6": service.Load(defaultFile); break;
                    case "7": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }

        static void CreateGoal(GoalService service)
        {
            Console.WriteLine("Select goal type:");
            Console.WriteLine(" 1) Simple");
            Console.WriteLine(" 2) Eternal");
            Console.WriteLine(" 3) Checklist");
            Console.Write("Type #: ");
            string typeChoice = Console.ReadLine();

            Console.Write("Name: ");
            string name = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Description: ");
            string desc = Console.ReadLine()?.Trim() ?? "";
            int points = ReadInt("Points per completion");

            switch (typeChoice)
            {
                case "1":
                    service.AddGoal(new SimpleGoal(name, desc, points));
                    break;
                case "2":
                    service.AddGoal(new EternalGoal(name, desc, points));
                    break;
                case "3":
                    int target = ReadInt("Target count (how many times to complete)");
                    int bonus = ReadInt("Bonus points on final completion");
                    service.AddGoal(new ChecklistGoal(name, desc, points, target, bonus));
                    break;
                default:
                    Console.WriteLine("Unknown goal type.");
                    break;
            }
        }

        static void RecordEvent(GoalService service)
        {
            if (!service.Goals.Any())
            {
                Console.WriteLine("No goals to record. Create one first.");
                return;
            }

            service.ListGoals();
            int index = ReadInt("Record which goal #") - 1;
            service.RecordEvent(index);
        }

            static int ReadInt(string label)
            {
                while (true)
                {
                    Console.Write($"{label}: ");
                    var s = Console.ReadLine();
                    if (int.TryParse(s, out int n) && n >= 0) return n;
                    Console.WriteLine("Please enter a non-negative integer.");
                }
            }
        }
    }

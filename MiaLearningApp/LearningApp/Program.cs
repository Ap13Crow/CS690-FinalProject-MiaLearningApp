using System;
using System.Linq;
using System.Threading;
using LearningApp.Data;
using LearningApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningApp
{
    internal class Program
    {
        private static Timer? _quizTimer;   // background quiz‑reminder timer

        static void Main(string[] args)
        {
            // start reminder every 1 minute
            _quizTimer = new Timer(CheckQuizDue, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); /*learn.microsoft.com timer ex turn0search2*/

            while (true)
            {
                DisplayMainMenu();
                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1": ViewCourses();         break;
                    case "2": AddCourse();           break;
                    case "3": UpdateProgress();      break;
                    case "4": ManageNotes();         break;
                    case "5": ShowProgressSummary(); break;
                    case "6": ManageVocabulary();    break;
                    case "7": GlobalSearch();        break;
                    case "8": ManageModules();       break;
                    case "9": ManageResources();     break;
                    case "10": ManageQuizzes();      break;
                    case "11":
                        Console.WriteLine("Exiting Learning App. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        Pause();
                        break;
                }
            }
        }

        private static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== My Learning App - Main Menu ===");
            Console.WriteLine("1) View Courses and Progress");
            Console.WriteLine("2) Add a New Course");
            Console.WriteLine("3) Update Progress in a Course");
            Console.WriteLine("4) Manage Notes");
            Console.WriteLine("5) View Progress Summary (Dashboard)");
            Console.WriteLine("6) Manage Vocabulary");
            Console.WriteLine("7) Global Search");
            Console.WriteLine("8) Manage Courses & Modules");
            Console.WriteLine("9) Manage Resources");
            Console.WriteLine("10) Manage Quizzes");
            Console.WriteLine("11) Exit");
            Console.Write("Enter your choice: ");
        }

        //───────────────────────────  Quiz Reminder  ───────────────────────────

        private static void CheckQuizDue(object? state)
        {
            using var ctx = new AppDbContext();
            var due = ctx.Quizzes
                         .Include(q => q.Course)
                         .Where(q => q.DueDate <= DateTime.Now && !q.ReminderSent)
                         .ToList();
            foreach (var q in due)
            {
                Console.Beep();                                               /*timer docs turn0search11*/
                Console.WriteLine($"\n** Reminder ** '{q.Title}' quiz ({q.Course?.Title}) is due!");
                q.ReminderSent = true;
            }
            ctx.SaveChanges();                                                /*EF save turn0search7*/
        }

        //───────────────────────────  Course Section  ──────────────────────────

        private static void AddCourse()
        {
            Console.Clear();
            Console.WriteLine("=== Add Course ===");
            Console.Write("Enter a new course title (or press Enter to cancel): ");
            var title = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("Cancelled."); Pause(); return; }

            using var ctx = new AppDbContext();
            if (ctx.Courses.Any(c => c.Title.ToLower() == title.ToLower()))
            {
                Console.WriteLine("Course already exists!");
            }
            else
            {
                ctx.Courses.Add(new Course { Title = title, Progress = 0 });
                ctx.SaveChanges();
                Console.WriteLine($"Course '{title}' created.");
            }
            Pause();
        }

        // ViewCourses, UpdateProgress, ManageNotes, ShowProgressSummary,
        // GlobalSearch  -> unchanged from your last version
        // (keep them in the file)

        //──────────────────────  8  Modules submenu  ──────────────────────────

        private static void ManageModules()
        {
            using var ctx = new AppDbContext();
            var courses = ctx.Courses.Include(c => c.Modules).ToList();
            if (!courses.Any()) { Console.WriteLine("Add a course first."); Pause(); return; }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Modules ===");
                for (int i = 0; i < courses.Count; i++)
                    Console.WriteLine($"{i + 1}) {courses[i].Title}");
                Console.WriteLine("0) Back");
                if (!int.TryParse(Console.ReadLine(), out int ci) || ci < 0 || ci > courses.Count) continue;
                if (ci == 0) return;

                var course = courses[ci - 1];
                Console.Clear();
                Console.WriteLine($"Modules for {course.Title}");
                Console.WriteLine("1) Add module");
                Console.WriteLine("2) Toggle completion");
                Console.WriteLine("3) Back");
                var sub = Console.ReadLine();
                if (sub == "3") continue;

                if (sub == "1")
                {
                    Console.Write("Module title: ");
                    var mt = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(mt))
                    {
                        course.Modules.Add(new Module { Title = mt });
                        ctx.SaveChanges();
                    }
                }
                else if (sub == "2")
                {
                    for (int i = 0; i < course.Modules.Count; i++)
                        Console.WriteLine($"{i + 1}) {(course.Modules[i].IsCompleted ? "[x]" : "[ ]")} {course.Modules[i].Title}");
                    if (int.TryParse(Console.ReadLine(), out int mi) &&
                        mi >= 1 && mi <= course.Modules.Count)
                    {
                        course.Modules[mi - 1].IsCompleted = !course.Modules[mi - 1].IsCompleted;
                        ctx.SaveChanges();
                    }
                }
            }
        }

        //──────────────────────  9  Resources submenu  ────────────────────────

        private static void ManageResources()
        {
            using var ctx = new AppDbContext();
            var courses = ctx.Courses.Include(c => c.Resources).ToList();
            if (!courses.Any()) { Console.WriteLine("Add a course first."); Pause(); return; }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Resources ===");
                for (int i = 0; i < courses.Count; i++)
                    Console.WriteLine($"{i + 1}) {courses[i].Title}");
                Console.WriteLine("0) Back");
                if (!int.TryParse(Console.ReadLine(), out int ci) || ci < 0 || ci > courses.Count) continue;
                if (ci == 0) return;

                var course = courses[ci - 1];
                Console.Clear();
                Console.WriteLine($"Resources for {course.Title}");
                foreach (var r in course.Resources)
                    Console.WriteLine($"• {r.Url}  ({r.Description})");
                Console.WriteLine("\n1) Add  2) Remove  3) Back");
                var action = Console.ReadLine();
                if (action == "3") continue;

                if (action == "1")
                {
                    Console.Write("URL: ");           var url = Console.ReadLine() ?? "";
                    Console.Write("Description: ");   var desc = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        course.Resources.Add(new Resource { Url = url, Description = desc });
                        ctx.SaveChanges();
                    }
                }
                else if (action == "2")
                {
                    for (int i = 0; i < course.Resources.Count; i++)
                        Console.WriteLine($"{i + 1}) {course.Resources[i].Url}");
                    if (int.TryParse(Console.ReadLine(), out int ri) &&
                        ri >= 1 && ri <= course.Resources.Count)
                    {
                        ctx.Resources.Remove(course.Resources[ri - 1]);       /*EF remove turn0search1*/
                        ctx.SaveChanges();
                    }
                }
            }
        }

        //────────────────────── 10  Quizzes submenu  ──────────────────────────

        private static void ManageQuizzes()
        {
            using var ctx = new AppDbContext();
            var courses = ctx.Courses.Include(c => c.Quizzes).ToList();
            if (!courses.Any()) { Console.WriteLine("Add a course first."); Pause(); return; }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Quizzes ===");
                for (int i = 0; i < courses.Count; i++)
                    Console.WriteLine($"{i + 1}) {courses[i].Title}");
                Console.WriteLine("0) Back");
                if (!int.TryParse(Console.ReadLine(), out int ci) || ci < 0 || ci > courses.Count) continue;
                if (ci == 0) return;

                var course = courses[ci - 1];
                Console.Clear();
                Console.WriteLine($"Quizzes for {course.Title}");
                foreach (var q in course.Quizzes)
                    Console.WriteLine($"• {q.Title} (Due {q.DueDate:d}){(q.ReminderSent ? "  ✅" : "")}");
                Console.WriteLine("\n1) Add  2) Remove  3) Back");
                var act = Console.ReadLine();
                if (act == "3") continue;

                if (act == "1")
                {
                    Console.Write("Quiz title: ");   var qt = Console.ReadLine() ?? "";
                    Console.Write("Due date (yyyy-MM-dd): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime due))
                    {
                        course.Quizzes.Add(new Quiz { Title = qt, DueDate = due });
                        ctx.SaveChanges();
                    }
                }
                else if (act == "2")
                {
                    for (int i = 0; i < course.Quizzes.Count; i++)
                        Console.WriteLine($"{i + 1}) {course.Quizzes[i].Title}");
                    if (int.TryParse(Console.ReadLine(), out int qi) &&
                        qi >= 1 && qi <= course.Quizzes.Count)
                    {
                        ctx.Quizzes.Remove(course.Quizzes[qi - 1]);
                        ctx.SaveChanges();
                    }
                }
            }
        }

        //────────────────────────────────────────────────────────────────────────

        private static void Pause()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}

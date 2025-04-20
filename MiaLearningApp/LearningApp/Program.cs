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
        private static Timer? _quizTimer;

        static void Main(string[] args)
        {
            _quizTimer = new Timer(CheckQuizDue, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

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
                    case "11": return;
                    default:  Console.WriteLine("Invalid choice."); Pause(); break;
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

        // ───────────────────────── Reminder Timer ────────────────────────────
        private static void CheckQuizDue(object? _)
        {
            using var ctx = new AppDbContext();
            var due = ctx.Quizzes.Include(q => q.Course)
                                 .Where(q => q.DueDate <= DateTime.Now && !q.ReminderSent)
                                 .ToList();
            foreach (var q in due)
            {
                Console.Beep();
                Console.WriteLine($"\n** Reminder ** '{q.Title}' quiz ({q.Course?.Title}) is due!");
                q.ReminderSent = true;
            }
            ctx.SaveChanges();
        }

        // ────────────────────────── Courses  ────────────────────────────────
        private static void ViewCourses()
        {
            Console.Clear();
            using var ctx = new AppDbContext();
            var courses = ctx.Courses.Include(c => c.Notes).ToList();
            if (!courses.Any()) { Console.WriteLine("No courses."); Pause(); return; }

            for (int i = 0; i < courses.Count; i++)
                Console.WriteLine($"{i + 1}) {courses[i].Title} - {courses[i].Progress}%");
            Pause();
        }

        private static void AddCourse()
        {
            Console.Clear();
            Console.Write("Course title (empty to cancel): ");
            var t = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(t)) { Console.WriteLine("Cancelled."); Pause(); return; }

            using var ctx = new AppDbContext();
            if (ctx.Courses.Any(c => c.Title.ToLower() == t.ToLower()))
                Console.WriteLine("Exists!");
            else
            {
                ctx.Courses.Add(new Course { Title = t });
                ctx.SaveChanges();
                Console.WriteLine("Added.");
            }
            Pause();
        }

        private static void UpdateProgress()
        {
            using var ctx = new AppDbContext();
            var list = ctx.Courses.ToList();
            if (!list.Any()) { Console.WriteLine("Add a course first."); Pause(); return; }

            for (int i = 0; i < list.Count; i++)
                Console.WriteLine($"{i + 1}) {list[i].Title} [{list[i].Progress}%]");
            Console.Write("Pick #: ");
            if (!int.TryParse(Console.ReadLine(), out int ix) || ix < 1 || ix > list.Count) return;
            Console.Write("New %: ");
            if (!int.TryParse(Console.ReadLine(), out int p)) return;
            list[ix - 1].Progress = Math.Clamp(p, 0, 100);
            ctx.SaveChanges();
        }

        // ───────────────────────── Notes  ───────────────────────────────────
        private static void ManageNotes()
        {
            Console.Clear();
            Console.WriteLine("1) Add  2) List by Course  3) Search  4) Back");
            var c = Console.ReadLine();
            if (c == "1") AddNote();
            else if (c == "2") ViewNotesByCourse();
            else if (c == "3") SearchNotes();
        }
        private static void AddNote()
        {
            using var ctx = new AppDbContext();
            var courses = ctx.Courses.ToList();
            if (!courses.Any()) { Console.WriteLine("Add course first."); Pause(); return; }
            for (int i = 0; i < courses.Count; i++) Console.WriteLine($"{i + 1}) {courses[i].Title}");
            if (!int.TryParse(Console.ReadLine(), out int ci) || ci < 1 || ci > courses.Count) return;
            Console.Write("Content: ");
            var txt = Console.ReadLine() ?? "";
            ctx.Notes.Add(new Note { Content = txt, CourseId = courses[ci - 1].CourseId });
            ctx.SaveChanges();
        }
        private static void ViewNotesByCourse()
        {
            using var ctx = new AppDbContext();
            var courses = ctx.Courses.Include(c => c.Notes).ToList();
            for (int i = 0; i < courses.Count; i++)
                Console.WriteLine($"{i + 1}) {courses[i].Title} ({courses[i].Notes.Count})");
            if (!int.TryParse(Console.ReadLine(), out int iSel) || iSel < 1 || iSel > courses.Count) return;
            foreach (var n in courses[iSel - 1].Notes) Console.WriteLine($"- {n.Content}");
            Pause();
        }
        private static void SearchNotes()
        {
            Console.Write("Search term: ");
            var t = Console.ReadLine() ?? "";
            using var ctx = new AppDbContext();
            var res = ctx.Notes.Include(n => n.Course)
                               .Where(n => n.Content.Contains(t)).ToList();
            foreach (var n in res)
                Console.WriteLine($"[{n.Course?.Title}] {n.Content}");
            Pause();
        }

        // ───────────────────────── Dashboard & Search  ──────────────────────
        private static void ShowProgressSummary()
        {
            using var ctx = new AppDbContext();
            Console.WriteLine($"Courses: {ctx.Courses.Count()}");
            Console.WriteLine($"Average %: {(ctx.Courses.Any() ? ctx.Courses.Average(c => c.Progress) : 0):F1}");
            Console.WriteLine($"Notes: {ctx.Notes.Count()}");
            Console.WriteLine($"Vocabulary: {ctx.Vocabulary.Count()}");
            Pause();
        }
        private static void GlobalSearch()
        {
            Console.Write("Term: ");
            var t = Console.ReadLine() ?? "";
            using var ctx = new AppDbContext();
            ctx.Courses.Where(c => c.Title.Contains(t)).ToList()
               .ForEach(c => Console.WriteLine($"Course: {c.Title}"));
            ctx.Notes.Where(n => n.Content.Contains(t)).Include(n => n.Course).ToList()
               .ForEach(n => Console.WriteLine($"Note: {n.Content} ({n.Course?.Title})"));
            ctx.Vocabulary.Where(v => v.SourceTerm.Contains(t) || v.TargetTerm.Contains(t)).ToList()
               .ForEach(v => Console.WriteLine($"Vocab: {v.SourceTerm}={v.TargetTerm}"));
            Pause();
        }

        // ───────────────────────── Vocabulary  (unchanged) ───────────────────
        private static void ManageVocabulary() { /* keep your earlier code */ }
        private static void TakeVocabularyQuiz() { /* keep earlier quiz */ }

        // ───────────────────── Modules / Resources / Quizzes  ────────────────
        private static void ManageModules()   { /* code from last response */ }
        private static void ManageResources() { /* code from last response */ }
        private static void ManageQuizzes()   { /* code from last response */ }

        // ───────────────────────── Helpers ───────────────────────────────────
        private static void Pause() { Console.WriteLine("Press Enter…"); Console.ReadLine(); }
    }
}

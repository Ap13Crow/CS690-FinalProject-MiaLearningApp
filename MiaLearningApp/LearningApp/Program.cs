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

        // ───────────────────────── Vocabulary ───────────────────
        private static void ManageVocabulary()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Vocabulary ===");
                Console.WriteLine("1) Add word");
                Console.WriteLine("2) List all");
                Console.WriteLine("3) Search");
                Console.WriteLine("4) Quiz");
                Console.WriteLine("0) Back");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddVocabularyEntry();   break;
                    case "2": ViewAllVocabulary();    break;
                    case "3": SearchVocabulary();     break;
                    case "4": TakeVocabularyQuiz();   break;
                    case "0": return;
                    default:  Console.WriteLine("Bad choice"); Pause(); break;
                }
            }
        }
        private static void AddVocabularyEntry()
        {
            Console.Clear();
            Console.WriteLine("=== Add Vocabulary ===");
            Console.Write("Source language (blank = English): ");
            var src = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(src)) src = "English";

            Console.Write("Target language (blank = Spanish): ");
            var tgt = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(tgt)) tgt = "Spanish";

            Console.Write("Source term (blank to cancel): ");
            var sTerm = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sTerm)) { Console.WriteLine("Cancelled."); Pause(); return; }

            Console.Write("Target term: ");          var tTerm = Console.ReadLine() ?? "";
            Console.Write("Explanation (opt): ");   var expl  = Console.ReadLine() ?? "";
            Console.Write("Example (opt): ");       var ex    = Console.ReadLine() ?? "";

            using var ctx = new AppDbContext();
            ctx.Vocabulary.Add(new Vocabulary
            {
                SourceLanguage = src,
                TargetLanguage = tgt,
                SourceTerm     = sTerm,
                TargetTerm     = tTerm,
                Explanation    = expl,
                Example        = ex,
                Mastered       = false
            });
            ctx.SaveChanges();
            Console.WriteLine("Saved.");
            Pause();
        }
        private static void ViewAllVocabulary()
        {
            Console.Clear();
            using var ctx = new AppDbContext();
            var list = ctx.Vocabulary.ToList();
            if (!list.Any()) { Console.WriteLine("No entries."); Pause(); return; }

            foreach (var v in list)
            {
                var mastered = v.Mastered ? "✅" : "";
                Console.WriteLine($"[{v.SourceLanguage}->{v.TargetLanguage}] {v.SourceTerm} = {v.TargetTerm} {mastered}");
                if (!string.IsNullOrWhiteSpace(v.Explanation)) Console.WriteLine($"   ▸ {v.Explanation}");
                if (!string.IsNullOrWhiteSpace(v.Example))     Console.WriteLine($"   ▸ {v.Example}");
            }
            Pause();
        }

        private static void SearchVocabulary()
        {
            Console.Write("Search term: ");
            var term = Console.ReadLine() ?? "";
            using var ctx = new AppDbContext();
            var hits = ctx.Vocabulary
                        .Where(v => v.SourceTerm.Contains(term) || v.TargetTerm.Contains(term))
                        .ToList();
            if (!hits.Any()) { Console.WriteLine("No match."); Pause(); return; }
            foreach (var v in hits)
                Console.WriteLine($"[{v.SourceLanguage}->{v.TargetLanguage}] {v.SourceTerm} = {v.TargetTerm}");
            Pause();
        }

        private static void TakeVocabularyQuiz()
        {
            Console.Clear();
            Console.WriteLine("=== Vocabulary Quiz ===");
            Console.Write("How many questions? (default 5, 0 to cancel): ");
            var numText = Console.ReadLine();
            if (numText?.Trim() == "0") return;                     // cancel

            int.TryParse(numText, out int questionCount);
            if (questionCount <= 0) questionCount = 5;

            using var ctx = new AppDbContext();
            var vocabList = ctx.Vocabulary
                            .Where(v => !v.Mastered)            // quiz only un‑mastered words
                            .ToList();
            if (!vocabList.Any())
            {
                Console.WriteLine("No vocabulary to quiz on!");
                Pause();
                return;
            }
            if (questionCount > vocabList.Count) questionCount = vocabList.Count;

            var rnd  = new Random();
            var pool = vocabList.OrderBy(_ => rnd.Next())
                                .Take(questionCount)
                                .ToList();

            int correct = 0;
            foreach (var v in pool)
            {
                Console.WriteLine($"\nTranslate from {v.SourceLanguage} to {v.TargetLanguage}: {v.SourceTerm}");
                var ans = Console.ReadLine() ?? "";
                if (ans.Trim().Equals(v.TargetTerm, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("✅ Correct!");
                    v.Mastered = true;                              // mark mastered
                    correct++;
                }
                else
                {
                    Console.WriteLine($"❌ Incorrect. Correct answer: {v.TargetTerm}");
                }
                ctx.SaveChanges();                                 // update mastery flag
            }

            Console.WriteLine($"\nQuiz finished! Score: {correct}/{questionCount}");
            Pause();
        }

        // ───────────────────── Modules / Resources / Quizzes  ────────────────
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
                Console.WriteLine("1) Add   2) Toggle complete   0) Back");
                var sub = Console.ReadLine();
                if (sub == "0") continue;

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
                    Console.WriteLine($"• {r.Url} ({r.Description})");
                Console.WriteLine("\n1) Add   2) Remove   0) Back");
                var act = Console.ReadLine();
                if (act == "0") continue;

                if (act == "1")
                {
                    Console.Write("URL: "); var url = Console.ReadLine() ?? "";
                    Console.Write("Description: "); var desc = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        course.Resources.Add(new Resource { Url = url, Description = desc });
                        ctx.SaveChanges();
                    }
                }
                else if (act == "2")
                {
                    for (int i = 0; i < course.Resources.Count; i++)
                        Console.WriteLine($"{i + 1}) {course.Resources[i].Url}");
                    if (int.TryParse(Console.ReadLine(), out int ri) &&
                        ri >= 1 && ri <= course.Resources.Count)
                    {
                        ctx.Resources.Remove(course.Resources[ri - 1]);
                        ctx.SaveChanges();
                    }
                }
            }
        }
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
                    Console.WriteLine($"• {q.Title} (Due {q.DueDate:d}) {(q.ReminderSent ? "✅" : "")}");
                Console.WriteLine("\n1) Add   2) Remove   0) Back");
                var act = Console.ReadLine();
                if (act == "0") continue;

                if (act == "1")
                {
                    Console.Write("Quiz title: ");
                    var qt = Console.ReadLine() ?? "";
                    Console.Write("Due (yyyy-MM-dd): ");
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

        // ───────────────────────── Helpers ───────────────────────────────────
        private static void Pause() { Console.WriteLine("Press Enter…"); Console.ReadLine(); }
    }
}

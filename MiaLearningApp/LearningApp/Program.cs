using System;
using LearningApp.Data;
using LearningApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // On startup, ensure the database is created (optional if using migrations)
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated(); 
            }

            while (true)
            {
                DisplayMainMenu();
                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        ViewCourses();
                        break;
                    case "2":
                        AddCourse();
                        break;
                    case "3":
                        UpdateProgress();
                        break;
                    case "4":
                        ManageNotes();
                        break;
                    case "5":
                        Console.WriteLine("Dashboard (not implemented yet).");
                        Pause();
                        break;
                    case "6":
                        Console.WriteLine("Exiting Mia Learning App. Goodbye!");
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
            Console.WriteLine("=== Mia Learning App - Main Menu ===");
            Console.WriteLine("1) View Courses and Progress");
            Console.WriteLine("2) Add a New Course");
            Console.WriteLine("3) Update Progress in a Course");
            Console.WriteLine("4) Manage Notes");
            Console.WriteLine("5) View Progress Summary (Dashboard)");
            Console.WriteLine("6) Exit");
            Console.Write("Enter your choice: ");
        }

        private static void ViewCourses()
        {
            Console.Clear();
            Console.WriteLine("=== View Courses ===");

            using (var context = new AppDbContext())
            {
                var courses = context.Courses
                                     .Include(c => c.Notes)
                                     .ToList();

                if (!courses.Any())
                {
                    Console.WriteLine("No courses found.");
                }
                else
                {
                    for (int i = 0; i < courses.Count; i++)
                    {
                        var c = courses[i];
                        Console.WriteLine($"{i + 1}) {c.Name} - {c.Progress}% complete (Notes: {c.Notes.Count})");
                    }

                    Console.WriteLine("\nSelect a course number to see details, or press Enter to go back:");
                    var choice = Console.ReadLine();
                    if (int.TryParse(choice, out int idx) && idx > 0 && idx <= courses.Count)
                    {
                        ViewCourseDetails(courses[idx - 1]);
                    }
                }
            }
            Pause();
        }

        private static void ViewCourseDetails(Course course)
        {
            Console.Clear();
            Console.WriteLine($"=== {course.Name} Details ===");
            Console.WriteLine($"Progress: {course.Progress}%");
            if (course.Notes.Count > 0)
            {
                Console.WriteLine("Notes:");
                foreach (var note in course.Notes)
                {
                    Console.WriteLine($"  - {note.Content}");
                }
            }
            else
            {
                Console.WriteLine("No notes for this course.");
            }
        }

        private static void AddCourse()
        {
            Console.Clear();
            Console.WriteLine("=== Add Course ===");
            Console.Write("Enter a new course name: ");
            var name = Console.ReadLine() ?? "";

            using (var context = new AppDbContext())
            {
                // Check if a course with the same name already exists
                bool exists = context.Courses.Any(c => c.Name.ToLower() == name.ToLower());
                if (exists)
                {
                    Console.WriteLine("A course with this name already exists!");
                }
                else
                {
                    var course = new Course
                    {
                        Name = name,
                        Progress = 0
                    };
                    context.Courses.Add(course);
                    context.SaveChanges();
                    Console.WriteLine($"Course '{name}' created successfully.");
                }
            }
            Pause();
        }

        private static void UpdateProgress()
        {
            Console.Clear();
            Console.WriteLine("=== Update Course Progress ===");

            using (var context = new AppDbContext())
            {
                var courses = context.Courses.ToList();
                if (!courses.Any())
                {
                    Console.WriteLine("No courses found.");
                    Pause();
                    return;
                }

                for (int i = 0; i < courses.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {courses[i].Name} - {courses[i].Progress}%");
                }
                Console.Write("Select a course number: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int index) && index > 0 && index <= courses.Count)
                {
                    var selectedCourse = courses[index - 1];
                    Console.Write($"Enter new progress % for {selectedCourse.Name} (0-100): ");
                    var pInput = Console.ReadLine();
                    if (int.TryParse(pInput, out int newProgress))
                    {
                        selectedCourse.Progress = Math.Max(0, Math.Min(100, newProgress));
                        context.SaveChanges();
                        Console.WriteLine($"{selectedCourse.Name} progress updated to {selectedCourse.Progress}%");
                    }
                    else
                    {
                        Console.WriteLine("Invalid progress input.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            Pause();
        }

        private static void ManageNotes()
        {
            Console.Clear();
            Console.WriteLine("=== Manage Notes ===");
            Console.WriteLine("1) Add a Note");
            Console.WriteLine("2) View Notes by Course");
            Console.WriteLine("3) Search Notes");
            Console.WriteLine("4) Return to Main Menu");
            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddNote();
                    break;
                case "2":
                    ViewNotesByCourse();
                    break;
                case "3":
                    SearchNotes();
                    break;
                default:
                    break; // Return
            }
        }

        private static void AddNote()
        {
            Console.Clear();
            Console.WriteLine("=== Add Note ===");
            using (var context = new AppDbContext())
            {
                var courses = context.Courses.ToList();
                if (!courses.Any())
                {
                    Console.WriteLine("No courses available. Create a course first.");
                    Pause();
                    return;
                }

                for (int i = 0; i < courses.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {courses[i].Name}");
                }
                Console.Write("Select a course number for the note: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int idx) && idx > 0 && idx <= courses.Count)
                {
                    var selectedCourse = courses[idx - 1];
                    Console.Write("Enter the note content: ");
                    var content = Console.ReadLine() ?? "";

                    var note = new Note
                    {
                        Content = content,
                        CourseId = selectedCourse.CourseId
                    };
                    context.Notes.Add(note);
                    context.SaveChanges();
                    Console.WriteLine("Note added successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid course selection.");
                }
            }
            Pause();
        }

        private static void ViewNotesByCourse()
        {
            Console.Clear();
            Console.WriteLine("=== View Notes by Course ===");

            using (var context = new AppDbContext())
            {
                var courses = context.Courses.Include(c => c.Notes).ToList();
                if (!courses.Any())
                {
                    Console.WriteLine("No courses found.");
                    Pause();
                    return;
                }

                for (int i = 0; i < courses.Count; i++)
                {
                    Console.WriteLine($"{i+1}) {courses[i].Name} (Notes: {courses[i].Notes.Count})");
                }
                Console.Write("Select a course to list notes: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int idx) && idx > 0 && idx <= courses.Count)
                {
                    var selected = courses[idx - 1];
                    Console.WriteLine($"\nNotes for {selected.Name}:");
                    foreach (var note in selected.Notes)
                    {
                        Console.WriteLine($"- {note.Content}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            Pause();
        }

        private static void SearchNotes()
        {
            Console.Clear();
            Console.WriteLine("=== Search Notes ===");
            Console.Write("Enter search term: ");
            var term = Console.ReadLine() ?? "";

            using (var context = new AppDbContext())
            {
                var results = context.Notes
                    .Where(n => n.Content.Contains(term))
                    .Include(n => n.Course)
                    .ToList();

                if (!results.Any())
                {
                    Console.WriteLine("No notes match your search.");
                }
                else
                {
                    Console.WriteLine($"Found {results.Count} notes:");
                    foreach (var note in results)
                    {
                        Console.WriteLine($"- [{note.Course?.Name}] {note.Content}");
                    }
                }
            }
            Pause();
        }

        private static void Pause()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}

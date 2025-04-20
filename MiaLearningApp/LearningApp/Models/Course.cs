using System;
using System.Collections.Generic;

namespace LearningApp.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        /// <summary>The title/name of the course.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Short description.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Subject area (e.g., Programming, Language).</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Completion percentage 0‑100.</summary>
        public int Progress { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate   { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public List<Module>   Modules   { get; set; } = new();
        public List<Note>     Notes     { get; set; } = new();
        public List<Resource> Resources { get; set; } = new();
        public List<Quiz>     Quizzes   { get; set; } = new();


        // Quiz reminder
        public DateTime? NextQuizDate { get; set; }
        public bool      QuizNotified { get; set; }
    }
}

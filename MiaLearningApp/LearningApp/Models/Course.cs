using System;
using System.Collections.Generic;

namespace LearningApp.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        /// <summary>
        /// The title/name of this course or learning goal.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// A short description or summary of the course.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// (Optional) Category or subject area, e.g. \"Programming\", \"Language\", etc.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Tracks how far along the user is (0-100%).
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Start date for this course/goal (optional).
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date or target completion date (optional).
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Date/time when the course entry was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date/time when the course entry was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// One-to-many relationship: a Course can have multiple Modules.
        /// </summary>
        public List<Module> Modules { get; set; } = new List<Module>();

        /// <summary>
        /// One-to-many relationship: a Course can have multiple Notes.
        /// </summary>
        public List<Note> Notes { get; set; } = new List<Note>();

        /// <summary>
        /// One-to-many relationship: a Course can have multiple Resources.
        /// </summary>
        public List<Resource> Resources { get; set; } = new List<Resource>();
    }
}

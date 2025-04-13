using System;
using System.Collections.Generic;

namespace LearningApp.Models
{
    public class Note
    {
        public int NoteId { get; set; }

        /// <summary>
        /// Optional short title or subject of the note.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The main text/content of the note.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Date/time the note was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date/time of last update to the note.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Many-to-one relationship: A Note can be associated with a specific Course.
        /// 
        /// If your wiki suggests a note might be linked to a Module specifically,
        /// you could also add ModuleId here instead or in addition to CourseId.
        /// </summary>
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        /// <summary>
        /// Many-to-many relationship with Tag using EF Core 5+ feature.
        /// One note can have multiple tags, one tag can belong to multiple notes.
        /// </summary>
        public List<Tag> Tags { get; set; } = new();
    }
}

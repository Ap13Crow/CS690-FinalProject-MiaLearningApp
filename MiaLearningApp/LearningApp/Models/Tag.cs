using System.Collections.Generic;

namespace LearningApp.Models
{
    public class Tag
    {
        public int TagId { get; set; }

        /// <summary>
        /// The tag text or name, e.g. \"Programming\", \"Grammar\", \"Tips\", etc.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Many-to-many relationship with Note.
        /// EF Core can handle this without an explicit join entity if using EF Core 5+.
        /// </summary>
        public List<Note> Notes { get; set; } = new();
    }
}

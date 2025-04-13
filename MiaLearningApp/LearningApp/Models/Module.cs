using System;

namespace LearningApp.Models
{
    public class Module
    {
        public int ModuleId { get; set; }

        /// <summary>
        /// Title or name of this module/lesson/section.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Brief description of the module contents or objectives.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// If the user wants to track time or complexity, store an estimate here.
        /// </summary>
        public int EstimatedHours { get; set; }

        /// <summary>
        /// True when the module has been completed by the user.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Optional due date if the user wants to set a deadline for this module.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// The Course that this module belongs to.
        /// </summary>
        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }
}

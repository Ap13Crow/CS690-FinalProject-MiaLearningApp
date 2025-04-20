using System;

namespace LearningApp.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }

        /// <summary>
        /// Title or short name for this resource.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional short description (e.g., what is this resource about).
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Link to the external resource (URL) or local file path.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Optional: categorize resource (e.g. \"Video\", \"Document\", \"Website\", etc.).
        /// </summary>
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>
        /// Date/time the resource was added to the system.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// (Optional) If resources are associated with a module specifically, add ModuleId here.
        /// For now, we'll associate them to a Course by default.
        /// </summary>
        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }
}

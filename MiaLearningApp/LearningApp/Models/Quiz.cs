namespace LearningApp.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool ReminderSent { get; set; }
        public int CourseId { get; set; }
        public Course? Course { get; set; }
    }
}

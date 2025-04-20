using Microsoft.EntityFrameworkCore;
using LearningApp.Models;

namespace LearningApp.Data
{
    public class AppDbContext : DbContext
    {
        // 1) Parameterless constructor for your main app (if you need OnConfiguring):
        public AppDbContext() { }

        // 2) Constructor for passing options in unit tests (or dependency injection):
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Existing override - uses SQLite if no options are provided:
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=MiaLearning.db");
            }
        }// Declare tables as DbSets
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Resource> Resources => Set<Resource>();
        public DbSet<Vocabulary> Vocabulary => Set<Vocabulary>();
        public DbSet<Quiz>      Quizzes    => Set<Quiz>();

    }
}

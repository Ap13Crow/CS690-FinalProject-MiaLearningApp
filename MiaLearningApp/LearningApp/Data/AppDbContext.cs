using Microsoft.EntityFrameworkCore;
using LearningApp.Models;

namespace LearningApp.Data
{
    public class AppDbContext : DbContext
    {
        // Declare tables as DbSets
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Resource> Resources => Set<Resource>();

        // Option A: override OnConfiguring (simplest for a console app)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Store data in local SQLite file: "MiaLearning.db"
                optionsBuilder.UseSqlite("Data Source=MiaLearning.db");
            }
        }
    }
}

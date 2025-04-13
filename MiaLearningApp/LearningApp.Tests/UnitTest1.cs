using Xunit;
using LearningApp.Data;
using LearningApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LearningApp.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CanAddCourseToDatabase()
        {
            // Use an in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_DB")
                .Options;

            // Arrange
            using (var context = new AppDbContext(options))
            {
                var course = new Course { Name = "Test Course" };

                // Act
                context.Courses.Add(course);
                context.SaveChanges();
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                var savedCourse = context.Courses.FirstOrDefault(c => c.Name == "Test Course");
                Assert.NotNull(savedCourse);
            }
        }
    }
}

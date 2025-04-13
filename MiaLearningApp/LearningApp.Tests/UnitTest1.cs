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
                var course = new Course { Title = "Test Course" };

                // Act
                context.Courses.Add(course);
                context.SaveChanges();
            }

            // Assert
            using (var context = new AppDbContext(options))
            {
                var savedCourse = context.Courses.FirstOrDefault(c => c.Title == "Test Course");
                Assert.NotNull(savedCourse);
            }
        }

        [Fact]
        public void CanAddNoteToCourseAndRetrieveIt()
        {
            // Setup a separate in-memory database for this test
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_DB_Notes")
                .Options;

            // Arrange & Act: Add a course and a note associated with it
            using (var context = new AppDbContext(options))
            {
                var course = new Course { Title = "Course for NoteTest" };
                context.Courses.Add(course);
                context.SaveChanges();

                var note = new Note { Content = "This is a test note", CourseId = course.CourseId };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            // Assert: Retrieve and verify the course and note
            using (var context = new AppDbContext(options))
            {
                var courseWithNotes = context.Courses.Include(c => c.Notes)
                                        .FirstOrDefault(c => c.Title == "Course for NoteTest");
                Assert.NotNull(courseWithNotes);
                Assert.Single(courseWithNotes.Notes);
                Assert.Equal("This is a test note", courseWithNotes.Notes[0].Content);

                var fetchedNote = context.Notes.Include(n => n.Course)
                                    .FirstOrDefault(n => n.Content == "This is a test note");
                Assert.NotNull(fetchedNote);
                Assert.NotNull(fetchedNote.Course);
                Assert.Equal("Course for NoteTest", fetchedNote.Course.Title);
            }
        }
    }
}

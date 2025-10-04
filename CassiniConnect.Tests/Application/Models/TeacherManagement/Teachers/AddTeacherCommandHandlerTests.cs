using Microsoft.EntityFrameworkCore;
using CassiniConnect.Application.Models.TeacherManagement.Teachers;
using CassiniConnect.Core.Models.Helpers;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Utilities.DTO;
using CassiniConnect.Core.Persistance;

namespace CassiniConnect.Tests.Application.Models.TeacherManagement.Teachers
{
    public class AddTeacherCommandHandlerTests
    {
        private readonly DataContext dataContext;
        private readonly AddTeacher.AddTeacherCommandHandler handler;

        public AddTeacherCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            dataContext = new DataContext(options);
            handler = new AddTeacher.AddTeacherCommandHandler(dataContext);
        }

        [Fact]
        public async Task Handle_ShouldAddTeacher_WhenValidRequest()
        {
            var userId = Guid.NewGuid();
            dataContext.LanguageCodes.Add(new LanguageCode { Id = Guid.NewGuid(), Code = "en" });
            dataContext.Subjects.Add(new Subject { Id = Guid.NewGuid(), Code = "MATH" });
            await dataContext.SaveChangesAsync();

            var command = new AddTeacher.AddTeacherCommand
            {
                UserId = userId,
                Rate = 50,
                Image = "image.jpg",
                Descriptions = new List<TeacherDescriptionDTO>
                {
                    new TeacherDescriptionDTO { LanguageCode = "en", Description = "Math Teacher" }
                },
                Subjects = new List<string> { "MATH" }
            };

            await handler.Handle(command, CancellationToken.None);

            var teacher = await dataContext.Teachers.Include(t => t.TeacherDescriptions).Include(t => t.Subjects).FirstOrDefaultAsync(t => t.UserId == userId);
            Assert.NotNull(teacher);
            Assert.Equal(command.Rate, teacher.Rate);
            Assert.Equal(command.Image, teacher.Image);
            Assert.Single(teacher.TeacherDescriptions);
            Assert.Single(teacher.Subjects);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTeacherAlreadyExists()
        {
            var userId = Guid.NewGuid();
            dataContext.Teachers.Add(new Teacher { Id = Guid.NewGuid(), UserId = userId });
            await dataContext.SaveChangesAsync();

            var command = new AddTeacher.AddTeacherCommand { UserId = userId };
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenLanguageNotFound()
        {
            var command = new AddTeacher.AddTeacherCommand
            {
                UserId = Guid.NewGuid(),
                Descriptions = new List<TeacherDescriptionDTO>
                {
                    new TeacherDescriptionDTO { LanguageCode = "fr", Description = "French Teacher" }
                }
            };
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenSubjectNotFound()
        {
            var command = new AddTeacher.AddTeacherCommand
            {
                UserId = Guid.NewGuid(),
                Subjects = new List<string> { "PHYS" }
            };
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
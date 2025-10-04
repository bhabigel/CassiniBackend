using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Application.Models.TeacherManagement.Teachers;
using CassiniConnect.Core.Models.Helpers;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Models.UserCore;
using CassiniConnect.Core.Persistance;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Tests.Application.Models.TeacherManagement.Teachers
{
    public class ListTeachersCommandHandlerTests
    {
        private readonly DataContext dataContext;
        private readonly ListTeachers.ListTeacherCommandHandler handler;

        public ListTeachersCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            dataContext = new DataContext(options);
            handler = new ListTeachers.ListTeacherCommandHandler(dataContext);
        }

        [Fact]
        public async Task Handle_ShouldReturnTeacherDescription_WhenLanguageExists()
        {
            var language = new LanguageCode { Id = Guid.NewGuid(), Code = "en" };
            await dataContext.LanguageCodes.AddAsync(language);

            var user = new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            await dataContext.Users.AddAsync(user);

            var teacher = new Teacher { Id = Guid.NewGuid(), User = user, Image = "image.jpg" };
            await dataContext.Teachers.AddAsync(teacher);

            var description = new TeacherDescription
            {
                Id = Guid.NewGuid(),
                TeacherId = teacher.Id,
                LanguageId = language.Id,
                Description = "Math Teacher"
            };

            await dataContext.TeacherDescriptions.AddAsync(description);
            await dataContext.SaveChangesAsync();

            var desc = await dataContext.TeacherDescriptions
                .FirstOrDefaultAsync(d => d.LanguageId == language.Id);
            Assert.NotNull(desc);
            Assert.Equal("Math Teacher", desc.Description);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenLanguageNotFound()
        {
            var command = new ListTeachers.ListTeacherCommand { LanguageCode = "fr" };
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
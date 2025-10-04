using Microsoft.EntityFrameworkCore;
using CassiniConnect.Application.Models.TeacherManagement.Teachers;
using CassiniConnect.Core.Models.Helpers;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Utilities.DTO;
using CassiniConnect.Core.Persistance;

namespace CassiniConnect.Tests.Application.Models.TeacherManagement.Teachers
{
    public class DeleteTeacherCommandHandlerTests
    {
        private readonly DataContext dataContext;
        private readonly DeleteTeacher.DeleteTeacherCommandHandler handler;

        public DeleteTeacherCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            dataContext = new DataContext(options);
            handler = new DeleteTeacher.DeleteTeacherCommandHandler(dataContext);
        }

        [Fact]
        public async Task Handle_ShouldDeleteTeacher_WhenTeacherExists()
        {
            var teacher = new Teacher { Id = Guid.NewGuid() };
            dataContext.Teachers.Add(teacher);
            await dataContext.SaveChangesAsync();

            var command = new DeleteTeacher.DeleteTeacherCommand { Id = teacher.Id };

            await handler.Handle(command, CancellationToken.None);

            var deletedTeacher = await dataContext.Teachers.FindAsync(teacher.Id);
            Assert.Null(deletedTeacher);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTeacherDoesNotExist()
        {
            var command = new DeleteTeacher.DeleteTeacherCommand { Id = Guid.NewGuid() };
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
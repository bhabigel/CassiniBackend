using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Application.Models.TeacherManagement.Teachers;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Tests.Application.Models.TeacherManagement.Teachers
{
    public class UpdateTeacherCommandHandlerTests
    {
        private readonly DataContext dataContext;
        private readonly UpdateTeacher.UpdateTeacherCommandHandler handler;

        public UpdateTeacherCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            dataContext = new DataContext(options);
            handler = new UpdateTeacher.UpdateTeacherCommandHandler(dataContext);
        }

        [Fact]
        public async Task Handle_ShouldUpdateTeacher_WhenValidDataProvided()
        {
            var teacher = new Teacher { Id = Guid.NewGuid(), Rate = 20, Image = "old.jpg" };
            dataContext.Teachers.Add(teacher);
            await dataContext.SaveChangesAsync();

            var command = new UpdateTeacher.UpdateTeacherCommand { Id = teacher.Id, Rate = 30, Image = "new.jpg" };

            var result = await handler.Handle(command, CancellationToken.None);

            var updatedTeacher = await dataContext.Teachers.FindAsync(teacher.Id);
            Assert.True(result);
            Assert.Equal(30, updatedTeacher.Rate);
            Assert.Equal("new.jpg", updatedTeacher.Image);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenNoFieldsToUpdate()
        {
            var teacher = new Teacher { Id = Guid.NewGuid(), Rate = 20, Image = "old.jpg" };
            dataContext.Teachers.Add(teacher);
            await dataContext.SaveChangesAsync();

            var command = new UpdateTeacher.UpdateTeacherCommand { Id = teacher.Id };

            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTeacherNotFound()
        {
            var command = new UpdateTeacher.UpdateTeacherCommand { Id = Guid.NewGuid(), Rate = 25 };
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRateIsNegative()
        {
            var teacher = new Teacher { Id = Guid.NewGuid(), Rate = 20, Image = "old.jpg" };
            dataContext.Teachers.Add(teacher);
            await dataContext.SaveChangesAsync();

            var command = new UpdateTeacher.UpdateTeacherCommand { Id = teacher.Id, Rate = -5 };

            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
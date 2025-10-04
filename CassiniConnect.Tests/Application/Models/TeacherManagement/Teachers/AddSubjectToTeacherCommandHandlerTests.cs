using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Application.Models.TeacherManagement.Teachers;
using System.Linq;
using CassiniConnect.Core.Models.Teaching;

namespace CassiniConnect.Tests.Application.Models.TeacherManagement.Teachers
{

    public class AddSubjectToTeacherCommandHandlerTests
    {
        private readonly DataContext dataContext;
        private readonly AddSubjectToTeacher.AddSubjectToTeacherCommandHandler handler;

        public AddSubjectToTeacherCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            dataContext = new DataContext(options);
            handler = new AddSubjectToTeacher.AddSubjectToTeacherCommandHandler(dataContext);
        }

        [Fact]
        public async Task Handle_ShouldAddSubjectToTeacher_WhenValidRequest()
        {
            var teacher = new Teacher { Id = Guid.NewGuid(), Subjects = new List<Subject>() };
            var subject = new Subject { Id = Guid.NewGuid() };
            dataContext.Teachers.Add(teacher);
            dataContext.Subjects.Add(subject);
            await dataContext.SaveChangesAsync();

            var command = new AddSubjectToTeacher.AddSubjectToTeacherCommand
            {
                TeacherId = teacher.Id,
                SubjectId = subject.Id
            };

            await handler.Handle(command, CancellationToken.None);

            var updatedTeacher = await dataContext.Teachers.Include(t => t.Subjects).FirstOrDefaultAsync(t => t.Id == teacher.Id);
            Assert.NotNull(updatedTeacher);
            Assert.Contains(updatedTeacher.Subjects, s => s.Id == subject.Id);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenTeacherNotFound()
        {
            var command = new AddSubjectToTeacher.AddSubjectToTeacherCommand
            {
                TeacherId = Guid.NewGuid(),
                SubjectId = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenSubjectNotFound()
        {
            var teacher = new Teacher { Id = Guid.NewGuid(), Subjects = new List<Subject>() };
            dataContext.Teachers.Add(teacher);
            await dataContext.SaveChangesAsync();

            var command = new AddSubjectToTeacher.AddSubjectToTeacherCommand
            {
                TeacherId = teacher.Id,
                SubjectId = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenSubjectAlreadyAssigned()
        {
            var teacher = new Teacher { Id = Guid.NewGuid(), Subjects = new List<Subject>() };
            var subject = new Subject { Id = Guid.NewGuid() };
            teacher.Subjects.Add(subject);
            dataContext.Teachers.Add(teacher);
            dataContext.Subjects.Add(subject);
            await dataContext.SaveChangesAsync();

            var command = new AddSubjectToTeacher.AddSubjectToTeacherCommand
            {
                TeacherId = teacher.Id,
                SubjectId = subject.Id
            };

            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
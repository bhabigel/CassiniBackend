using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Teachers
{
    public class AddSubjectToTeacher
    {
        public class AddSubjectToTeacherCommand : IRequest<Unit>
        {
            public Guid TeacherId { get; set; }
            public Guid SubjectId { get; set; }
        }

        public class AddSubjectToTeacherCommandHandler : IRequestHandler<AddSubjectToTeacherCommand, Unit>
        {
            private readonly DataContext dataContext;
            public AddSubjectToTeacherCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(AddSubjectToTeacherCommand command, CancellationToken cancellationToken)
            {
                var teacher = await dataContext.Teachers.Include(t => t.Subjects).FirstOrDefaultAsync(t => t.Id == command.TeacherId, cancellationToken);
                if (teacher == null)
                {
                    throw new Exception("Teacher with given id not found.");
                }

                var subject = await dataContext.Subjects.FindAsync(command.SubjectId);
                if (subject == null)
                {
                    throw new Exception("Subject with given id not found.");
                }

                if (teacher.Subjects.Any(s => s.Id == command.SubjectId))
                {
                    throw new Exception("Subject is already assigned to the teacher.");
                }

                teacher.Subjects.Add(subject);
                await dataContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
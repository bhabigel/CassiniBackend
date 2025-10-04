using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Application.Utilities;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Teachers
{
    public class DeleteTeacher
    {
        public class DeleteTeacherCommand : IRequest<Unit>
        {
            public Guid Id { get; set; }
        }

        public class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand, Unit>
        {
            private readonly DataContext dataContext;
            public DeleteTeacherCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(DeleteTeacherCommand command, CancellationToken cancellationToken)
            {
                var teacher = await dataContext.Teachers.FirstOrDefaultAsync(t => t.Id == command.Id);
                if (teacher == null)
                {
                    throw new Exception("Teacher not found with given id!");
                }

                dataContext.Teachers.Remove(teacher);
                await dataContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
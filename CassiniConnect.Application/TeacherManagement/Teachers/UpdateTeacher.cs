using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.TeacherManagement.Teachers
{
    public class UpdateTeacher
    {
        public class UpdateTeacherCommand : IRequest<bool>
        {
            public Guid Id { get; set; }
            public float? Rate { get; set; }
            public string? Image { get; set; }
        }

        public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, bool>
        {
            private readonly DataContext dataContext;
            public UpdateTeacherCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<bool> Handle(UpdateTeacherCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(command.Image) && !command.Rate.HasValue)
                {
                    throw new Exception("No field to update, everything is null!");
                }

                var teacher = await dataContext.Teachers.FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);
                if (teacher == null)
                {
                    throw new Exception("Teacher not found by the given id!");
                }

                if(command.Rate.HasValue)
                {
                    teacher.Rate = command.Rate.Value;
                }

                if(!string.IsNullOrWhiteSpace(command.Image))
                {
                    teacher.Image = command.Image;
                }

                await dataContext.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
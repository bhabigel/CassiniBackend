using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Teachers
{
    public class UpdateTeacherDescription
    {
        public class UpdateTeacherDescriptionCommand: IRequest<bool>
        {
            public Guid Id { get; set; }
            public string? Description { get; set; }
        }

        public class UpdateTeacherDescriptionCommandHandler : IRequestHandler<UpdateTeacherDescriptionCommand, bool>
        {
            private readonly DataContext dataContext;
            public UpdateTeacherDescriptionCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<bool> Handle(UpdateTeacherDescriptionCommand command, CancellationToken cancellationToken)
            {
                if(string.IsNullOrEmpty(command.Description))
                {
                    throw new Exception("Updated description is null or empty!");
                }

                var teacherDescription = await dataContext.TeacherDescriptions.FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken);
                if (teacherDescription == null)
                {
                    throw new Exception("Teacher description with given id was not found!");
                }

                teacherDescription.Description = command.Description;
                await dataContext.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
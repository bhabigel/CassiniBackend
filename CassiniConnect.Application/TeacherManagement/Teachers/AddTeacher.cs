using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.TeacherManagement.Teachers
{
    public class AddTeacher
    {
        public class AddTeacherCommand : IRequest<Guid>
        {
            public Guid UserId { get; set; }
            public float Rate { get; set; }
            public string Image { get; set; } = string.Empty;
            public List<TeacherDescriptionDTO> Descriptions { get; set; } = [];
        }

        public class AddTeacherCommandHandler : IRequestHandler<AddTeacherCommand, Guid>
        {
            private readonly DataContext dataContext;
            public AddTeacherCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Guid> Handle(AddTeacherCommand command, CancellationToken cancellationToken)
            {
                var userExists = await dataContext.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
                if (!userExists)
                {
                    throw new Exception("No user found with given id!");
                }

                foreach (var desc in command.Descriptions)
                {

                    var languageExists = await dataContext.LanguageCodes.AnyAsync(l => l.Id == desc.LanguageId, cancellationToken);
                    if (!languageExists)
                    {
                        throw new Exception("Language not found with given id!");
                    }
                }

                var teacher = new Teacher
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Rate = command.Rate,
                    Image = command.Image
                };

                var descriptions = new List<TeacherDescription>();
                foreach (var desc in command.Descriptions)
                {
                    var description = new TeacherDescription
                    {
                        Id = Guid.NewGuid(),
                        LanguageId = desc.LanguageId,
                        Description = desc.Description
                    };

                    descriptions.Add(description);
                    await dataContext.TeacherDescriptions.AddAsync(description);
                }

                teacher.TeacherDescriptions = descriptions;
                await dataContext.Teachers.AddAsync(teacher, cancellationToken);
                await dataContext.SaveChangesAsync(cancellationToken);

                return teacher.Id;
            }
        }
    }
}
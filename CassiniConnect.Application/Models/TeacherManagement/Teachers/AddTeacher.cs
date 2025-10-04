using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Teachers
{
    public class AddTeacher
    {
        public class AddTeacherCommand : IRequest<Unit>
        {
            public Guid UserId { get; set; }
            public float Rate { get; set; }
            public string Image { get; set; } = string.Empty;
            public List<TeacherDescriptionDTO> Descriptions { get; set; } = [];
            public List<string> Subjects { get; set; } = [];
        }

        public class AddTeacherCommandHandler : IRequestHandler<AddTeacherCommand, Unit>
        {
            private readonly DataContext dataContext;
            public AddTeacherCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(AddTeacherCommand command, CancellationToken cancellationToken)
            {
                var teacherExists = await dataContext.Teachers.AnyAsync(t => t.UserId == command.UserId, cancellationToken);
                if (teacherExists)
                {
                    throw new Exception("Teacher associated with given user already exists!");
                }

                foreach (var desc in command.Descriptions)
                {

                    var languageExists = await dataContext.LanguageCodes.AnyAsync(l => l.Code == desc.LanguageCode, cancellationToken);
                    if (!languageExists)
                    {
                        throw new Exception("Language not found with given code!");
                    }
                }

                var teacher = new Teacher
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Rate = command.Rate,
                    Image = command.Image
                };

                await dataContext.Teachers.AddAsync(teacher, cancellationToken);
                var descriptions = new List<TeacherDescription>();
                foreach (var desc in command.Descriptions)
                {
                    var languageId = await dataContext.LanguageCodes.Where(l => l.Code == desc.LanguageCode).Select(l => l.Id).FirstOrDefaultAsync(cancellationToken);
                    var description = new TeacherDescription
                    {
                        Id = Guid.NewGuid(),
                        TeacherId = teacher.Id,
                        LanguageId = languageId,
                        Description = desc.Description
                    };

                    descriptions.Add(description);
                    await dataContext.TeacherDescriptions.AddAsync(description, cancellationToken);
                }

                var subjects = new List<Subject>();
                foreach(var subj in command.Subjects)
                {
                    var subject = await dataContext.Subjects.Where(s => s.Code == subj).FirstOrDefaultAsync(cancellationToken);
                    if(subject == null)
                    {
                        throw new Exception("No subject with given code!");
                    }
                    subjects.Add(subject);
                }

                teacher.TeacherDescriptions = descriptions;
                teacher.Subjects = subjects;
                await dataContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
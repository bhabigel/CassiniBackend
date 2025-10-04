using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Subjects
{
    public class AddSubject
    {
        public class AddSubjectCommand : IRequest<Unit>
        {
            public string SubjectCode = string.Empty;
            public List<SubjectNameDTO> SubjectNames { get; set; } = [];
        }

        public class AddSubjectCommandHandler : IRequestHandler<AddSubjectCommand, Unit>
        {
            private readonly DataContext dataContext;

            public AddSubjectCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(AddSubjectCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(command.SubjectCode) || command.SubjectNames == null)
                {
                    throw new Exception("One or more of the obligatory fields!");
                }

                var subjectExists = await dataContext.Subjects.AnyAsync(s => s.Code == command.SubjectCode);
                if (subjectExists)
                {
                    throw new Exception("Subject already added!");
                }

                foreach (var subjectName in command.SubjectNames)
                {
                    var languageExists = await dataContext.LanguageCodes.AnyAsync(l => l.Id == subjectName.LanguageId);
                    if (!languageExists)
                    {
                        throw new Exception("Language not found with given id!");
                    }
                }

                var subject = new Subject
                {
                    Id = Guid.NewGuid(),
                    Code = command.SubjectCode
                };

                await dataContext.Subjects.AddAsync(subject, cancellationToken);
                var subjectNames = new List<SubjectName>();
                foreach (var name in command.SubjectNames)
                {
                    var subjectName = new SubjectName
                    {
                        Id = Guid.NewGuid(),
                        SubjectId = subject.Id,
                        LanguageId = name.LanguageId,
                        Name = name.SubjectName
                    };
                    subjectNames.Add(subjectName);
                    await dataContext.SubjectNames.AddAsync(subjectName, cancellationToken);
                }
                subject.SubjectNames = subjectNames;
                await dataContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
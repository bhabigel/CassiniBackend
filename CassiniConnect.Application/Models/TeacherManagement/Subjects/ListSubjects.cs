using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Subjects
{
    public class ListSubjects
    {
        public class ListSubjectsRequest : IRequest<List<string>>
        {
            public string LanguageCode { get; set; } = string.Empty;
        }

        public class ListSubjectsRequestHandler : IRequestHandler<ListSubjectsRequest, List<string>>
        {
            private readonly DataContext dataContext;

            public ListSubjectsRequestHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<List<string>> Handle(ListSubjectsRequest request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(request.LanguageCode))
                {
                    throw new Exception("One or more of the obligatory fields are empty!");
                }

                var language = await dataContext.LanguageCodes.FirstOrDefaultAsync(x => x.Code == request.LanguageCode, cancellationToken);
                if (language == null)
                {
                    throw new Exception("No language found by given code!");
                }
                var subjectNames = await dataContext.Subjects.Include(s => s.SubjectNames)
                                                         .Select(s => s.SubjectNames.Where(sn => sn.Language.Code == request.LanguageCode).FirstOrDefault())
                                                         .ToListAsync(cancellationToken);
                if (subjectNames == null)
                {
                    throw new Exception("No subject was found!");
                }

                foreach (var s in subjectNames)
                {
                    if (s == null)
                    {
                        throw new Exception("Some subjectname is null!");
                    }
                }
                var subjects = subjectNames.Select(s => s.Name).ToList();

                return subjects;
            }
        }
    }
}
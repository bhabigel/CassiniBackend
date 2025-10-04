using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Subjects
{
    public class ListSubjectCodes
    {
        public class ListSubjectCodesRequest : IRequest<List<SubjectCode>>
        {
            public string LanguageCode { get; set; } = string.Empty;
        }

        public class ListSubjectCodesRequestHandler : IRequestHandler<ListSubjectCodesRequest, List<SubjectCode>>
        {
            private readonly DataContext dataContext;
            public ListSubjectCodesRequestHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<List<SubjectCode>> Handle(ListSubjectCodesRequest request, CancellationToken cancellationToken)
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
                var codes = await dataContext.Subjects
                    .Include(s => s.SubjectNames)
                    .Select(s => new SubjectCode
                    {
                        Code = s.Code,
                        Name = s.SubjectNames.Where(sn => sn.Language.Code == request.LanguageCode).Select(sn => sn.Name).FirstOrDefault()
                    })
                    .ToListAsync(cancellationToken);
                if (codes == null)
                {
                    throw new Exception("No subject was found!");
                }
                return codes;
            }
        }
    }
}
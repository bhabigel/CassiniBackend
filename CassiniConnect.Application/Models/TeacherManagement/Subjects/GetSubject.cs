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
    public class GetSubject
    {
        public class GetSubjectRequest : IRequest<Subject>
        {
            public string Code { get; set; } = string.Empty;
        }

        public class GetSubjectRequestHandler : IRequestHandler<GetSubjectRequest, Subject>
        {
            private readonly DataContext dataContext;
            public GetSubjectRequestHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Subject> Handle(GetSubjectRequest request, CancellationToken cancellationToken)
            {
                var subject = await dataContext.Subjects.Include(s => s.Teachers)
                                                        .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);
                if (subject == null)
                {
                    throw new Exception("Subject with given code does not exist!");
                }
                return subject;
            }
        }
    }
}
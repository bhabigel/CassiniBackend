using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.TeacherManagement.Subjects
{
    public class GetSubject
    {
        public class GetSubjectRequest : IRequest<Subject>
        {
            public Guid Id { get; set; }
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
                                                        .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
                if (subject == null)
                {
                    throw new Exception("Tantárgy nem található a megadott azonosító alatt!");
                }
                return subject;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using MediatR;

namespace CassiniConnect.Application.TeacherManagement.Subjects
{
    public class AddSubject
    {
        public class AddSubjectCommand : IRequest<Guid>
        {
            public string Name { get; set; } = string.Empty;
        }

        public class AddSubjectCommandHandler : IRequestHandler<AddSubjectCommand, Guid>
        {
            private readonly DataContext dataContext;

            public AddSubjectCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Guid> Handle(AddSubjectCommand request, CancellationToken cancellationToken)
            {
                if(String.IsNullOrEmpty(request.Name))
                {
                    throw new Exception("Tantárgy elnevezése üres!");
                }

                var subject = new Subject
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };

                await dataContext.Subjects.AddAsync(subject);
                await dataContext.SaveChangesAsync(cancellationToken);

                return subject.Id;
            }
        }
    }
}
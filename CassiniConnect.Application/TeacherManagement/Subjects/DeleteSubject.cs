using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.TeacherManagement.Subjects
{
    public class DeleteSubject
    {
        public class DeleteSubjectCommand : IRequest<bool>
        {
            public Guid Id { get; set; }
        }

        public class DeleteSubjectCommandHandler:IRequestHandler<DeleteSubjectCommand, bool>
        {
            private readonly DataContext dataContext;
            public DeleteSubjectCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<bool> Handle(DeleteSubjectCommand command, CancellationToken cancellationToken)
            {
                var subject = await dataContext.Subjects.FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);
                if (subject == null)
                {
                    throw new Exception("Subject with given id does not exist!");
                }

                dataContext.Subjects.Remove(subject);
                await dataContext.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
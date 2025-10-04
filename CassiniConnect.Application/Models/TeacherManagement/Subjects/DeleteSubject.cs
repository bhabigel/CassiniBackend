using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Subjects
{
    public class DeleteSubject
    {
        public class DeleteSubjectCommand : IRequest<Unit>
        {
            public string Code { get; set; } = string.Empty;
        }

        public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, Unit>
        {
            private readonly DataContext dataContext;
            public DeleteSubjectCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(DeleteSubjectCommand command, CancellationToken cancellationToken)
            {
                var subject = await dataContext.Subjects.FirstOrDefaultAsync(s => s.Code == command.Code, cancellationToken);
                if (subject == null)
                {
                    throw new Exception("Subject with given code does not exist!");
                }

                dataContext.Subjects.Remove(subject);
                await dataContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
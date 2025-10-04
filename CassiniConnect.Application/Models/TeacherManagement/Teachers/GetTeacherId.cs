using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Teachers
{
    public class GetTeacherId
    {
        public class GetTeacherIdRequest : IRequest<Guid>
        {
            public Guid UserId { get; set; }
        }

        public class GetTeacherIdRequestHandler : IRequestHandler<GetTeacherIdRequest, Guid>
        {
            private DataContext dataContext;
            public GetTeacherIdRequestHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Guid> Handle(GetTeacherIdRequest request, CancellationToken cancellationToken)
            {
                var user = await dataContext.Teachers.Include(s => s.UserId == request.UserId).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception("No teacher was found by given user!");
                }

                return user.Id;
            }
        }
    }
}
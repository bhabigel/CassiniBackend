using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.TeacherManagement.Teachers
{
    public class ListTeachers
    {
        public class ListTeacherCommand : IRequest<List<Teacher>>
        {
        }

        public class ListTeacherCommandHandler : IRequestHandler<ListTeacherCommand, List<Teacher>>
        {
            private readonly DataContext dataContext;
            public ListTeacherCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<List<Teacher>> Handle(ListTeacherCommand request, CancellationToken cancellationToken)
            {
                var teachers = await dataContext.Teachers.ToListAsync();
                if (teachers == null)
                {
                    throw new Exception("No teacher was found in the database!");
                }   
                return teachers;
            }
        }
    }
}
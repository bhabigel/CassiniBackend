using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.TeacherManagement.Teachers
{
    public class FilterTeachers
    {
        public class FilterTeachersRequest: IRequest<List<TeacherDTO>>
        {
            public string LanguageCode { get; set; } = string.Empty;
            public string NameSearch { get; set; } = string.Empty;
        }

        public class FilterTeachersRequestHandler: IRequestHandler<FilterTeachersRequest, List<TeacherDTO>>
        {
            private readonly DataContext dataContext;
            public FilterTeachersRequestHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<List<TeacherDTO>> Handle(FilterTeachersRequest request, CancellationToken cancellationToken)
            {
                var teachers = await dataContext.Teachers
                .Include(t => t.TeacherDescriptions)
                .Include(t => t.User)
                .Include(t => t.Subjects).ThenInclude(s => s.SubjectNames)
                .Where(t => t.User.LastName.Contains(request.NameSearch) || t.User.FirstName.Contains(request.NameSearch))
                .ToListAsync();

                if (teachers == null)
                {
                    throw new Exception("No teacher was found in the database!");
                }

                var language = await dataContext.LanguageCodes.FirstOrDefaultAsync(x => x.Code == request.LanguageCode, cancellationToken);
                if (language == null)
                {
                    throw new Exception("No language found by given code!");
                }

                var teacherDTO = new List<TeacherDTO>();

                foreach (var teacher in teachers)
                {
                    var correctDesc = teacher.TeacherDescriptions.Where(d => d.LanguageId == language.Id).FirstOrDefault();
                    var correctSubjectNames = teacher.Subjects.Select(s => s.SubjectNames.Where(n => n.LanguageId == language.Id).FirstOrDefault());

                    if (correctDesc == null)
                    {
                        throw new Exception("Description not found wih given language!");
                    }

                    if (correctSubjectNames == null)
                    {
                        throw new Exception("Subjectnames not found with given language!");
                    }
                    else
                    {
                        foreach (var subjectName in correctSubjectNames)
                        {
                            if (subjectName == null)
                            {
                                throw new Exception("Subjectname not found with given language!");
                            }
                        }
                    }
                    var subjects = correctSubjectNames.Select(s => s.Name).ToList();

                    if(subjects == null)
                    {
                        throw new Exception("Subjects null");
                    }
                    
                    if(teacher.User == null)
                    {
                        throw new Exception("User is null");
                    }

                    var dto = new TeacherDTO
                    {
                        Id = teacher.Id,
                        Image = teacher.Image,
                        Name = teacher.User.LastName + " " + teacher.User.FirstName,
                        Subjects = subjects,
                        Description = correctDesc.Description
                    };
                    teacherDTO.Add(dto);
                }
                return teacherDTO;
            }
        }
    }
}
using AutoMapper;
using CassiniConnect.Core.Models.Teaching;

namespace CassiniConnect.Application.Mapping
{
    public class TeacherMappingProfile : Profile
    {
        public TeacherMappingProfile()
        {
            CreateMap<Teacher, Teacher>();
        }
    }
}
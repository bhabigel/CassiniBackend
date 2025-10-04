using AutoMapper;
using CassiniConnect.Core.Models.Teaching;

namespace CassiniConnect.Application.Mapping
{
    public class TeacherDescriptionMappingProfile : Profile
    {
        public TeacherDescriptionMappingProfile()
        {
            CreateMap<TeacherDescription, TeacherDescription>();
        }
    }
}
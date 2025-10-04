using AutoMapper;
using CassiniConnect.Core.Models.Teaching;

namespace CassiniConnect.Application.Mapping
{
    public class SubjectMappingProfile : Profile
    {
        public SubjectMappingProfile()
        {
            CreateMap<Subject, Subject>();
        }
    }
}
using AutoMapper;
using CassiniConnect.Core.Models.UserCore;

namespace CassiniConnect.Application.Mapping
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<Role, Role>();
        }
    }
}
using AutoMapper;
using CassiniConnect.Core.Models.UserCore;

namespace CassiniConnect.Application.Mapping
{
    public class UserMappingProfile:Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, User>();
        }
    }
}
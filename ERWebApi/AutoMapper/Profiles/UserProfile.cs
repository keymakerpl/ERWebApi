using AutoMapper;
using ERService.Business;
using ERWebApi.Models;

namespace ERWebApi.AutoMapper.Profiles
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

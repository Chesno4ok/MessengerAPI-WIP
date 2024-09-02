using AutoMapper;
using System;

namespace ChesnokMessengerAPI.Services
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<User, UserInfo>();
            CreateMap<User, UserCredentials>();
        }
    }
}

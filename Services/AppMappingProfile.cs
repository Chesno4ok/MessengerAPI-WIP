using AutoMapper;
using System;
using ChesnokMessengerAPI.Templates;

namespace ChesnokMessengerAPI.Services
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<MessageTemplate, Message>();
            CreateMap<ChatTemplate, Chat>();
            CreateMap<ChatUserTemplate, ChatUser>();
            CreateMap<UserTemplate, User>();
        }
    }
}

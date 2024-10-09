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
            CreateMap<EditMessageTemplate, Message>();
            CreateMap<ChatTemplate, Chat>();
            CreateMap<EditChatTemplate, Chat>();
            CreateMap<ChatUserTemplate, ChatUser>();
            CreateMap<UserTemplate, User>();
            CreateMap<EditUserTemplate, User>();
            CreateMap<User, UserResponse>();
            CreateMap<User, UserCredentials>();
            CreateMap<Chat, ChatResponse>();
            CreateMap<ChatUser, ChatUserResponse>();
            CreateMap<Message, MessageResponse>();

        }
    }
}

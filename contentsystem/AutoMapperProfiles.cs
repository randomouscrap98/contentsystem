using System;
using System.Collections.Generic;
using AutoMapper;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    public class SearchProfile : Profile
    {
        public SearchProfile()
        {
            //Always map forward/up. 

            //Copy constructors essentially
            CreateMap<EntitySearchBase, EntitySearch>()
                .ReverseMap();
            CreateMap<EntitySearchBase, EntityValueSearch>()
                .ReverseMap();
            CreateMap<EntitySearchBase, EntityRelationSearch>()
                .ReverseMap();

            CreateMap<EntitySearch, UserSearch>()
                .ForMember(d => d.UsernameLike, o => o.MapFrom(s => s.NameLike))
                .ReverseMap();
            CreateMap<EntitySearch, ContentSearch>() //NameLike shows up in both
                .ReverseMap();
            CreateMap<EntityRelationSearch, PermissionSearch>()
                .ReverseMap();
        }
    }

    public class ContentProfile : Profile
    {
        public ContentProfile() //Dictionary<Enum, string> keys)
        {
            //Copy constructors essentially
            CreateMap<BasicUser, User>()
                .ReverseMap();
            CreateMap<BasicContent, Content>()
                .ReverseMap();
            CreateMap<BasicCategory, Category>()
                .ReverseMap();
            CreateMap<BasicComment, Comment>()
                .ReverseMap();

            CreateMap<Entity, BasicUser>()
                .ForMember(d => d.username, o => o.MapFrom(s => s.name))
                .ReverseMap();
            CreateMap<Entity, BasicContent>()
                .ReverseMap();
        }
    }
}
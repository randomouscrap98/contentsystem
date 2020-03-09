using AutoMapper;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    public class SearchProfile : Profile
    {
        public SearchProfile()
        {
            CreateMap<UserSearch, EntitySearch>()
                .ForMember(d => d.NameLike, o => o.MapFrom(s => s.UsernameLike))
                .ReverseMap();
        }
    }
}
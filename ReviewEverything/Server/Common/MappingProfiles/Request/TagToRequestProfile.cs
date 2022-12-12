using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class TagToRequestProfile : Profile
    {
        public TagToRequestProfile()
        {
            CreateMap<TagRequest, Tag>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Reviews, opt =>
                    opt.MapFrom(src => src.Reviews));

            CreateMap<Tag, TagRequest>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Reviews, opt =>
                    opt.MapFrom(src => src.Reviews));
        }
    }
}

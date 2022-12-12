using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Response
{
    public class TagToResponseProfile : Profile
    {
        public TagToResponseProfile()
        {
            CreateMap<Tag, TagResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));

            CreateMap<TagResponse, Tag>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));
        }
    }
}
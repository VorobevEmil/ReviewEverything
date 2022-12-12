using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class CloudImageToRequestProfile : Profile
    {
        public CloudImageToRequestProfile()
        {
            CreateMap<CloudImageRequest, CloudImage>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Url, opt =>
                    opt.MapFrom(src => src.Url));

            CreateMap<CloudImage, CloudImageRequest>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Url, opt =>
                    opt.MapFrom(src => src.Url));
        }
    }
}

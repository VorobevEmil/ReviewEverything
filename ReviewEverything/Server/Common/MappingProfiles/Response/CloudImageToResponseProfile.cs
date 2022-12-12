using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Response
{
    public class CloudImageToResponseProfile : Profile
    {
        public CloudImageToResponseProfile()
        {
            CreateMap<CloudImage, CloudImageResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Url, opt =>
                    opt.MapFrom(src => src.Url));
        }
    }
}
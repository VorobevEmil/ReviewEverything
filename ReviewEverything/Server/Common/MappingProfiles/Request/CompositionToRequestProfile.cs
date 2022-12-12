using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class CompositionToRequestProfile : Profile
    {
        public CompositionToRequestProfile()
        {
            CreateMap<CompositionRequest, Composition>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CategoryId, opt =>
                    opt.MapFrom(src => src.CategoryId));

            CreateMap<Composition, CompositionRequest>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CategoryId, opt =>
                    opt.MapFrom(src => src.CategoryId));
        }
    }
}

using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.MappingProfiles
{
    public class DomainToRequestProfile : Profile
    {
        public DomainToRequestProfile()
        {
            CreateMap<CategoryRequest, Category>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));
        }
    }
}

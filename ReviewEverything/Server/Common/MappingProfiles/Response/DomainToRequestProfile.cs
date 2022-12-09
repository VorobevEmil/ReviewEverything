using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.MappingProfiles.Response
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

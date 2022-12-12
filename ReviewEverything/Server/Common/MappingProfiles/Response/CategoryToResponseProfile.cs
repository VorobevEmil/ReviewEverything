using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Response
{
    public class CategoryToResponseProfile : Profile
    {
        public CategoryToResponseProfile()
        {
            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));
        }
    }
}

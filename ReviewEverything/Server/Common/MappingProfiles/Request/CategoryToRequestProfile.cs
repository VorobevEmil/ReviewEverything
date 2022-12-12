using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class CategoryToRequestProfile : Profile
    {
        public CategoryToRequestProfile()
        {
            CreateMap<CategoryRequest, Category>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));
            CreateMap<Category, CategoryRequest>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));
        }
    }
}

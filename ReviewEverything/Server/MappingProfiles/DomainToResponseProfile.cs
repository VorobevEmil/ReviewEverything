using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt =>
                    opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Reviews, opt =>
                    opt.MapFrom(src => src.AuthorReviews));

            CreateMap<Review, ReviewResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CompositionId, opt =>
                    opt.MapFrom(src => src.CompositionId))
                .ForMember(dest => dest.Composition, opt =>
                    opt.MapFrom(src => src.Comments));

            CreateMap<Composition, CompositionResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));

            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));
        }
    }
}
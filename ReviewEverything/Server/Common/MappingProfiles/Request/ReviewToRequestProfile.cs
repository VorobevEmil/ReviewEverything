using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class ReviewToRequestProfile : Profile
    {
        public ReviewToRequestProfile()
        {
            CreateMap<ReviewRequest, Review>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Subtitle, opt =>
                    opt.MapFrom(src => src.Subtitle))
                .ForMember(dest => dest.AuthorScore, opt =>
                    opt.MapFrom(src => src.AuthorScore))
                .ForMember(dest => dest.Body, opt =>
                    opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.CompositionId, opt =>
                    opt.MapFrom(src => src.CompositionId))
                .ForMember(dest => dest.Composition, opt =>
                    opt.MapFrom(src => src.Composition))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.Tags))
                .ForMember(dest => dest.CloudImages, opt =>
                    opt.MapFrom(src => src.CloudImages));

            CreateMap<Review, ReviewRequest>()
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Subtitle, opt =>
                    opt.MapFrom(src => src.Subtitle))
                .ForMember(dest => dest.AuthorScore, opt =>
                    opt.MapFrom(src => src.AuthorScore))
                .ForMember(dest => dest.Body, opt =>
                    opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.CompositionId, opt =>
                    opt.MapFrom(src => src.CompositionId))
                .ForMember(dest => dest.Composition, opt =>
                    opt.MapFrom(src => src.Composition))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.Tags))
                .ForMember(dest => dest.CloudImages, opt =>
                    opt.MapFrom(src => src.CloudImages));
        }
    }
}
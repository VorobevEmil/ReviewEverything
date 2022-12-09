using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class ReviewToResponseProfile : Profile
    {
        public ReviewToResponseProfile()
        {
            CreateMap<Review, ReviewResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CloudImage, opt =>
                    opt.MapFrom(src => src.CloudImages[0]))
                .ForMember(dest => dest.CompositionId, opt =>
                    opt.MapFrom(src => src.CompositionId))
                .ForMember(dest => dest.Composition, opt =>
                    opt.MapFrom(src => src.Composition.Title))
                .ForMember(dest => dest.Author, opt =>
                    opt.MapFrom(src => src.Author.UserName))
                .ForMember(dest => dest.AuthorId, opt =>
                    opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AuthorScore, opt =>
                    opt.MapFrom(src => src.AuthorScore))
                .ForMember(dest => dest.CommentCount, opt =>
                    opt.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.LikeUsers, opt =>
                    opt.MapFrom(src => src.LikeUsers.Count))
                .ForMember(dest => dest.CreationDate, opt =>
                    opt.MapFrom(src => src.CreationDate));
        }
    }
}

using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Response
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
                .ForMember(dest => dest.Subtitle, opt =>
                    opt.MapFrom(src => src.Subtitle))
                .ForMember(dest => dest.CloudImage, opt =>
                    opt.MapFrom(src => src.CloudImages[0]))
                .ForMember(dest => dest.UserScores, opt =>
                    opt.MapFrom(src => src.Composition.UserScores))
                .ForMember(dest => dest.CompositionId, opt =>
                    opt.MapFrom(src => src.CompositionId))
                .ForMember(dest => dest.Composition, opt =>
                    opt.MapFrom(src => src.Composition.Title))
                .ForMember(dest => dest.CategoryId, opt =>
                    opt.MapFrom(src => src.Composition.CategoryId))
                .ForMember(dest => dest.Category, opt =>
                    opt.MapFrom(src => src.Composition.Category.Title))
                .ForMember(dest => dest.Author, opt =>
                    opt.MapFrom(src => src.Author.FullName))
                .ForMember(dest => dest.AuthorId, opt =>
                    opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AuthorScore, opt =>
                    opt.MapFrom(src => src.AuthorScore))
                .ForMember(dest => dest.CommentCount, opt =>
                    opt.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.LikeUsers, opt =>
                    opt.MapFrom(src => src.LikeUsers.Select(x => x.Id).ToList()))
                .ForMember(dest => dest.CreationDate, opt =>
                    opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.UpdateDate, opt =>
                    opt.MapFrom(src => src.UpdateDate));


            CreateMap<Review, ArticleReviewResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Subtitle, opt =>
                    opt.MapFrom(src => src.Subtitle))
                .ForMember(dest => dest.Body, opt =>
                    opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.CloudImages, opt =>
                    opt.MapFrom(src => src.CloudImages))
                .ForMember(dest => dest.CompositionId, opt =>
                    opt.MapFrom(src => src.CompositionId))
                .ForMember(dest => dest.Composition, opt =>
                    opt.MapFrom(src => src.Composition.Title))
                .ForMember(dest => dest.CategoryId, opt =>
                    opt.MapFrom(src => src.Composition.CategoryId))
                .ForMember(dest => dest.Category, opt =>
                    opt.MapFrom(src => src.Composition.Category.Title))
                .ForMember(dest => dest.Author, opt =>
                    opt.MapFrom(src => src.Author.FullName))
                .ForMember(dest => dest.AuthorId, opt =>
                    opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.AuthorScore, opt =>
                    opt.MapFrom(src => src.AuthorScore))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.Tags))
                .ForMember(dest => dest.UserScores, opt =>
                    opt.MapFrom(src => src.Composition.UserScores))
                .ForMember(dest => dest.LikeUsers, opt =>
                    opt.MapFrom(src => src.LikeUsers.Select(x => x.Id).ToList()))
                .ForMember(dest => dest.CreationDate, opt =>
                    opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.UpdateDate, opt =>
                    opt.MapFrom(src => src.UpdateDate));

            CreateMap<Review, ReviewSearchResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt =>
                    opt.MapFrom(src => src.Title));
        }
    }
}

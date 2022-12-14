using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class CommentToRequestProfile : Profile
    {
        public CommentToRequestProfile()
        {
            CreateMap<CommentRequest, Comment>()
                .ForMember(dest => dest.Body, opt =>
                    opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.ReviewId, opt =>
                    opt.MapFrom(src => src.ReviewId));
        }
    }
}
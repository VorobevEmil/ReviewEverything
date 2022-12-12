using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Response
{
    public class CommentToResponseProfile : Profile
    {
        public CommentToResponseProfile()
        {
            CreateMap<Comment, CommentResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Body, opt =>
                    opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.UserId, opt =>
                    opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt =>
                    opt.MapFrom(src => src.User.UserName));
        }
    }
}

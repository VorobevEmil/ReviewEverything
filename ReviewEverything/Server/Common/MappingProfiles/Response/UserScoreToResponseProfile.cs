using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Response
{
    public class UserScoreToResponseProfile : Profile
    {
        public UserScoreToResponseProfile()
        {
            CreateMap<UserScore, UserScoreResponse>()
                .ForMember(dest => dest.Score, opt =>
                    opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.UserId, opt =>
                    opt.MapFrom(src => src.UserId));
        }
    }
}

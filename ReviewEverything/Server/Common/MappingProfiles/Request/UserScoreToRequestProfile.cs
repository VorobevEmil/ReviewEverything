using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class UserScoreToRequestProfile : Profile
    {
        public UserScoreToRequestProfile()
        {
            CreateMap<UserScoreRequest, UserScore>()
                .ForMember(dest => dest.CompositionId, opt =>
                    opt.MapFrom(src => src.CompositionId))
                .ForMember(dest => dest.Score, opt =>
                    opt.MapFrom(src => src.Score));
        }
    }
}

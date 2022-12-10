using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Request
{
    public class ApplicationUserToResponseProfile : Profile
    {
        public ApplicationUserToResponseProfile()
        {
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt =>
                    opt.MapFrom(src => src.UserName));
        }
    }
}

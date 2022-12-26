using AutoMapper;
using ReviewEverything.Server.Models;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Server.Common.MappingProfiles.Response
{
    public class ApplicationUserToResponseProfile : Profile
    {
        public ApplicationUserToResponseProfile()
        {
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt =>
                    opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt =>
                    opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Likes, opt =>
                    opt.MapFrom(src => src.LikeReviews.Count));

            CreateMap<ApplicationUser, UserManagementResponse>()
                .ForMember(dest => dest.Id, opt =>
                    opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt =>
                    opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt =>
                    opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Status, opt =>
                    opt.MapFrom(src => src.Block ? "Заблокирован" : "Разблокирован"));
        }
    }
}

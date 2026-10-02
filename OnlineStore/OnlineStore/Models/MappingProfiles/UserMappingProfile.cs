using AutoMapper;
using OnlineStore.Models.DTOs;
using OnlineStore.Models.FrontendModels;
using OnlineStore.DBModels;

namespace OnlineStore.Models.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserModel, UserDTO>().ReverseMap();

            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Value))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<User, UserDTO>();
        }
    }
}

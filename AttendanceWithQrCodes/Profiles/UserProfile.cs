using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<UserChangePasswordDto, User>()
                .ReverseMap();
            CreateMap<UserCreateDto, User>()
                .ReverseMap();
            CreateMap<UserUpdateDto, User>()
                .ForMember(
                    dest => dest.RoleId, 
                    src => src.Ignore())
                .ReverseMap();
            CreateMap<UserDto, User>()
                .ForPath(   
                    dest => dest.Role.Name,
                    src => src.MapFrom(s => s.Role.RoleName))
                .ReverseMap();
            CreateMap<UserListDto, User>()
                .ForPath(   
                    dest => dest.Role.Name,
                    src => src.MapFrom(s => s.Role.RoleName))
                .ReverseMap();
            CreateMap<UserNameEmailDto, User>()
                .ReverseMap();
        }
    }
}

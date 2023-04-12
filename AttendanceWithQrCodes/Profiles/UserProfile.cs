using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<UserCreateUpdateDto, User>();
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
        }
    }
}

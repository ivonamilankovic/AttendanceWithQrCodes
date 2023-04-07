using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile() 
        {
            CreateMap<Role, RoleDto>()
                .ForMember(
                    dest => dest.RoleName,
                    src => src.MapFrom(s => s.Name))
                .ReverseMap();
        }
    }
}

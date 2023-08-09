using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class StudentInfoProfile : Profile
    {
        public StudentInfoProfile() 
        {
            CreateMap<StudentInformation, StudentInfoCreateDto>()
                .ReverseMap();
            CreateMap<StudentInformation, StudentInfoUpdateDto>()
                .ReverseMap();
            CreateMap<StudentInfoDetailsDto, StudentInformation>()
                .ForPath(
                    dest => dest.User,
                    src => src.MapFrom(s => s.User))
                .ReverseMap();
            CreateMap<StudentInformation, StudentInfoAttendanceDetailsDto>()
                .ReverseMap();
        }
    }
}

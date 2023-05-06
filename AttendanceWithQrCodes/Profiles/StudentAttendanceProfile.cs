using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class StudentAttendanceProfile : Profile
    {
        public StudentAttendanceProfile() 
        {
            CreateMap<StudentAttendance, StudentAttendanceDto>()
                .ForMember(
                    dest => dest.Index,
                    src => src.MapFrom(s => s.StudentIndex))
                .ForMember(
                    dest => dest.MacAddress,
                    src => src.Ignore())
                .ReverseMap();
        }
    }
}

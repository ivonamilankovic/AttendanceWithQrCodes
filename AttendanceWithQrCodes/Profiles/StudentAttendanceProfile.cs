using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class StudentAttendanceProfile : Profile
    {
        public StudentAttendanceProfile() 
        {
            CreateMap<StudentAttendance, StudentAttendanceCreateDto>()
                .ForMember(
                    dest => dest.Index,
                    src => src.MapFrom(s => s.StudentIndex))
                .ForMember(
                    dest => dest.Latitude,
                    src => src.Ignore())
                .ForMember(
                    dest => dest.Longitude,
                    src => src.Ignore())
                .ReverseMap();
            CreateMap<StudentAttendance, StudentAttendanceDetailsDto>()
                .ReverseMap();
            CreateMap<StudentAttendance, StudentAttendanceListDto>()
                .ReverseMap();
            CreateMap<StudentAttendance, StudentAttendanceForExcelDto>()
                .ForMember(
                    dest => dest.Index,
                    src => src.MapFrom(s => s.StudentIndex))
                .ForPath(
                    dest => dest.Student,
                    src => src.MapFrom(s => $"{s.Student.User.FirstName} {s.Student.User.LastName}"))
                .ForMember(
                    dest => dest.DateOfAttendance,
                    src => src.MapFrom(s => s.Date))
                .ForPath(
                    dest => dest.CourseName,
                    src => src.MapFrom(s => s.Lecture.Course.Name))
                .ForPath(
                    dest => dest.LectureName,
                    src => src.MapFrom(s => s.Lecture.Name))
                .ForPath(
                    dest => dest.Lecturer,
                    src => src.MapFrom(s => $"{s.Lecture.Lecturer.FirstName} {s.Lecture.Lecturer.LastName}"))
                .ReverseMap();
        }
    }
}

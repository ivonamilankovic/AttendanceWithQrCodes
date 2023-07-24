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
                    dest => dest.StudyProfile.Name,
                    src => src.MapFrom(s => s.StudyProfile.ProfileName))
                .ForPath(
                    dest => dest.StudyLanguage.Name,
                    src => src.MapFrom(s => s.StudyLanguage.LanguageName))
                .ForPath(
                    dest => dest.User,
                    src => src.MapFrom(s => s.User))
                .ReverseMap();
            CreateMap<StudentInformation, StudentInfoAttendanceDetailsDto>()
                 .ForPath(
                    dest => dest.StudyProfile.ProfileName,
                    src => src.MapFrom(s => s.StudyProfile.Name))
                .ForPath(
                    dest => dest.StudyLanguage.LanguageName,
                    src => src.MapFrom(s => s.StudyLanguage.Name))
                .ReverseMap();
        }
    }
}

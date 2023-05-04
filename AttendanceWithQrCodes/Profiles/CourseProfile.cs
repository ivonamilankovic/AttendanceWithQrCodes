using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile() 
        {
            CreateMap<CourseCreateUpdateDto, Course>()
                .ForPath(
                    dest => dest.CourseLanguages, 
                    src => src.Ignore())
                .ForPath(
                    dest => dest.CourseStudyProfiles, 
                    src => src.Ignore())
                .ReverseMap();
            CreateMap<Course, CourseDetailsDto>()
                .ForPath(
                    dest => dest.Languages, 
                    src => src.MapFrom(s => s.CourseLanguages))
                .ForPath(
                    dest => dest.StudyProfiles, 
                    src => src.MapFrom(s => s.CourseStudyProfiles))
                .ReverseMap();
            CreateMap<Course, CourseListDto>()
                .ReverseMap();
            CreateMap<Course, CourseNameDto>()
                .ReverseMap();
            CreateMap<CourseLanguage, CourseLanguageDto>()
                .ForMember(
                    dest => dest.Language, 
                    src => src.MapFrom(s => s.StudyLanguage.Name))
                .ReverseMap();
            CreateMap<CourseStudyProfile, CourseStudyProfileDto>()
                .ForMember(
                    dest => dest.ProfileName, 
                    src => src.MapFrom(s => s.StudyProfile.Name))
                .ReverseMap();
        }
    }
}

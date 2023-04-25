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
                .ForPath(dest => dest.CourseLanguages, src => src.Ignore())
                .ForPath(dest => dest.CourseStudyProfiles, src => src.Ignore())
                .ReverseMap();
        }
    }
}

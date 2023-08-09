using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class StudyProfileProfile : Profile
    {
        public StudyProfileProfile() 
        {
            CreateMap<StudyProfile, StudyProfileDto>()
                .ReverseMap();
        }
    }
}

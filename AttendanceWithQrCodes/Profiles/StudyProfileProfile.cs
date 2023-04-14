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
                .ForMember( 
                    dest => dest.ProfileName,
                    src => src.MapFrom(s => s.Name))
                .ReverseMap();
        }
    }
}

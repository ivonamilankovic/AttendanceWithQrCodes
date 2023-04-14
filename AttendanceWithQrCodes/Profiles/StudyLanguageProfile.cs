using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class StudyLanguageProfile : Profile
    {
        public StudyLanguageProfile() 
        {
            CreateMap<StudyLanguage, StudyLanguageDto>()
                .ForMember(
                    dest => dest.LanguageName,
                    src => src.MapFrom(s => s.Name))
                .ReverseMap();
        }
    }
}

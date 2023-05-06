using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class LectureProfile : Profile
    {
        public LectureProfile() 
        {
            CreateMap<LectureCreateDto, Lecture>()
                .ReverseMap();
            CreateMap<LectureUpdateDto, Lecture>()
                .ReverseMap();
            CreateMap<Lecture, LectureDetailsDto>()
                .ReverseMap();
            CreateMap<Lecture, LectureAttendanceDetailsDto>()
                .ReverseMap();
        }
    }
}

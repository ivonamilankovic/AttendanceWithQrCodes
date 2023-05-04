using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;

namespace AttendanceWithQrCodes.Profiles
{
    public class QrCodeProfile : Profile
    {
        public QrCodeProfile() 
        {
            CreateMap<Models.QrCode, QrCodeDto>()
                .ReverseMap();
        }
    }
}

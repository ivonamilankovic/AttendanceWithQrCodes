﻿using AttendanceWithQrCodes.Models;
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
                    dest => dest.MacAddress,
                    src => src.Ignore())
                .ReverseMap();
            CreateMap<StudentAttendance, StudentAttendanceDetailsDto>()
                .ReverseMap();
            CreateMap<StudentAttendance, StudentAttendanceListDto>()
                .ReverseMap();
        }
    }
}
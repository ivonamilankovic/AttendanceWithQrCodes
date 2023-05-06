using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace AttendanceWithQrCodes.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class StudentAttendanceController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public StudentAttendanceController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates student registration to lecture.
        /// </summary>
        /// <param name="attendanceDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(StudentAttendanceDto attendanceDto)
        {
            if (attendanceDto == null)
            {
                return BadRequest();
            }
            if (attendanceDto.Index == 0 || attendanceDto.LectureId == 0)
            {
                return BadRequest();
            }

            StudentInformation? student = await _context.StudentInformations
                                        .Include(s => s.User)
                                        .SingleOrDefaultAsync(s => s.Index == attendanceDto.Index);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            if (student.MacAddress != attendanceDto.MacAddress)
            {
                return BadRequest("Access from different device.");
            }

            Lecture? lecture = await _context.Lectures
                                .Include(l => l.QrCode)
                                .SingleOrDefaultAsync(l => l.Id == attendanceDto.LectureId);
            if (lecture == null)
            {
                return NotFound("Lecture not found.");
            }

            bool attendanceSubmited = await _context.StudentAttendances
                                    .AnyAsync(a => a.StudentIndex == attendanceDto.Index    
                                    && a.Lecture.Id == attendanceDto.LectureId);
            if (attendanceSubmited)
            {
                return BadRequest("You have already submited to this lecture."); 
            }

            DateTime now = DateTime.Now;
            TimeSpan timeDifference = now - lecture.QrCode.ExpiresAt;
            if (timeDifference.TotalMinutes >= 5)
            {
                return BadRequest("Qr code has expired. You can't be registered to this lecture.");
            }

            StudentAttendance attendance = _mapper.Map<StudentAttendanceDto, StudentAttendance>(attendanceDto);
            attendance.Present = true;
            attendance.Date = now;

            _context.StudentAttendances.Add(attendance);
            await _context.SaveChangesAsync();
            return Ok(attendance);

        }
    }
}

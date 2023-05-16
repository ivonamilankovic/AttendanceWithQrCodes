using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Linq;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        /// Returns list of attendances.
        /// </summary>
        /// <param name="lectureId"></param>
        /// <param name="studentIndex"></param>
        /// <param name="courseId"></param>
        /// <param name="lecturerId"></param>
        /// <param name="profileId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll(int lectureId, int studentIndex, int courseId, int lecturerId, int profileId, int languageId)
        {
            IList<StudentAttendance> attendances = await _context.StudentAttendances
                                    .Include(a => a.Student)
                                    .Include(a => a.Student.User)
                                    .Include(a => a.Student.StudyLanguage)
                                    .Include(a => a.Student.StudyProfile)
                                    .Include(a => a.Lecture)
                                    .Include(a => a.Lecture.Course)
                                    .Include(a => a.Lecture.Lecturer)
                                    .Include(a => a.Lecture.QrCode)
                                    .WhereIf(studentIndex != 0, a => a.StudentIndex == studentIndex)
                                    .WhereIf(profileId != 0, a => a.Student.StudyProfileId == profileId)
                                    .WhereIf(languageId != 0, a => a.Student.StudyLanguageId == languageId)
                                    .WhereIf(lectureId != 0, a => a.LectureId == lectureId)
                                    .WhereIf(lecturerId != 0, a => a.Lecture.LecturerId == lecturerId)
                                    .WhereIf(courseId != 0, a => a.Lecture.CourseId == courseId)
                                    .ToListAsync();
            if (!attendances.Any())
            {
                return NoContent();
            }

            IList<StudentAttendanceListDto> attendanceDetailsDtos = _mapper.Map<IList<StudentAttendance>, IList<StudentAttendanceListDto>>(attendances);
            return Ok(attendanceDetailsDtos);
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
        public async Task<IActionResult> Create(StudentAttendanceCreateDto attendanceDto)
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
                                        .Include(s => s.StudyLanguage)
                                        .Include(s => s.StudyProfile)
                                        .SingleOrDefaultAsync(s => s.Index == attendanceDto.Index);
            if (student == null)
            {
                return NotFound("Student not found.");
            }
            
            Lecture? lecture = await _context.Lectures
                                .Include(l => l.Course)
                                .Include(l => l.Lecturer)
                                .Include(l => l.QrCode)
                                .SingleOrDefaultAsync(l => l.Id == attendanceDto.LectureId);
            if (lecture == null)
            {
                return NotFound("Lecture not found.");
            }

            bool attendanceSubmited = await _context.StudentAttendances
                                    .AnyAsync(a => a.StudentIndex == attendanceDto.Index    
                                    && a.LectureId == attendanceDto.LectureId);
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

            StudentAttendance attendance = _mapper.Map<StudentAttendanceCreateDto, StudentAttendance>(attendanceDto);
            attendance.Present = true;
            attendance.Date = now;

            _context.StudentAttendances.Add(attendance);
            await _context.SaveChangesAsync();

            StudentAttendanceDetailsDto attendanceDetailsDto = _mapper.Map<StudentAttendance, StudentAttendanceDetailsDto>(attendance);

            return Ok(attendanceDetailsDto);

        }

        /// <summary>
        /// Updates students presence by their index.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="studentIndex"></param>
        /// <returns></returns>
        [HttpPut("{id}/{studentIndex}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, int studentIndex) 
        {
            bool studentValid = await _context.StudentInformations.AnyAsync(s => s.Index == studentIndex);
            if (!studentValid)
            {
                return NotFound("Student does not exist.");
            }

            StudentAttendance? attendance = await _context.StudentAttendances.SingleOrDefaultAsync(a => a.Id == id);
            if(attendance == null)
            {
                return NotFound("This student did not attend this lecture.");
            }
            attendance.Present = !attendance.Present;
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Deletes student attendance by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            StudentAttendance? attendance = await _context.StudentAttendances.SingleOrDefaultAsync(a => a.Id == id);
            if (attendance == null)
            {
                return NotFound("This student did not attend this lecture.");
            }
            _context.StudentAttendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

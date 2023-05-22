using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Linq;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using static AttendanceWithQrCodes.Data.RoleConstants;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using AttendanceWithQrCodes.HelperMethods;
using ClosedXML.Excel;

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
        private readonly ILocationCheck _locationCheck;
        public StudentAttendanceController(Context context, IMapper mapper, ILocationCheck locationCheck)
        {
            _context = context;
            _mapper = mapper;
            _locationCheck = locationCheck;
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
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll(int lectureId, int studentIndex, int courseId, int lecturerId, int profileId, int languageId)
        {
            IList<StudentAttendance> attendances = await FetchAllAttendances(lectureId, courseId, lecturerId, profileId, languageId, studentIndex);
            
            if (!attendances.Any())
            {
                return NoContent();
            }

            IList<StudentAttendanceListDto> attendanceDetailsDtos = _mapper.Map<IList<StudentAttendance>, IList<StudentAttendanceListDto>>(attendances);
            return Ok(attendanceDetailsDtos);
        }

        /// <summary>
        /// Generates .xlsx file with student attendances for download.
        /// </summary>
        /// <param name="lectureId"></param>
        /// <param name="courseId"></param>
        /// <param name="lecturerId"></param>
        /// <param name="profileId"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        [HttpGet("excel")]
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<FileContentResult> GetAllAsExcelFile(int lectureId, int courseId, int lecturerId, int profileId, int languageId)
        {
            IList<StudentAttendance> attendances = await FetchAllAttendances(lectureId, courseId, lecturerId, profileId, languageId);
            
            IList<StudentAttendanceForExcelDto> attendanceDtos = _mapper.Map<IList<StudentAttendance>, IList<StudentAttendanceForExcelDto>>(attendances);
            
            XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet sheet = workbook.Worksheets.Add("StudentAttendances");
            int row = 1;
            sheet.Cell(row, 1).Value = "No.";
            sheet.Cell(row, 2).Value = "Index";
            sheet.Cell(row, 3).Value = "Student";
            sheet.Cell(row, 4).Value = "Present";
            sheet.Cell(row, 5).Value = "DateOfAttendance";
            sheet.Cell(row, 6).Value = "CourseName";
            sheet.Cell(row, 7).Value = "LectureName";
            sheet.Cell(row, 8).Value = "Lecturer";

            int no = 1;
            foreach (StudentAttendanceForExcelDto a in attendanceDtos)
            {
                row++;
                sheet.Cell(row, 1).Value = no;
                sheet.Cell(row, 2).Value = a.Index;
                sheet.Cell(row, 3).Value = a.Student;
                sheet.Cell(row, 4).Value = a.Present;
                sheet.Cell(row, 5).Value = a.DateOfAttendance;
                sheet.Cell(row, 6).Value = a.CourseName;
                sheet.Cell(row, 7).Value = a.LectureName;
                sheet.Cell(row, 8).Value = a.Lecturer;
                no++;
            }
            
            sheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            sheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            sheet.Row(1).Style.Fill.BackgroundColor = XLColor.FromArgb(0xCCC7C6);
            sheet.Rows(2, no).Style.Fill.BackgroundColor = XLColor.FromArgb(0xF0EAE9);
            sheet.Row(1).Style.Font.Bold = true;
            sheet.Row(1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Rows(2,row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            sheet.Columns().Width = 25;
            sheet.Column(1).Width = 6;
            sheet.Column(2).Width = 15;
            sheet.Column(4).Width = 15;
            sheet.Column(1).Style.Font.Bold = true;

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                byte[] content = ms.ToArray();
                string fileName = "StudentAttendances-" + Guid.NewGuid() + ".xlsx";

                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        /// <summary>
        /// Creates student registration to lecture.
        /// </summary>
        /// <param name="attendanceDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole + "," + StudentRole)]
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

            if (attendanceDto.Latitude > 0 && attendanceDto.Longitude > 0)
            {
                bool locationGood = _locationCheck.IsLocationAcceptable(attendanceDto.Latitude, attendanceDto.Longitude);
                if (!locationGood)
                {
                    return BadRequest("Your location is not acceptable.");
                }
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
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
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
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
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

        /// <summary>
        /// Help method to get list of attendances.
        /// </summary>
        /// <param name="lectureId"></param>
        /// <param name="studentIndex"></param>
        /// <param name="courseId"></param>
        /// <param name="lecturerId"></param>
        /// <param name="profileId"></param>
        /// <param name="languageId"></param>
        /// <returns>List of StudentAttendance objects.</returns>
        public async Task<IList<StudentAttendance>> FetchAllAttendances(int lectureId, int courseId, int lecturerId, int profileId, int languageId, int studentIndex = 0)
        {
            IList<StudentAttendance> attendances = await _context.StudentAttendances
                                    .Include(a => a.Student)
                                    .Include(a => a.Student.User)
                                    .Include(a => a.Student.StudyLanguage)
                                    .Include(a => a.Student.StudyProfile)
                                    .Include(a => a.Lecture)
                                    .Include(a => a.Lecture.Course)
                                    .Include(a => a.Lecture.Lecturer)
                                    .WhereIf(studentIndex != 0, a => a.StudentIndex == studentIndex)
                                    .WhereIf(profileId != 0, a => a.Student.StudyProfileId == profileId)
                                    .WhereIf(languageId != 0, a => a.Student.StudyLanguageId == languageId)
                                    .WhereIf(lectureId != 0, a => a.LectureId == lectureId)
                                    .WhereIf(lecturerId != 0, a => a.Lecture.LecturerId == lecturerId)
                                    .WhereIf(courseId != 0, a => a.Lecture.CourseId == courseId)
                                    .ToListAsync();
            return attendances;
        }

        }
}

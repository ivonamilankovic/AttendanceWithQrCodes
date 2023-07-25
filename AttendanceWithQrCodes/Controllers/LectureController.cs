using AttendanceWithQrCodes.Data;
using static AttendanceWithQrCodes.Data.RoleConstants;
using AttendanceWithQrCodes.Linq;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AttendanceWithQrCodes.QrCode;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mime;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Spreadsheet;

namespace AttendanceWithQrCodes.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class LectureController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ICreateQrCode _createQrCode;
        public LectureController(Context context, IMapper mapper, ICreateQrCode createQrCode)
        {
            _context = context;
            _mapper = mapper;
            _createQrCode = createQrCode;
        }

        /// <summary>
        /// Returns list of lectures.
        /// </summary>
        /// <param name="lecturerId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll(int lecturerId, int courseId)
        {
            IList<Lecture> lectures = await _context.Lectures
                                        .Include(l => l.QrCode)
                                        .Include(l => l.Lecturer)
                                        .Include(l => l.Course)
                                        .WhereIf(lecturerId != 0, l => l.LecturerId == lecturerId)
                                        .WhereIf(courseId != 0, l => l.CourseId == courseId)
                                        .ToListAsync();
            if (!lectures.Any())
            {
                return NoContent();
            }

            IList<LectureDetailsDto> lectureDetailsDtos = _mapper.Map<IList<Lecture>, IList<LectureDetailsDto>>(lectures);
            return Ok(lectureDetailsDtos);
        }
       
        /// <summary>
        /// Returns lecture by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            Lecture? lecture = await _context.Lectures
                                       .Include(l => l.QrCode)
                                       .Include(l => l.Lecturer)
                                       .Include(l => l.Course)
                                       .SingleOrDefaultAsync(l => l.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }

           LectureDetailsDto lectureDetailsDto = _mapper.Map<Lecture, LectureDetailsDto>(lecture);
            return Ok(lectureDetailsDto);
        }

        /// <summary>
        /// Returns picture od qr code for lecture.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("QrCode/{id}")]
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<FileContentResult> GetQrCodeById(int id)
        {
            Lecture? lecture = await _context.Lectures.Include(l => l.QrCode).FirstOrDefaultAsync(l => l.Id == id);

            string qrCodeImgName = lecture.QrCode.ImageName;
            string domian = AppDomain.CurrentDomain.BaseDirectory;
            string[] domianParts = domian.Split("bin");
            string path = String.Concat(domianParts[0], "QrCode\\codes\\" + qrCodeImgName);

            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(path));
            byte[] content = stream.ToArray();

            return File( content, "image/png", qrCodeImgName);
        }

        /// <summary>
        /// Creates new lecture.
        /// </summary>
        /// <param name="lectureDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(LectureCreateDto lectureDto)
        {
            if(lectureDto == null)
            {
                return BadRequest();
            }
            if(lectureDto.Name.IsNullOrEmpty() || lectureDto.Description.IsNullOrEmpty())
            {
                return BadRequest("Please provide information about lecture.");
            }

            User? lecturer = await _context.Users.Include(u => u.Role)
                                .SingleOrDefaultAsync(u => u.Id == lectureDto.LecturerId);
            if (lecturer == null)
            {
                return NotFound("Lecturer does not exist.");
            }
            
            if(lecturer.Role.Name == ProfessorRole)
            {
                bool hasThisCourse = await _context.Courses.AnyAsync(c => c.ProfessorId == lectureDto.LecturerId && c.Id == lectureDto.CourseId);
                if (!hasThisCourse)
                {
                    return BadRequest("This lecturer does not teach this course.");
                }
            }
            else if(lecturer.Role.Name == AssistantRole)
            {
                bool hasThisCourse = await _context.Courses.AnyAsync(c => c.AssistantId == lectureDto.LecturerId && c.Id == lectureDto.CourseId);
                if (!hasThisCourse)
                {
                    return BadRequest("This lecturer does not teach this course.");
                }
            }
            else
            {
                return BadRequest("User you provided does not have role of lecturer.");
            }

            bool courseExists = await _context.Courses.AnyAsync(c => c.Id == lectureDto.CourseId);
            if (!courseExists)
            {
                return NotFound("Course you provided does not exist.");
            }

            Lecture lecture = _mapper.Map<LectureCreateDto, Lecture>(lectureDto);
            lecture.Course = await _context.Courses.SingleOrDefaultAsync(c => c.Id == lectureDto.CourseId);
            
            if(lecture.Course.TotalTakenLectures == (lecture.Course.LecturesNumForProfessor + lecture.Course.LecturesNumForAssistent))
            {
                return BadRequest("You can't have more lectures than specified in course info.");
            }
            else
            {
                lecture.Course.TotalTakenLectures++;
            }
            
            lecture.Date = DateTime.Now;
            _context.Lectures.Add(lecture);
            await _context.SaveChangesAsync();

            Models.QrCode qrCode = _createQrCode.GenerateQrCode(lecture.Date, lecture.Id);
            _context.QrCodes.Add(qrCode);
            await _context.SaveChangesAsync();

            lecture.QrCode = qrCode;
            await _context.SaveChangesAsync();

            LectureDetailsDto lectureDetailsDto = _mapper.Map<Lecture, LectureDetailsDto>(lecture);

            return Ok(lectureDetailsDto);
        }

        /// <summary>
        /// Updates lecture by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lectureDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, LectureUpdateDto lectureDto )
        {
            if (lectureDto == null)
            {
                return BadRequest();
            }
            if (lectureDto.Name.IsNullOrEmpty() || lectureDto.Description.IsNullOrEmpty())
            {
                return BadRequest("Please provide information about lecture.");
            }

            Lecture? lecture = await _context.Lectures
                                      .Include(l => l.QrCode)
                                      .Include(l => l.Lecturer)
                                      .Include(l => l.Course)
                                      .SingleOrDefaultAsync(l => l.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }

            _mapper.Map<LectureUpdateDto, Lecture>(lectureDto, lecture);
            await _context.SaveChangesAsync();

            LectureDetailsDto lectureDetailsDto = _mapper.Map<Lecture, LectureDetailsDto>(lecture);
            return Ok(lectureDetailsDto);
        }

        /// <summary>
        /// Creates new qr code for lecture by lecture id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("QrCode/{id}")]
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateNewQrCode(int id)
        {
            Lecture? lecture = await _context.Lectures
                                      .Include(l => l.QrCode)
                                      .Include(l => l.Lecturer)
                                      .Include(l => l.Course)
                                      .SingleOrDefaultAsync(l => l.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }

            if(lecture.Date.Date != DateTime.Now.Date)
            {
                return BadRequest("You can't make new qr code because this class was not today.");
            }
            
            Models.QrCode? qrToDelete = lecture.QrCode;
            lecture.QrCode = null;
            lecture.Date = DateTime.Now;
            if (qrToDelete != null)
            {
                _context.QrCodes.Remove(qrToDelete);
            }
            await _context.SaveChangesAsync();

            Models.QrCode qrCode = _createQrCode.GenerateQrCode(lecture.Date, lecture.Id);
            _context.QrCodes.Add(qrCode);
            await _context.SaveChangesAsync();

            lecture.QrCode = qrCode;
            await _context.SaveChangesAsync();

            LectureDetailsDto lectureDetailsDto = _mapper.Map<Lecture, LectureDetailsDto>(lecture);

            return Ok(lectureDetailsDto);
        }

        /// <summary>
        /// Deletes lecture by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            Lecture? lecture = await _context.Lectures
                                     .Include(l => l.QrCode)
                                     .Include(l => l.Lecturer)
                                     .Include(l => l.Course)
                                     .SingleOrDefaultAsync(l => l.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }
            Models.QrCode? qr = lecture.QrCode;
            int cId = lecture.Course.Id;
            lecture.QrCode = null;
            lecture.Lecturer = null;
            lecture.Course = null;
            if (qr != null)
            {
                _context.QrCodes.Remove(qr);
            }
            IList<StudentAttendance> attendances = await _context.StudentAttendances
                                                .Include(a => a.Lecture)
                                                .Where(a => a.LectureId == id)
                                                .ToListAsync();
            foreach(StudentAttendance a in attendances)
            {
                _context.StudentAttendances.Remove(a);
            }
            await _context.SaveChangesAsync();

            Course? course = await _context.Courses.SingleOrDefaultAsync(c => c.Id == cId);
            if (course == null)
            {
                return NotFound();
            }

            course.TotalTakenLectures--;
            await _context.SaveChangesAsync();

            _context.Lectures.Remove(lecture);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

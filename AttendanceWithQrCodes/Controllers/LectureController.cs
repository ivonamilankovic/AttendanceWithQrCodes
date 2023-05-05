﻿using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Linq;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AttendanceWithQrCodes.QrCode;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Net.Mime;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll(int lecturerId, int courseId)
        {
            IList<Lecture> lectures = await _context.Lectures
                                        .Include(l => l.QrCode)
                                        .Include(l => l.Lecturer)
                                        .Include(l => l.Course)
                                        .WhereIf(lecturerId != 0, l => l.Lecturer.Id == lecturerId)
                                        .WhereIf(courseId != 0, l => l.Course.Id == courseId)
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
        /// Creates new lecture.
        /// </summary>
        /// <param name="lectureDto"></param>
        /// <returns></returns>
        [HttpPost]
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
            
            if(lecturer.Role.Name == "Professor")
            {
                bool hasThisCourse = await _context.Courses.AnyAsync(c => c.ProfessorId == lectureDto.LecturerId && c.Id == lectureDto.CourseId);
                if (!hasThisCourse)
                {
                    return BadRequest("This lecturer does not teach this course.");
                }
            }
            else if(lecturer.Role.Name == "Assistant")
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

            lecture.Date = DateTime.Now;
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
            int cId = lecture.Course.Id;
            lecture.QrCode = null;
            lecture.Lecturer = null;
            lecture.Course = null;
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
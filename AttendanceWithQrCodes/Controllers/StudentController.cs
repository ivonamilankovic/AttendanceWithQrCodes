using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Linq;
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
    public class StudentController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public StudentController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns list of students.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll(int studyProfile, int studyLanguage)
        {
            IList<StudentInformation> students = await _context.StudentInformations
                                                .Include(s => s.User)
                                                .Include(s => s.StudyLanguage)
                                                .Include(s => s.StudyProfile)
                                                .WhereIf(studyProfile != 0, s => s.StudyProfile.Id == studyProfile)
                                                .WhereIf(studyLanguage != 0, s => s.StudyLanguage.Id == studyLanguage)
                                                .ToListAsync();
            if (!students.Any())
            {
                return NoContent();
            }

            IList<StudentInfoDetailsDto> studentInfoDtos = _mapper.Map<IList<StudentInformation>, IList<StudentInfoDetailsDto>>(students);
            return Ok(studentInfoDtos);
        }

        /// <summary>
        /// Returns student infos by index number.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet("{index}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int index)
        {
            StudentInformation? student = await _context.StudentInformations
                                        .Include(s => s.User)                
                                        .Include(s => s.StudyLanguage)                
                                        .Include(s => s.StudyProfile)                
                                        .SingleOrDefaultAsync(s => s.Index == index);
            if(student == null)
            {
                return NotFound();
            }

            StudentInfoDetailsDto studentDetailsDto = _mapper.Map<StudentInformation, StudentInfoDetailsDto>(student);
            return Ok(studentDetailsDto);
        }

        /// <summary>
        /// Creates new student.
        /// </summary>
        /// <param name="studentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(StudentInfoCreateDto studentDto)
        {
            if (studentDto == null)
            {
                return BadRequest();
            }

            bool indexExists = await _context.StudentInformations.AnyAsync(s => s.Index == studentDto.Index);
            if (indexExists)
            {
                return BadRequest("Index you provided already exists. It must be unique!");
            }

            bool userAlreadyStudent = await _context.StudentInformations.Include(s => s.User).AnyAsync(s => s.User.Id == studentDto.UserId);
            if (userAlreadyStudent)
            {
                return BadRequest("User you provided is already set to another index number. Choose another user.");
            }

            bool langExists = await _context.StudyLanguages.AnyAsync(l => l.Id == studentDto.StudyLanguageId);
            if (!langExists)
            {
                return BadRequest("Language with provided id does not exists.");
            }

            bool profileExists = await _context.StudyProfiles.AnyAsync(p => p.Id == studentDto.StudyProfileId);
            if (!profileExists)
            {
                return BadRequest("Profile with provided id does not exists.");
            }

            StudentInformation student = _mapper.Map<StudentInfoCreateDto, StudentInformation>(studentDto);
            _context.StudentInformations.Add(student);
            await _context.SaveChangesAsync();

            StudentInfoDetailsDto studentDetailsDto = _mapper.Map<StudentInformation, StudentInfoDetailsDto>(student);
            return Ok(studentDetailsDto);
        }

        /// <summary>
        /// Updates student info by index number.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="studentDto"></param>
        /// <returns></returns>
        [HttpPut("{index}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int index, StudentInfoUpdateDto studentDto)
        {
            StudentInformation? student = await _context.StudentInformations
                                        .Include(s => s.User)
                                        .Include(s => s.StudyLanguage)
                                        .Include(s => s.StudyProfile)
                                        .SingleOrDefaultAsync(s => s.Index == index);
            if(student == null)
            {
                return NotFound();
            }

            if (studentDto == null)
            {
                return BadRequest();
            }

            bool langExists = await _context.StudyLanguages.AnyAsync(l => l.Id == studentDto.StudyLanguageId);
            if (!langExists)
            {
                return BadRequest("Language with provided id does not exists.");
            }

            bool profileExists = await _context.StudyProfiles.AnyAsync(p => p.Id == studentDto.StudyProfileId);
            if (!profileExists)
            {
                return BadRequest("Profile with provided id does not exists.");
            }

            _mapper.Map(studentDto, student);
            await _context.SaveChangesAsync();

            student.StudyLanguage = await _context.StudyLanguages.SingleOrDefaultAsync(l => l.Id == studentDto.StudyLanguageId);
            student.StudyProfile = await _context.StudyProfiles.SingleOrDefaultAsync(p => p.Id == studentDto.StudyProfileId);
            StudentInfoDetailsDto studentDetailsDto = _mapper.Map<StudentInformation, StudentInfoDetailsDto>(student);
            return Ok(studentDetailsDto);
        }

        /// <summary>
        /// Deletes students info by index number.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpDelete("{index}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int index)
        {
            StudentInformation? student = await _context.StudentInformations.SingleOrDefaultAsync(s => s.Index == index);
            if(student == null)
            {
                return NotFound();
            }

            _context.StudentInformations.Remove(student);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

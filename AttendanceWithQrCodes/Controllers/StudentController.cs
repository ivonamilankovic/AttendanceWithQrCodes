using AttendanceWithQrCodes.Data;
using static AttendanceWithQrCodes.Data.RoleConstants;
using AttendanceWithQrCodes.HelperMethods;
using AttendanceWithQrCodes.Linq;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Hangfire;

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
        private readonly IFetchAuthHeader _fetchAuthHeader;
        private readonly IDeletingHelperMethods _deletingHelper;
        private readonly string _baseUrl;
        public StudentController(Context context, IMapper mapper, IFetchAuthHeader fetchAuthHeader, IDeletingHelperMethods deletingHelper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _fetchAuthHeader = fetchAuthHeader;
            _deletingHelper = deletingHelper;
            _baseUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}";
        }

        /// <summary>
        /// Returns list of students.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll(int studyProfile, int studyLanguage)
        {
            IList<StudentInformation> students = await _context.StudentInformations
                                                .Include(s => s.User)
                                                .Include(s => s.StudyLanguage)
                                                .Include(s => s.StudyProfile)
                                                .WhereIf(studyProfile != 0, s => s.StudyProfileId == studyProfile)
                                                .WhereIf(studyLanguage != 0, s => s.StudyLanguageId == studyLanguage)
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
        [Authorize(Roles = AdminRole + "," + ProfessorRole + "," + AssistantRole + "," + StudentRole)]
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
        [Authorize(Roles = AdminRole + "," + StudentRole + "," + DefaultRole)]
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

            bool userAlreadyStudent = await _context.StudentInformations.Include(s => s.User).AnyAsync(s => s.UserId == studentDto.UserId);
            if (userAlreadyStudent)
            {
                return BadRequest("User you provided is already set to another index number. Choose another user.");
            }

            User? user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == studentDto.UserId);
            if(user.Role.Name == DefaultRole)
            {
                user.Role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == DefaultRole);
            }
            if (user.Role.Name != StudentRole)
            {
                return BadRequest("User you provided is not student. Choose another user.");
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

            student.StudyLanguage = await _context.StudyLanguages.SingleOrDefaultAsync(l => l.Id == studentDto.StudyLanguageId);
            student.StudyProfile = await _context.StudyProfiles.SingleOrDefaultAsync(p => p.Id == studentDto.StudyProfileId);
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
        [Authorize(Roles = AdminRole + "," + StudentRole)]
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
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int index)
        {
            StudentInformation? student = await _context.StudentInformations.SingleOrDefaultAsync(s => s.Index == index);
            if(student == null)
            {
                return NotFound();
            }

            string authHeaderValue = _fetchAuthHeader.FetchAuthorizationHeaderValue(Request);
            string apiPath = _baseUrl + "/api/StudentAttendance/";

            BackgroundJob.Enqueue(() => _deletingHelper.DeleteAttendances(index, authHeaderValue, apiPath));
            BackgroundJob.Enqueue(() => _deletingHelper.DeleteStudentInfo(index));

            return Ok();
        }
    }
}

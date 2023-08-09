using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Models.DTOs;
using AttendanceWithQrCodes.Models;
using static AttendanceWithQrCodes.Data.RoleConstants;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace AttendanceWithQrCodes.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    public class StudyLanguageController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public StudyLanguageController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns list of study languages.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll()
        {
            IList<StudyLanguage> langs = await _context.StudyLanguages.ToListAsync();
            if (!langs.Any())
            {
                return NoContent();
            }

            return Ok(langs);
        }

        /// <summary>
        /// Returns study languages by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            StudyLanguage? lang = await _context.StudyLanguages.SingleOrDefaultAsync(l => l.Id == id);
            if (lang == null)
            {
                return NotFound();
            }

            return Ok(lang);
        }

        /// <summary>
        /// Creates new study language.
        /// </summary>
        /// <param name="langDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Create(StudyLanguageDto langDto)
        {
            if (langDto.Name.IsNullOrEmpty())
            {
                return BadRequest();
            }

            bool langExists = await _context.StudyLanguages.AnyAsync(l => l.Name == langDto.Name);
            if (langExists)
            {
                return BadRequest("Language already exists!");
            }

            StudyLanguage lang = _mapper.Map<StudyLanguageDto, StudyLanguage>(langDto);
            _context.StudyLanguages.Add(lang);
            await _context.SaveChangesAsync();

            return Ok(lang);
        }

        /// <summary>
        /// Updates study language by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="langDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Update(int id, StudyLanguageDto langDto)
        {
            if (langDto.Name.IsNullOrEmpty())
            {
                return BadRequest();
            }

            bool langExists = await _context.StudyLanguages.AnyAsync(l => l.Name == langDto.Name && l.Id != id);
            if (langExists)
            {
                return BadRequest("Language already exists!");
            }

            StudyLanguage? lang = await _context.StudyLanguages.SingleOrDefaultAsync(l => l.Id == id);
            if (lang == null)
            {
                return NotFound();
            }

            _mapper.Map(langDto, lang);
            await _context.SaveChangesAsync();

            return Ok(lang);
        }

        /// <summary>
        /// Deletes study language by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            StudyLanguage? lang = await _context.StudyLanguages.SingleOrDefaultAsync(l => l.Id == id); ;
            if (lang == null)
            {
                return NotFound();
            }

            IList<CourseLanguage> courseLanguages = await _context.CoursesLanguages
                                                .Include(cl => cl.StudyLanguage)
                                                .Where(cl => cl.StudyLanguageId == id)
                                                .ToListAsync();
            foreach(CourseLanguage courseLanguage in courseLanguages)
            {
                _context.CoursesLanguages.Remove(courseLanguage);
            }

            IList<StudentInformation> students = await _context.StudentInformations
                                                .Include(s => s.StudyLanguage)
                                                .Where(s => s.StudyLanguageId == id)
                                                .ToListAsync();
            foreach(StudentInformation s in students)
            {
                s.StudyLanguage = null;   
            }

            _context.StudyLanguages.Remove(lang);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

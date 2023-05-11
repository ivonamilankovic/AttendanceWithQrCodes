using AttendanceWithQrCodes.Data;
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
    public class StudyProfileController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public StudyProfileController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns list of study profiles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll()
        {
            IList<StudyProfile> profiles = await _context.StudyProfiles.ToListAsync();
            if (!profiles.Any())
            {
                return NoContent();
            }

            return Ok(profiles);
        }

        /// <summary>
        /// Returns study profile by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            StudyProfile? profile = await _context.StudyProfiles.SingleOrDefaultAsync(p => p.Id == id);
            if(profile == null)
            {
                return NotFound();
            }

            StudyProfileDto profileDto = _mapper.Map<StudyProfile, StudyProfileDto>(profile);
            return Ok(profileDto);
        }

        /// <summary>
        /// Creates new study profile.
        /// </summary>
        /// <param name="profileDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(StudyProfileDto profileDto)
        {
            if (profileDto.ProfileName.IsNullOrEmpty())
            {
                return BadRequest();
            }

            bool profileExists = await _context.StudyProfiles.AnyAsync(p => p.Name == profileDto.ProfileName);
            if (profileExists)
            {
                return BadRequest("Profile already exists!");
            }

            StudyProfile profile = _mapper.Map<StudyProfileDto, StudyProfile>(profileDto);
            _context.StudyProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return Ok(profile);
        }

        /// <summary>
        /// Updates study profile by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, StudyProfileDto profileDto)
        {
            if (profileDto.ProfileName.IsNullOrEmpty())
            {
                return BadRequest();
            }

            bool profileExists = await _context.StudyProfiles.AnyAsync(p => p.Name == profileDto.ProfileName && p.Id != id);
            if (profileExists)
            {
                return BadRequest("Profile already exists!");
            }

            StudyProfile? profile = await _context.StudyProfiles.SingleOrDefaultAsync(p => p.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            _mapper.Map(profileDto, profile);
            await _context.SaveChangesAsync();

            return Ok(profile);
        }

        /// <summary>
        /// Deletes study profile by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            StudyProfile? profile = await _context.StudyProfiles.SingleOrDefaultAsync(p => p.Id == id);
            if(profile == null)
            {
                return NotFound();
            }

            IList<CourseStudyProfile> courseStudyProfiles = await _context.CoursesStudyProfiles
                                                .Include(cl => cl.StudyProfile)
                                                .Where(cl => cl.StudyProfileId == id)
                                                .ToListAsync();
            foreach (CourseStudyProfile courseStudyProfile in courseStudyProfiles)
            {
                _context.CoursesStudyProfiles.Remove(courseStudyProfile);
            }

            IList<StudentInformation> students = await _context.StudentInformations
                                                .Include(s => s.StudyProfile)
                                                .Where(s => s.StudyProfileId == id)
                                                .ToListAsync();
            foreach (StudentInformation s in students)
            {
                s.StudyProfile = null;
            }

            _context.StudyProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}

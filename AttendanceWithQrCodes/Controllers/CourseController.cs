using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace AttendanceWithQrCodes.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public CourseController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IList<Course> courses = await _context.Courses.Include(c => c.Assistant)
                                         .Include(c => c.Professor)
                                         .ToListAsync();
            return Ok(courses);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateUpdateDto courseDto) 
        {
            if(courseDto == null)
            {
                return BadRequest();
            }

            bool sameCourseExists = await _context.Courses.AnyAsync(c => c.Name == courseDto.Name);
            if (sameCourseExists)
            {
                return BadRequest("Course with same name already exists.");
            }  

            User? professor = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == courseDto.ProfessorId);
            if(professor == null)
            {
                return NotFound("User you provided for professor does not exist.");
            }
            if (professor.Role.Name != "Professor")
            {
                return BadRequest("User you provided for professor does not have role of professor.");
            }
            if(courseDto.LecturesNumForProfessor <= 0)
            {
                return BadRequest("Number of lectures must be grater than 0.");
            }

            if(courseDto.AssistantId != 0)
            {
                User? assistant = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == courseDto.AssistantId);
                if (assistant == null)
                {
                    return NotFound("User you provided for assistant does not exist.");
                }
                if (assistant.Role.Name != "Assistant")
                {
                    return BadRequest("User you provided for assistant does not have role of assistant.");
                }
                if(courseDto.LecturesNumForAssistent <= 0)
                {
                    return BadRequest("Number of lecturese must be grater than 0.");
                }
            }
            else
            {
                courseDto.LecturesNumForAssistent = null;
                courseDto.AssistantId = null;
            }

            if (!courseDto.CourseLanguages.Any())
            {
                return BadRequest("You must provide languages for this course.");
            }

            if (!courseDto.CourseStudyProfiles.Any())
            {
                return BadRequest("You must provide study profiles for this course.");
            }

            Course course = _mapper.Map<CourseCreateUpdateDto, Course>(courseDto);
            course.CourseLanguages = null;
            course.CourseStudyProfiles = null;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            foreach(CourseLanguageIds languageId in courseDto.CourseLanguages)
            {
                CourseLanguage cl = new CourseLanguage
                {
                    CourseId = course.Id,
                    StudyLanguageId = languageId.Id
                };
                _context.CoursesLanguages.Add(cl);
            }

            foreach (CourseStudyProfilesIds profileId in courseDto.CourseStudyProfiles)
            {
                CourseStudyProfile cp = new CourseStudyProfile
                {
                    CourseId = course.Id,
                    StudyProfileId = profileId.Id
                };
                _context.CoursesStudyProfiles.Add(cp);
            }

            await _context.SaveChangesAsync();

            course.CourseLanguages = await _context.CoursesLanguages    
                                    .Where(cp => cp.CourseId == course.Id)  
                                    .ToArrayAsync();
            course.CourseStudyProfiles = await _context.CoursesStudyProfiles
                                    .Where(cp => cp.CourseId == course.Id)
                                    .ToArrayAsync();

            return Ok(course);
        }
    }
}

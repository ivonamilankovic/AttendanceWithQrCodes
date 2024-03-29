﻿using AttendanceWithQrCodes.Data;
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
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly IFetchAuthHeader _fetchAuthHeader;
        private readonly IDeletingHelperMethods _deletingHelper;
        private readonly string _baseUrl;
        public CourseController(Context context, IMapper mapper, IFetchAuthHeader fetchAuthHeader, IDeletingHelperMethods deletingHelper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _fetchAuthHeader = fetchAuthHeader;
            _deletingHelper = deletingHelper;
            _baseUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}";
        }

        /// <summary>
        /// Returns list of courses.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll(int professorId, int assistantId, int languageId, int profileId)
        {
            IList<Course> courses = await _context.Courses
                                         .Include(c => c.Assistant)
                                         .Include(c => c.Professor)
                                         .WhereIf(professorId != 0, c => c.ProfessorId == professorId)
                                         .WhereIf(assistantId != 0, c => c.AssistantId == assistantId)
                                         .ToListAsync();
            if (!courses.Any())
            {
                return NoContent();
            }

            List<Course> filteredCourses = new List<Course>();

            foreach(Course course in courses)
            {
                course.CourseLanguages = await _context.CoursesLanguages
                                   .Include(cl => cl.StudyLanguage)
                                   .Where(cl => cl.CourseId == course.Id)
                                   .ToArrayAsync();
                course.CourseStudyProfiles = await _context.CoursesStudyProfiles
                                    .Include(cp => cp.StudyProfile)
                                    .Where(cp => cp.CourseId == course.Id)
                                    .ToArrayAsync();

                bool anyRequired = languageId != 0 || profileId != 0;

                if (anyRequired)
                {
                    IList<CourseLanguage> languages = await _context.CoursesLanguages
                                       .Include(cl => cl.StudyLanguage)
                                       .Where(cl => cl.CourseId == course.Id)
                                       .Where(cl => cl.StudyLanguageId == languageId)
                                       .ToArrayAsync();
                    IList<CourseStudyProfile> profiles = await _context.CoursesStudyProfiles
                                        .Include(cp => cp.StudyProfile)
                                        .Where(cp => cp.CourseId == course.Id)
                                        .Where(cp => cp.StudyProfileId == profileId)
                                        .ToArrayAsync();

                    bool bothRequired = languageId != 0 && profileId != 0;
                    bool hasLanguages = languages.Any();
                    bool hasProfiles = profiles.Any();

                    if (!bothRequired && hasLanguages)
                    {
                        filteredCourses.Add(course);
                    }
                    else if (!bothRequired && hasProfiles)
                    {
                        filteredCourses.Add(course);
                    }
                    else if (bothRequired && hasProfiles && hasLanguages)
                    {
                        filteredCourses.Add(course);
                    }
                }
                else
                {
                    filteredCourses.Add(course);
                }
            }

            filteredCourses = filteredCourses.Distinct().ToList();
            IList<CourseDetailsDto> courseList = _mapper.Map<IList<Course>, IList<CourseDetailsDto>>(filteredCourses);
            if (!courseList.Any())
            {
                return NoContent();
            }
            return Ok(courseList);
        }

        /// <summary>
        /// Returns course by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            Course? course = await _context.Courses
                                .Include(c => c.Professor)                 
                                .Include(c => c.Assistant)                 
                                .Where(c => c.Id == id)
                                .SingleOrDefaultAsync();
            if(course == null)
            {
                return NotFound();
            }

            course.CourseLanguages = await _context.CoursesLanguages
                                   .Include(cl => cl.StudyLanguage)
                                   .Where(cl => cl.CourseId == course.Id)
                                   .ToArrayAsync();
            course.CourseStudyProfiles = await _context.CoursesStudyProfiles
                                .Include(cp => cp.StudyProfile)
                                .Where(cp => cp.CourseId == course.Id)
                                .ToArrayAsync();

            CourseDetailsDto courseDetails = _mapper.Map<Course, CourseDetailsDto>(course);
            return Ok(courseDetails);
        } 

        /// <summary>
        /// Creates new course.
        /// </summary>
        /// <param name="courseDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = AdminRole + "," + ProfessorRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
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
            if (professor.Role.Name != ProfessorRole)
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
                if (assistant.Role.Name != AssistantRole)
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

            foreach(int languageId in courseDto.CourseLanguages)
            {
                CourseLanguage cl = new CourseLanguage
                {
                    CourseId = course.Id,
                    StudyLanguageId = languageId
                };
                _context.CoursesLanguages.Add(cl);
            }

            foreach (int profileId in courseDto.CourseStudyProfiles)
            {
                CourseStudyProfile cp = new CourseStudyProfile
                {
                    CourseId = course.Id,
                    StudyProfileId = profileId
                };
                _context.CoursesStudyProfiles.Add(cp);
            }
            
            await _context.SaveChangesAsync();

            course.CourseLanguages = await _context.CoursesLanguages
                                    .Include(cl => cl.StudyLanguage)
                                    .Where(cp => cp.CourseId == course.Id)  
                                    .ToArrayAsync();
            course.CourseStudyProfiles = await _context.CoursesStudyProfiles
                                    .Include(cp => cp.StudyProfile)
                                    .Where(cp => cp.CourseId == course.Id)
                                    .ToArrayAsync();

            CourseDetailsDto courseDetails = _mapper.Map<Course, CourseDetailsDto>(course);
            return Ok(courseDetails);
        }

        /// <summary>
        /// Updates course by id.
        /// </summary>
        /// <param name="courseDto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = AdminRole + "," + ProfessorRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Update(CourseCreateUpdateDto courseDto, int id)
        {
            if (courseDto == null)
            {
                return BadRequest();
            }

            bool sameCourseExists = await _context.Courses.AnyAsync(c => c.Name == courseDto.Name && c.Id != id);
            if (sameCourseExists)
            {
                return BadRequest("Course with same name already exists.");
            }

            User? professor = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == courseDto.ProfessorId);
            if (professor == null)
            {
                return NotFound("User you provided for professor does not exist.");
            }
            if (professor.Role.Name != ProfessorRole)
            {
                return BadRequest("User you provided for professor does not have role of professor.");
            }
            if (courseDto.LecturesNumForProfessor <= 0)
            {
                return BadRequest("Number of lectures must be grater than 0.");
            }

            if (courseDto.AssistantId != 0)
            {
                User? assistant = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == courseDto.AssistantId);
                if (assistant == null)
                {
                    return NotFound("User you provided for assistant does not exist.");
                }
                if (assistant.Role.Name != AssistantRole)
                {
                    return BadRequest("User you provided for assistant does not have role of assistant.");
                }
                if (courseDto.LecturesNumForAssistent <= 0)
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

            Course? course = await _context.Courses
                                .Include(c => c.Professor)
                                .Include(c => c.Assistant)
                                .Where(c => c.Id == id)
                                .SingleOrDefaultAsync();
            if(course == null)
            {
                return NotFound();
            }
            _mapper.Map<CourseCreateUpdateDto, Course>(courseDto, course);
            course.CourseLanguages = null;
            course.CourseStudyProfiles = null;
            await _context.SaveChangesAsync();

            course.CourseLanguages = await _context.CoursesLanguages
                                   .Include(cl => cl.StudyLanguage)
                                   .Where(cp => cp.CourseId == id)
                                   .ToListAsync();
            course.CourseStudyProfiles = await _context.CoursesStudyProfiles
                                    .Include(cp => cp.StudyProfile)
                                    .Where(cp => cp.CourseId == id)
                                    .ToListAsync();
            
            foreach (int languageId in courseDto.CourseLanguages)
            {
                CourseLanguage cl = new CourseLanguage
                {
                    CourseId = id,
                    StudyLanguageId = languageId
                };

                if (!course.CourseLanguages.Any(c => c.StudyLanguageId == cl.StudyLanguageId))
                {
                    _context.CoursesLanguages.Add(cl);
                }                
            }
            foreach (CourseLanguage language in course.CourseLanguages)
            {
                if (!courseDto.CourseLanguages.Any(cl => cl == language.StudyLanguageId))
                {
                    _context.CoursesLanguages.Remove(language);
                }
            }

            foreach (int profileId in courseDto.CourseStudyProfiles)
            {
                CourseStudyProfile cp = new CourseStudyProfile
                {
                    CourseId = id,
                    StudyProfileId = profileId
                };

                if (!course.CourseStudyProfiles.Any(c => c.StudyProfileId == cp.StudyProfileId))
                {
                    _context.CoursesStudyProfiles.Add(cp);
                }
            }
            foreach (CourseStudyProfile profile in course.CourseStudyProfiles)
            {
                if (!courseDto.CourseStudyProfiles.Any(cp => cp == profile.StudyProfileId))
                {
                    _context.CoursesStudyProfiles.Remove(profile);
                }
            }

            await _context.SaveChangesAsync();

            course.CourseLanguages = await _context.CoursesLanguages
                                    .Include(cl => cl.StudyLanguage)
                                    .Where(cp => cp.CourseId == course.Id)
                                    .ToArrayAsync();
            course.CourseStudyProfiles = await _context.CoursesStudyProfiles
                                    .Include(cp => cp.StudyProfile)
                                    .Where(cp => cp.CourseId == course.Id)
                                    .ToArrayAsync();

            CourseDetailsDto courseDetails = _mapper.Map<Course, CourseDetailsDto>(course);
            return Ok(courseDetails);
        }

        /// <summary>
        /// Deletes course by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = AdminRole + "," + ProfessorRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            Course? course = await _context.Courses
                                .Where(c => c.Id == id)
                                .SingleOrDefaultAsync();
            if (course == null)
            {
                return NotFound();
            }

            string authHeaderValue = _fetchAuthHeader.FetchAuthorizationHeaderValue(Request);
            string apiPath = _baseUrl + "/api/Lecture/";

            BackgroundJob.Enqueue( () => _deletingHelper.DeleteLectures(id, authHeaderValue, apiPath));
            BackgroundJob.Enqueue( () => _deletingHelper.DeleteCourseLanguagesAndProfiles(id));
            BackgroundJob.Enqueue( () => _deletingHelper.DeleteCourse(id));
            return Ok();
        }

    }
}

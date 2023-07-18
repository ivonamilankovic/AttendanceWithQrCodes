using AttendanceWithQrCodes.Data;
using static AttendanceWithQrCodes.Data.RoleConstants;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace AttendanceWithQrCodes.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public UserController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns list of users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = AdminRole + "," + ProfessorRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll()
        {
            IList<User> users = await _context.Users.Include(u => u.Role).ToListAsync();
            if (!users.Any())
            {
                return NoContent();
            }

            IList<UserListDto> usersDto = _mapper.Map<IList<User>, IList<UserListDto>>(users);
            return Ok(usersDto);
        }

        /// <summary>
        /// Returns user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            User? user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            UserDto userDto = _mapper.Map<User, UserDto>(user);
            return Ok(userDto);
        }

        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(UserCreateDto userDto)
        {
            if(userDto == null)
            {
                return BadRequest();
            }

            bool emailTaken = await _context.Users.AnyAsync(u => u.Email == userDto.Email);
            if (emailTaken)
            {
                return BadRequest("Email already taken.");
            }

            User user = _mapper.Map<UserCreateDto, User>(userDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user.Role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == user.RoleId);
            UserDto userData = _mapper.Map<User, UserDto>(user);
            return Ok(userData);
        }

        /// <summary>
        /// Updates user by id.
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(UserUpdateDto userDto, int id)
        {
            User? user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }

            if (userDto.FirstName.IsNullOrEmpty() || userDto.LastName.IsNullOrEmpty())
            {
                return BadRequest();
            }
            if(userDto.RoleId != 0)
            {
                user.RoleId = userDto.RoleId;
                user.Role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == userDto.RoleId);
            }

            _mapper.Map(userDto, user);
            await _context.SaveChangesAsync();

            user.Role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == user.RoleId);
            UserDto userData = _mapper.Map<User, UserDto>(user);
            return Ok(userData);
        }
        
        /// <summary>
        /// changes users password by id.
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("ChangePassword/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword(UserChangePasswordDto userDto, int id)
        {
            User? user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }

            if (userDto.Password.IsNullOrEmpty())
            {
                return BadRequest();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = AdminRole)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            User? user = await _context.Users.Include(u => u.Role)
                        .SingleOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if(user.Role.Name == StudentRole)
            {
                StudentInformation? studentInformation = await _context.StudentInformations
                                                    .SingleOrDefaultAsync(s => s.UserId == id);
                if (studentInformation != null)
                {
                    IList<StudentAttendance> attendances = await _context.StudentAttendances
                                                        .Where(a => a.StudentIndex == studentInformation.Index)
                                                        .ToListAsync();
                    foreach (StudentAttendance a in attendances)
                    {
                        _context.StudentAttendances.Remove(a);
                    }
                    await _context.SaveChangesAsync();
                    _context.StudentInformations.Remove(studentInformation);
                    await _context.SaveChangesAsync();
                }
            }
            else if (user.Role.Name == ProfessorRole)
            {
                IList<Lecture> lectures = await _context.Lectures
                                        .Where(l => l.LecturerId == id)
                                        .ToListAsync();
                foreach(Lecture l in lectures)
                {
                    l.LecturerId = null;
                }

                IList<Course> courses = await _context.Courses
                                        .Where(l => l.ProfessorId == id)
                                        .ToListAsync();
                foreach(Course c in courses)
                {
                    c.ProfessorId = null;
                }

                await _context.SaveChangesAsync();
            }
            else if (user.Role.Name == AssistantRole)
            {
                IList<Lecture> lectures = await _context.Lectures
                                        .Where(l => l.LecturerId == id)
                                        .ToListAsync();
                foreach (Lecture l in lectures)
                {
                    l.LecturerId = null;
                }

                IList<Course> courses = await _context.Courses
                                        .Where(l => l.AssistantId == id)
                                        .ToListAsync();
                foreach (Course c in courses)
                {
                    c.AssistantId = null;
                }

                await _context.SaveChangesAsync();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}

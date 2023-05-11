﻿using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(UserCreateUpdateDto userDto)
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

            User user = _mapper.Map<UserCreateUpdateDto, User>(userDto);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(UserCreateUpdateDto userDto, int id)
        {
            User? user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }

            if (userDto == null)
            {
                return BadRequest();
            }

            bool emailTaken = await _context.Users.AnyAsync(u => u.Email == userDto.Email && u.Id != id);
            if (emailTaken)
            {
                return BadRequest("Email already taken.");
            }

            _mapper.Map(userDto, user);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            await _context.SaveChangesAsync();

            user.Role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == user.RoleId);
            UserDto userData = _mapper.Map<User, UserDto>(user);
            return Ok(userData);
        }

        /// <summary>
        /// Deletes user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
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

            if(user.Role.Name == "Student")
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
            else if (user.Role.Name == "Professor")
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
            else if (user.Role.Name == "Assistant")
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

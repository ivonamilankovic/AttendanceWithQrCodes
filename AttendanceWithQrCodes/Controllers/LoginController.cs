﻿using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.HelperMethods;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Mime;

namespace AttendanceWithQrCodes.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Context _context;
        private readonly IJwtHelper _jwtHelper;

        public LoginController(Context context, IJwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Logs user in.
        /// </summary>
        /// <param name="userLoginDto"></param>
        /// <returns>jwt token</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginAsync(UserLoginDto userLoginDto)
        {
            User? user = await _context.Users.Include(u => u.Role)
                                .SingleOrDefaultAsync(x => x.Email == userLoginDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid email address.");
            }

            if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
            {
                return BadRequest("Invalid password.");
            }

            string token = _jwtHelper.GenerateLoginToken(user);
            JObject jsonObject = new JObject();
            jsonObject["Token"] = token;
            return Ok(jsonObject);
        }

        /// <summary>
        /// Validates Jwt token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>User</returns>
        [HttpGet("Validate")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ValidateToken(string token)
        {
            int userId = 0;
            try
            {
                userId = _jwtHelper.ValidateLoginToken(token);
            }catch(Exception e) 
            { 
                return NoContent();
            }

            User? user = await _context.Users.Include(u => u.Role)
                        .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return BadRequest("Invalid token.");
            }

            if(user.Role.Name == RoleConstants.StudentRole)
            {
                StudentInformation? student = await _context.StudentInformations.Include(s => s.User)
                    .Include(s => s.StudyLanguage).Include(s=>s.StudyProfile)
                    .SingleOrDefaultAsync(s => s.UserId == userId);
                if (student != null)
                {
                    return Ok(student);
                }
            }

            return Ok(user);
        }

    }
}

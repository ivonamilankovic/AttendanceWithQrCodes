using AttendanceWithQrCodes.Data;
using AttendanceWithQrCodes.HelperMethods;
using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Security.Claims;

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
            return Ok(token);
        }

        /// <summary>
        /// Validates Jwt token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>User</returns>
        [HttpGet("Validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ValidateToken(string token)
        {
            int userId = _jwtHelper.ValidateLoginToken(token);

            User? user = await _context.Users.Include(u => u.Role)
                        .SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return BadRequest("Invalid token.");
            }

            return Ok(user);
        }

    }
}

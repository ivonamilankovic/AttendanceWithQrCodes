using AttendanceWithQrCodes.Data;
using static AttendanceWithQrCodes.Data.RoleConstants;
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
    public class RoleController : ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public RoleController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns list of roles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll()
        {
            IList<Role> roles = await _context.Roles.ToListAsync();
            if (!roles.Any())
            {
                return NoContent();
            }
            return Ok(roles);
        }

        /// <summary>
        /// Returns role by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            Role? role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == id);
            if(role == null)
            {
                return NotFound();
            }

            RoleDto roleDto = _mapper.Map<Role, RoleDto>(role);
            return Ok(roleDto);
        }

        /// <summary>
        /// Creates new role.
        /// </summary>
        /// <param name="roleDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(RoleDto roleDto)
        {
            if(roleDto.RoleName.IsNullOrEmpty())
            {
                return BadRequest();
            }

            bool roleExists = await _context.Roles.AnyAsync(r => r.Name == roleDto.RoleName);
            if (roleExists)
            {
                return BadRequest("Role already exists!");
            }

            Role newRole = _mapper.Map<RoleDto, Role>(roleDto);
            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();
            return Ok(newRole);
        }

        /// <summary>
        /// Updates existing role.
        /// </summary>
        /// <param name="roleDto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(RoleDto roleDto, int id)
        {
            if (roleDto.RoleName.IsNullOrEmpty())
            {
                return BadRequest();
            }

            bool roleExists = await _context.Roles.AnyAsync(r => r.Name == roleDto.RoleName && r.Id != id);
            if (roleExists)
            {
                return BadRequest("Role already exists!");
            }

            Role? role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == id);
            if(role == null)
            {
                return NotFound();
            }

            if(role.Name == DefaultRole)
            {
                return BadRequest("Can't update default role.");
            }

            _mapper.Map<RoleDto, Role>(roleDto, role);
            await _context.SaveChangesAsync();
            return Ok(role);
        }

        /// <summary>
        /// Deletes role.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            Role? role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            
            if (role.Name == DefaultRole)
            {
                return BadRequest("Can't delete default role.");
            }

            Role? defaultRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == DefaultRole);
            if(defaultRole == null)
            {
                return NotFound("Default role (\"User\") not found. Please make one to be able to delete others.");
            }
            IList<User> users = await _context.Users.Where(u => u.RoleId == id).ToListAsync();
            foreach(User u in users)
            {
                u.RoleId = defaultRole.Id;
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}

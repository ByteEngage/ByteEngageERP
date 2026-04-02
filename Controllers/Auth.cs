using System.Security.Claims;
using ByteEngageERP.Data;
using ByteEngageERP.Models;
using ByteEngageERP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteEngageERP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;

        public AuthController(AppDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // 🔐 LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Invalid request" });

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            if (string.IsNullOrEmpty(user.PasswordHash))
                return StatusCode(500, new { message = "User password not set properly" });

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _jwt.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Username,
                    user.Role,
                    user.OrganizationId,
                    user.AddedBy,
                }
            });
        }
        //Create Admin By SuperAdmin
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Username , Password and Organization Are required" });
            if(string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest(new { message = "Username is required" });
            if(string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Password is required" });
            if (dto.OrganizationId <= 0)
                return BadRequest(new { message = "Organization is required" });

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest(new { message = "Username already exists" });
            var addedById = User.FindFirst("AddedBy")?.Value;
            var user = new User
            {
                Username = dto.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Admin",
                OrganizationId = dto.OrganizationId ,
                AddedBy =  int.Parse(addedById)
                
                // 🔥 from form
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Admin created successfully",
                user = new { user.Username, user.Role, user.OrganizationId }
            });
        }
        
        
        // Create User By Admin
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddUser dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Username and Password are required" });

           
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest(new { message = "Username already exists" });

            var tenantId = User.FindFirst("TenantId")?.Value;
            var AddedByID = User.FindFirst("AddedBy")?.Value;

            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized(new { message = "Invalid token" });

            var user = new User
            {
                Username = dto.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
                OrganizationId = int.Parse(tenantId), // 🔥 auto from admin
                AddedBy = int.Parse(AddedByID) // 🔥 auto from admin
                
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "User created successfully",
                user = new { user.Username, user.Role }
            });
        }
        
        // 👤 GET CURRENT USER
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var tenantId = User.FindFirst("TenantId")?.Value;

            return Ok(new
            {
                username,
                role,
                tenantId
            });
        }
    }
}
using Microsoft.AspNetCore.Http;
using ByteEngageERP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ByteEngageERP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto.Username == "admin" && dto.Password.ToString() == "123456")
            {
                return Ok(new
                {
                    token = "dummy-token",
                    user = new
                    {
                        username = "admin",
                        role = "Admin"
                    }
                });
            }

            return Unauthorized("Invalid credentials");
        }
    }
}

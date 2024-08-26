using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TodoAppAPI.Data;
using TodoAppAPI.DTOs;
using TodoAppAPI.Entities;


namespace TodoAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationLoginDto userRegistration)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userRegistration.Username))
            {
                return BadRequest("Username already exists.");
            }

            try
            {
                var addUser = new User
                {
                    Username = userRegistration.Username,
                    Password = HashPassword(userRegistration.Password)
                };

                _context.Users.Add(addUser);
                await _context.SaveChangesAsync();
                return Ok(userRegistration);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("Database error occurred: " + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest("Argument null error: " + ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest("Invalid format: " + ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserRegistrationLoginDto loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            var hashedInputPassword = HashPassword(loginRequest.Password);

            if (user.Password != hashedInputPassword)
            {
                return Unauthorized("Invalid password");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username
            };


            return Ok(userDto);
        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] int Id)
        {
            var userRequest = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);

            if (userRequest == null)
            {
                return NotFound("User not found");
            }

            var userResponse = new UserDto
            {
                Id = userRequest.Id,
                Username = userRequest.Username
            };

            return Ok(userResponse);
        }
    }

}


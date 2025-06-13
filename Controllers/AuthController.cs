using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using StudentAPI.Model;
using StudentAPI.Service;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IConfiguration configuration, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tokenService = tokenService;
            _logger = logger;
        }

        // Anyone can register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
                return BadRequest(new { message = "Registration data is missing." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return BadRequest(new { message = "Validation failed.", errors });
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
                return BadRequest(new { message = "Username is required." });

            if (string.IsNullOrWhiteSpace(user.PassWord))
                return BadRequest(new { message = "Password is required." });

            if (string.IsNullOrWhiteSpace(user.Role))
                return BadRequest(new { message = "Role is required." });

            if (user.Role != "user" && user.Role != "admin")
                user.Role = "user";

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("Register", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                string hashedPassword = Enpas.HashPassword(user.PassWord);

                cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value = Guid.NewGuid().ToString();
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = user.UserName;
                cmd.Parameters.Add("@PassWord", SqlDbType.NVarChar, 255).Value = hashedPassword;
                cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value = user.Role;

                await con.OpenAsync();
                int i = await cmd.ExecuteNonQueryAsync();
                await con.CloseAsync();

                if (i > 0)
                    return Ok(new { message = "Registration successful!" });
                else
                    return StatusCode(500, new { message = "Registration failed. The username may already exist." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return StatusCode(500, new { message = "An unexpected error occurred during registration. Please try again later." });
            }
        }

        // Anyone can login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginUser)
        {
            if (loginUser == null)
                return BadRequest(new { message = "Login data is missing." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return BadRequest(new { message = "Validation failed.", errors });
            }

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("upLogin", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = loginUser.UserName;

                await con.OpenAsync();
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    string storedPassword = reader["PassWord"].ToString();
                    string hashedInputPassword = Enpas.HashPassword(loginUser.PassWord);

                    if (hashedInputPassword == storedPassword)
                    {
                        User existingUser = new User
                        {
                            UserName = reader["UserName"].ToString(),
                            Role = reader["Role"].ToString()
                        };

                        var token = _tokenService.GenerateToken(existingUser);
                        return Ok(new { token });
                    }
                }
                return Unauthorized(new { message = "Invalid username or password." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new { message = "An unexpected error occurred during login. Please try again later." });
            }
        }

        // Example: Only authenticated users can access this endpoint
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            // You can access user claims here
            var userName = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            return Ok(new { userName, role });
        }
    }
}
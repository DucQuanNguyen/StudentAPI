using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentAPI.DTO;
using StudentAPI.Model;
using StudentAPI.Service;
using StudentAPI.Controllers; // For Mapper

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (registerDto == null)
                return BadRequest(new { message = "Registration data is missing." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return BadRequest(new { message = "Validation failed.", errors });
            }

            if (string.IsNullOrWhiteSpace(registerDto.UserName))
                return BadRequest(new { message = "Username is required." });

            if (string.IsNullOrWhiteSpace(registerDto.PassWord))
                return BadRequest(new { message = "Password is required." });

            if (string.IsNullOrWhiteSpace(registerDto.Role))
                return BadRequest(new { message = "Role is required." });

            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = registerDto.UserName,
                    PassWord = registerDto.PassWord,
                    Role = registerDto.Role
                };

                var result = await _authService.RegisterAsync(user);
                if (result)
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
                var token = await _authService.LoginAsync(loginUser);
                if (token != null)
                    return Ok(new { token });
                return Unauthorized(new { message = "Invalid username or password." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new { message = "An unexpected error occurred during login. Please try again later." });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userName = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            return Ok(new { userName, role });
        }
    }
}
using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using StudentAPI.Model;
using StudentAPI.Service;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IConfiguration configuration, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tokenService = tokenService;
            _logger = logger;
        }

        //public int GetLastUserId()
        //{
        //    using SqlConnection con = new SqlConnection(_connectionString);
        //    SqlDataAdapter adapter = new SqlDataAdapter("GetUser", con);
        //    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
        //    DataTable dataTable = new DataTable();
        //    adapter.Fill(dataTable);
        //    if (dataTable.Rows.Count > 0)
        //        return dataTable.Rows.Count;
        //    else return 0;
        //}
        //public int autoGenNextUserId()
        //{
        //    int Id = GetLastUserId();
        //    return Id+1;
        //}
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("Registration data is missing.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    return BadRequest("Username is required.");
                }
                if (string.IsNullOrWhiteSpace(user.PassWord))
                {
                    return BadRequest("Password is required.");
                }
                if (string.IsNullOrWhiteSpace(user.Role))
                {
                    return BadRequest("Role is required.");
                }
                if (user.Role != "user" && user.Role != "admin")
                {
                    user.Role = "user";
                }
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("Register", con);
                cmd.CommandType = CommandType.StoredProcedure;

                string hashedPassword = Enpas.HashPassword(user.PassWord);

                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@PassWord", hashedPassword);
                cmd.Parameters.AddWithValue("@Role", user.Role);

                await con.OpenAsync();
                int i = await cmd.ExecuteNonQueryAsync();
                await con.CloseAsync();

                if (i > 0)
                    return Ok("Registration successful!");
                else
                    return StatusCode(500, "Registration failed. The username may already exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return StatusCode(500, "An unexpected error occurred during registration. Please try again later.");
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginUser)
        {
            if (loginUser == null)
                return BadRequest("Login data is missing.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("upLogin", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserName", loginUser.UserName);

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
                        return Ok(new { Token = token });
                    }
                }
                return Unauthorized(new { Message = "Invalid username or password." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, "An unexpected error occurred during login. Please try again later.");
            }
        }
    }
    }
}
using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        public AuthController(IConfiguration configuration, ITokenService tokenService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tokenService = tokenService;
        }

        public int GetLastUserId()
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter("GetUser", con);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if (dataTable.Rows.Count > 0)
                return dataTable.Rows.Count;
            else return 0;
        }
        public int autoGenNextUserId()
        {
            int Id = GetLastUserId();
            return Id+1;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                if (user == null || string.IsNullOrWhiteSpace(user.UserName) ||
                string.IsNullOrWhiteSpace(user.PassWord) || string.IsNullOrWhiteSpace(user.Role))
                {
                    return BadRequest("Thông tin đăng ký không hợp lệ!");
                }

                using SqlConnection con = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("Register", con);
                cmd.CommandType = CommandType.StoredProcedure;

                string hashedPassword = Enpas.HashPassword(user.PassWord);

                cmd.Parameters.AddWithValue("@Id", autoGenNextUserId());
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@PassWord", user.PassWord);
                cmd.Parameters.AddWithValue("@Role", user.Role);

                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i > 0 ? Ok("Đăng ký thành công!") : StatusCode(500, "Lỗi đăng ký.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost("login")]
        public IActionResult Login(string UserName, string PassWord)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(PassWord))
            {
                return BadRequest("Tên đăng nhập hoặc mật khẩu không được để trống!");
            }
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                SqlDataAdapter adapter = new SqlDataAdapter("upLogin", con);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapter.SelectCommand.Parameters.AddWithValue("@UserName", UserName);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    string hashedInputPassword = Enpas.HashPassword(PassWord);
                    string storedPassword = dataTable.Rows[0]["PassWord"].ToString();

                    if (hashedInputPassword == storedPassword)
                    {
                        User existingUser = new User
                        {
                            UserName = dataTable.Rows[0]["UserName"].ToString(),
                            Role = dataTable.Rows[0]["Role"].ToString()
                        };

                        var token = _tokenService.GenerateToken(existingUser);
                        return Ok(new { Token = token }); ;
                    }

                }
                return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
    }
}
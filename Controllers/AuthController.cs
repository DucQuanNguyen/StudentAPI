using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using StudentAPI.Model;

namespace StudentAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        public AuthController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        private const string SecretKey = "HS256"; // Khóa bí mật để ký JWT
        public int GetLastUserId()
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter("GetUser", con);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            if(dataTable.Rows.Count > 0 )
            return dataTable.Rows.Count;
            else return 0;
        }
        public int autoGenUserId()
        {
            int Id = GetLastUserId();
            return Id++;
        }
        [HttpPost("register")]
        public String Register([FromBody] User user)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("Register", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", autoGenUserId);
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@PassWord", user.PassWord);
            cmd.Parameters.AddWithValue("@Role", user.Role);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            string ms;
            if (i > 0) { ms = "Add success!"; }
            else { ms = "Error"; }
            return ms;
        }


        [HttpPost("login")]
        public string Login(string UserName, string PassWord)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter("upLogin", con);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("@UserName", UserName);
            adapter.SelectCommand.Parameters.AddWithValue("@PassWord", PassWord);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            User existingUser = new User();
            if (dataTable.Rows.Count > 0)
            {
                existingUser.UserName = dataTable.Rows[0]["UserName"].ToString();
                existingUser.PassWord = dataTable.Rows[0]["PassWord"].ToString();
                var token = GenerateJwtToken(existingUser);
                return token;
            }
            else
            {
                string message = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return message;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
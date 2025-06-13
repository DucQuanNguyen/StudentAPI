using System.Data;
using Microsoft.Data.SqlClient;
using StudentAPI.Controllers;
using StudentAPI.Model;

namespace StudentAPI.Service
{
    public class AuthService
    {
        private readonly string _connectionString;
        private readonly ITokenService _tokenService;

        public AuthService(IConfiguration configuration, ITokenService tokenService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tokenService = tokenService;
        }

        public async Task<bool> RegisterAsync(User user)
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

            return i > 0;
        }

        public async Task<string?> LoginAsync(LoginRequest loginUser)
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

                    return _tokenService.GenerateToken(existingUser);
                }
            }
            return null;
        }
    }
}
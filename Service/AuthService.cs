using System.Data;
using Microsoft.Data.SqlClient;
using StudentAPI.Controllers;
using StudentAPI.Model;

namespace StudentAPI.Service
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly ITokenService _tokenService;

        public AuthService(IConfiguration configuration, ITokenService tokenService)
            : base(configuration)
        {
            _tokenService = tokenService;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            await using var cmd = await CreateCommandAsync("Register").ConfigureAwait(false);

            string hashedPassword = Enpas.HashPassword(user.PassWord);

            cmd.Parameters.AddWithValue("@Id", user.Id.ToString());
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@PassWord", hashedPassword);
            cmd.Parameters.AddWithValue("@Role", user.Role);

            int i = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            return i > 0;
        }

        public async Task<string?> LoginAsync(LoginRequest loginUser)
        {
            await using var cmd = await CreateCommandAsync("upLogin").ConfigureAwait(false);
            cmd.Parameters.AddWithValue("@UserName", loginUser.UserName);

            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                string? storedPassword = reader["PassWord"] as string;
                string hashedInputPassword = Enpas.HashPassword(loginUser.PassWord);

                if (!string.IsNullOrEmpty(storedPassword) && hashedInputPassword == storedPassword)
                {
                    var existingUser = new User
                    {
                        UserName = reader["UserName"] as string ?? string.Empty,
                        Role = reader["Role"] as string ?? string.Empty
                    };

                    return _tokenService.GenerateToken(existingUser);
                }
            }
            return null;
        }
    }
}
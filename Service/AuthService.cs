using System.Data;
using Microsoft.Data.SqlClient;
using StudentAPI.Controllers;
using StudentAPI.Model;

namespace StudentAPI.Service
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IDataMapper<User> _mapper;
        private readonly IParameterAdder<User> _parameterAdder;

        public AuthService(IConfiguration configuration, ITokenService tokenService)
            : base(configuration)
        {
            _tokenService = tokenService;
            _mapper = new UserMapper();
            _parameterAdder = new UserParameterAdder();
        }

        public async Task<bool> RegisterAsync(User user)
        {
            await using var cmd = await CreateCommandAsync("Register").ConfigureAwait(false);

            user.PassWord = Enpas.HashPassword(user.PassWord);
            _parameterAdder.AddParameters(cmd, user);

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
                var existingUser = _mapper.Map(reader);
                string hashedInputPassword = Enpas.HashPassword(loginUser.PassWord);

                if (!string.IsNullOrEmpty(existingUser.PassWord) && hashedInputPassword == existingUser.PassWord)
                {
                    // Không trả về PassWord cho token
                    existingUser.PassWord = string.Empty;
                    return _tokenService.GenerateToken(existingUser);
                }
            }
            return null;
        }
    }
}
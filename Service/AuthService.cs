using StudentAPI.Controllers;
using StudentAPI.Model;
using StudentAPI.Service;

namespace StudentAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User, Guid> _repository;
        private readonly ITokenService _tokenService;

        public AuthService(
            IRepository<User, Guid> repository,
            ITokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            user.PassWord = Enpas.HashPassword(user.PassWord);
            var created = await _repository.CreateAsync(user);
            return created != null;
        }

        public async Task<string?> LoginAsync(LoginRequest loginUser)
        {
            var user = await _repository.GetByUserNameAsync(loginUser.UserName);

            if (user != null)
            {
                string hashedInputPassword = Enpas.HashPassword(loginUser.PassWord);
                if (!string.IsNullOrEmpty(user.PassWord) && hashedInputPassword == user.PassWord)
                {
                    user.PassWord = string.Empty; // Không trả về password
                    return _tokenService.GenerateToken(user);
                }
            }
            return null;
        }
    }
}
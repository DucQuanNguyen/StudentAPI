using StudentAPI.Model;

namespace StudentAPI.Service
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(User user);
        Task<string?> LoginAsync(LoginRequest loginUser);
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudentAPI.Model;

namespace StudentAPI.Service
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
    
}

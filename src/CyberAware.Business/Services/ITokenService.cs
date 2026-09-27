using CyberAware.DataAccess.Entities;

namespace CyberAware.Business.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
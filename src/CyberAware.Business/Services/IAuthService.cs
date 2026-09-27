using CyberAware.Business.DTOs;

namespace CyberAware.Business.Services;

public interface IAuthService
{
    Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> VerifyTwoFactorAsync(Verify2FARequest request);
    Task<Setup2FAResponse?> SetupTwoFactorAsync(int userId);
}
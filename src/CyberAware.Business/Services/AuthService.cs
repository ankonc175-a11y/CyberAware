using CyberAware.Business.DTOs;
using CyberAware.DataAccess.Entities;
using CyberAware.DataAccess.Repositories;

namespace CyberAware.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly ITwoFactorService _twoFactorService;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService,
        ITwoFactorService twoFactorService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _twoFactorService = twoFactorService;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
            return (false, "Email already registered.");

        var learnerRole = await _roleRepository.GetByNameAsync("Learner");
        if (learnerRole == null)
            return (false, "Learner role not found. Seed the Roles table.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            RoleID = learnerRole.RoleID,
            IsActive = true,
            IsTwoFactorEnabled = false,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        return (true, "Registration successful.");
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
            return new LoginResponse { Success = false, Message = "Invalid credentials." };

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return new LoginResponse { Success = false, Message = "Invalid credentials." };

        if (user.IsTwoFactorEnabled)
        {
            return new LoginResponse
            {
                Success = true,
                RequiresTwoFactor = true,
                Message = "2FA code required."
            };
        }

        return new LoginResponse
        {
            Success = true,
            Token = _tokenService.GenerateToken(user),
            Message = "Login successful."
        };
    }

    public async Task<LoginResponse> VerifyTwoFactorAsync(Verify2FARequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !user.IsTwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
            return new LoginResponse { Success = false, Message = "2FA not enabled for this account." };

        if (!_twoFactorService.VerifyCode(user.TwoFactorSecret, request.Code))
            return new LoginResponse { Success = false, Message = "Invalid 2FA code." };

        return new LoginResponse
        {
            Success = true,
            Token = _tokenService.GenerateToken(user),
            Message = "2FA verified. Login successful."
        };
    }

    public async Task<Setup2FAResponse?> SetupTwoFactorAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        var secret = _twoFactorService.GenerateSecret();
        user.TwoFactorSecret = secret;
        user.IsTwoFactorEnabled = true;
        await _userRepository.UpdateAsync(user);

        return new Setup2FAResponse
        {
            Secret = secret,
            QrCodeUri = _twoFactorService.GenerateQrCodeUri(secret, user.Email)
        };
    }
}
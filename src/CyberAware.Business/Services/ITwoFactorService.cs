namespace CyberAware.Business.Services;

public interface ITwoFactorService
{
    string GenerateSecret();
    string GenerateQrCodeUri(string secret, string email);
    bool VerifyCode(string secret, string code);
}
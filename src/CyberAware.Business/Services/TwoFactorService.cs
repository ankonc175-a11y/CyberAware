using OtpNet;

namespace CyberAware.Business.Services;

public class TwoFactorService : ITwoFactorService
{
    public string GenerateSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string secret, string email)
    {
        return $"otpauth://totp/CyberAware:{email}?secret={secret}&issuer=CyberAware";
    }

    public bool VerifyCode(string secret, string code)
    {
        try
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
        }
        catch
        {
            return false;
        }
    }
}
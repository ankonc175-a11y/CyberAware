namespace CyberAware.Business.DTOs;

public class Verify2FARequest
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
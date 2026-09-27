namespace CyberAware.Business.DTOs;

public class Setup2FAResponse
{
    public string Secret { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
}
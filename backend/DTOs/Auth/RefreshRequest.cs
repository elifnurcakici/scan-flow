namespace backend.DTOs.Auth;

public class RefreshRequest
{
    public string Email { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

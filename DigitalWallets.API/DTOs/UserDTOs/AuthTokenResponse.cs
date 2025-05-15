namespace DigitalWallets.API.DTOs.UserDTOs;

public class AuthTokenResponse : AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
}

﻿namespace DigitalWallets.API.DTOs.UserDTOs;

public class RefreshTokenRequest
{
    public string Email { get; set; }
    public string RefreshToken { get; set; }
}

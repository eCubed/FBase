﻿namespace FBase.Api.Server.Models;

public class RefreshTokenRequest
{
    public string Token { get; set; } = "";
    public string RefreshToken { get; set; } = "";
}

using System;
using System.Collections.Generic;

namespace FBase.ApiServer
{
    public interface ITokenResponse
    {
        string Username { get; set; }
        string Token { get; set; }
        public string RefreshToken { get; set; }
        List<string> Roles { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

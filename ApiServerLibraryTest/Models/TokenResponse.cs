using FBase.ApiServer;
using System;
using System.Collections.Generic;

namespace ApiServerLibraryTest.Models
{
    public class TokenResponse : ITokenResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

using FBase.ApiServer;
using System;

namespace ApiServerLibraryTest.Data
{
    public class RefreshToken : IRefreshToken<int>
    {
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int UserId { get; set; }
        public TestUser User { get; set; }
        public long Id { get; set; }
    }
}

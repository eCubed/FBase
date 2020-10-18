using FBase.ApiServer;

namespace ApiServerLibraryTest.Models
{
    public class AuthenticatedInfo : IAuthenticatedInfo<int>
    {
        public int RequestorId { get; set; }
        public string RequestorName { get; set; }
    }
}

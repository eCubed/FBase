using FBase.ApiServer.OAuth;

namespace ApiServerLibraryTest.Data
{
    public class AppAuthorization : IAppAuthorization<int>
    {
        public long AppId { get; set; }
        public App? App { get; set; }
        public int UserId { get; set; }
        public TestUser? User { get; set; }

        public long Id { get; set; }
    }
}

using FBase.ApiServer.OAuth;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiServerLibraryTest.Data
{
    public class AuthorizationCode : IAuthorizationCode<int>
    {
        public string? Code { get; set; }
        public int UserId { get; set; }
        public TestUser? User { get; set; }
        public long AppId { get; set; }
        public App? App { get; set; }
        public string? CodeChallenge { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long Id { get; set; }
    }
}

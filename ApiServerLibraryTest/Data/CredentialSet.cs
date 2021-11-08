using FBase.ApiServer.OAuth;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiServerLibraryTest.Data
{
    public class CredentialSet : ICredentialSet
    {
        public long AppId { get; set; }
        public App App { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long Id { get; set; }
    }
}

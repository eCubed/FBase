using FBase.ApiServer.OAuth;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiServerLibraryTest.Data
{
    public class App : IApp<int>
    {
        public int UserId { get; set; }
        public TestUser? User { get; set; }
        public string? Name { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long Id { get; set; }
    }
}

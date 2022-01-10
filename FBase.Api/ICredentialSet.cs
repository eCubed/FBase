using FBase.Foundations;
using System;

namespace FBase.Api
{
    public interface ICredentialSet : IIdentifiable<long>
    {
        long AppId { get; set; }
        string Name { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string RedirectUrl { get; set; }
        DateTime? CreatedDate { get; set; }
    }
}

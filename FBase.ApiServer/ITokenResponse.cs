using System.Collections.Generic;

namespace FBase.ApiServer
{
    public interface ITokenResponse
    {
        string Username { get; set; }
        string Token { get; set; }
        List<string> Roles { get; set; }

        void SetAdditionalProperties(List<KeyValuePair<string, string>> valuePairs);
    }
}

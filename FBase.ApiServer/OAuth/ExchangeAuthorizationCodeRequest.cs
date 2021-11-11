namespace FBase.ApiServer.OAuth
{
    public class ExchangeAuthorizationCodeRequest
    {
        public string ClientId { get; set; }
        public string AuthorizationCode { get; set; }
        public string CodeVerifier { get; set; }
    }
}

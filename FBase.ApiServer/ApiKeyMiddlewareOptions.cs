namespace FBase.ApiServer
{
    public class ApiKeyMiddlewareOptions
    {
        public string ClientIdentifierKey { get; set; }
        public string ApiKeyHeaderKey { get; set; }
        public string HashHeaderKey { get; set; }
        public string DataHeaderKey { get; set; }

        public ApiKeyMiddlewareOptions()
        {
            ClientIdentifierKey = "ClientId";
            ApiKeyHeaderKey = "x-ApiKey";
            HashHeaderKey = "x-Hash";
            DataHeaderKey = "x-Data";
        }
    }
}

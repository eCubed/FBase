using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FBase.ApiClient
{
    public class TypedHttpResponseMessage<TResponseType, TErrorType>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; }

        public TResponseType Body { get; set; }
        public TErrorType Error { get; set; }

        public TypedHttpResponseMessage()
        {
        }

        public async Task SetAsync(HttpResponseMessage httpResponseMessage)
        {

            StatusCode = httpResponseMessage.StatusCode;
            IsSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;

            if (IsSuccessStatusCode)
                Body = await httpResponseMessage.ContentAsTypeAsync<TResponseType>();
            else
                Error = await httpResponseMessage.ContentAsTypeAsync<TErrorType>();

        }
    }
}

namespace NetCore6WebApi.Providers
{
    public interface IEmailer
    {
        public void Send(string from, string to, string title, string body);
    }
}

namespace NetCore6WebApi
{
    public class EmailCredentials
    {
        public string SenderEmail { get; set; } = "";
        public string CredentialUserName { get; set; } = "";
        public string CredentialPassword { get; set; } = "";
        public string SmtpClientHost { get; set; } = "";
        public int SmtpClientPort { get; set; } = 587;
    }
}

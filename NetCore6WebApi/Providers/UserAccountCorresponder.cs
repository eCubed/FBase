using FBase.Api.Server.Providers;
using NetCore6WebApi.Data;

namespace NetCore6WebApi.Providers
{
    public class UserAccountCorresponder : IUserAccountCorresponder<TestingUser, string>
    {
        private IEmailer Emailer { get; set; }
        private TestingConfig Config { get; set; }

        public UserAccountCorresponder(TestingConfig config, IEmailer emailer)
        {
            Emailer = emailer;
            Config = config;
        }

        public void SendAccountLockedOutCorrespondence(TestingUser user)
        {
            throw new NotImplementedException();
        }

        public void SendAccountReinstatedCorrespondence(TestingUser user)
        {
            throw new NotImplementedException();
        }

        public void SendEmailConfirmedCorrespondence(TestingUser user)
        {
            
        }

        public void SendPasswordResetRequestCorrespondence(TestingUser user, string passwordResetToken, string encryptedEmail)
        {
            string href = $"https://localhost:4300/password/reset/{passwordResetToken}/{encryptedEmail}";
            string passwordResetMessageBody = $"You requested to change your password because you forgot. <a href=\"{href}\">Here</a>";

            Emailer.Send(from: Config.EmailCredentials.SenderEmail, to: user.Email, title: "Password Reset Request", body: passwordResetMessageBody);

        }

        public void SendSuccessfulRegistrationCorrespondence(TestingUser user, string emailConfirmationToken, string encryptedEmail)
        {
            string href = $"http://localhost:3000/email/confirm/{emailConfirmationToken}/{encryptedEmail}";
            string emailConfirmationMessageBody = $"You'll need to verify your email address before logging in. <a href=\"{href}\">Here</a>";

            Emailer.Send(Config.EmailCredentials.SenderEmail, user.Email, "Testing Net 6 Email Confirmation", emailConfirmationMessageBody);
        }
    }
}

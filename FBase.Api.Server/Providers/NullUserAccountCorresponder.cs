using Microsoft.AspNetCore.Identity;

namespace FBase.Api.Server.Providers
{
    public class NullUserAccountCorresponder<TUser, TUserKey> : IUserAccountCorresponder<TUser, TUserKey>
         where TUser : IdentityUser<TUserKey>
    where TUserKey : IEquatable<TUserKey>
    {
        public void SendAccountLockedOutCorrespondence(TUser user)
        {
        }

        public void SendAccountReinstatedCorrespondence(TUser user)
        {
        }

        public void SendEmailConfirmedCorrespondence(TUser user)
        {
        }

        public void SendPasswordResetRequestCorrespondence(TUser user, string passwordResetToken, string encryptedEmail)
        {
        }

        public void SendSuccessfulRegistrationCorrespondence(TUser user, string emailConfirmationToken, string encryptedEmail)
        {
        }
    }
}

using Microsoft.AspNetCore.Identity;

namespace FBase.Api.Server;

public interface IUserAccountCorresponder<TUser, TUserKey>
    where TUser : IdentityUser<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    void SendSuccessfulRegistrationCorrespondence(TUser user, string emailConfirmationToken, string encryptedEmail);
    void SendPasswordResetRequestCorrespondence(TUser user, string passwordResetToken, string encryptedEmail);

    void SendEmailConfirmedCorrespondence(TUser user);

    void SendAccountLockedOutCorrespondence(TUser user);
    void SendAccountReinstatedCorrespondence(TUser user);

}

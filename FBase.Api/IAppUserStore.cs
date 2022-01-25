using FBase.Foundations;

namespace FBase.Api;

public interface IAppUserStore<TUser, TUserKey> : IAsyncStore<TUser, TUserKey>
    where TUser : class, IAppUser<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    IQueryable<TUser> GetQueryableUsers();
    IQueryable<TUser> GetQueryableUsers(string searchText);
}

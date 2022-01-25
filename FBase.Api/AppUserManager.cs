using FBase.Foundations;

namespace FBase.Api;

public class AppUserManager<TUser, TUserKey> : ManagerBase<TUser, TUserKey>
    where TUser : class, IAppUser<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    public AppUserManager(IAppUserStore<TUser, TUserKey> store) : base(store)
    {
    }

    protected IAppUserStore<TUser, TUserKey> GetUserStore() => (IAppUserStore<TUser, TUserKey>)Store;

    public ResultSet<TModel> SearchUsers<TModel>(string searchText, Func<TUser, TModel> projector, int page = 1, int pageSize = 10)
        where TModel : class
    {
        var qUsers = GetUserStore().GetQueryableUsers();
        if (!string.IsNullOrEmpty(searchText))
        {
            qUsers = qUsers.Where(u => u.UserName.Contains(searchText));
        }

        qUsers = qUsers.OrderBy(u => u.UserName);

        var usersRes = ResultSetHelper.GetResults<TUser, int>(qUsers, page, pageSize);
        return ResultSetHelper.Convert(usersRes, projector);
    }

}

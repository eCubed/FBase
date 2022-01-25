using FBase.Api;
using FBase.DotNetCore.EntityFramework;

namespace NetCore6WebApi.Data
{
    public class TestingAppUserStore : EntityStoreBase<TestingUser, string>, IAppUserStore<TestingUser, string>
    {
        public TestingAppUserStore(TestingDbContext context) : base(context)
        {
        }

        public IQueryable<TestingUser> GetQueryableUsers()
        {
            return db.Set<TestingUser>().AsQueryable();
        }

        public IQueryable<TestingUser> GetQueryableUsers(string searchText)
        {
            return db.Set<TestingUser>().Where(u => u.UserName.ToLower().Contains(searchText.ToLower()) ||
                u.Email.ToLower().Contains(searchText.ToLower())).AsQueryable();
        }
    }
}

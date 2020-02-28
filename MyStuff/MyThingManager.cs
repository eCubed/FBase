using FBase.Foundations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyStuff
{
    public class MyThingManager<TMyThing> : ManagerBase<TMyThing, int>
        where TMyThing : class, IMyThing, new()
    {
        public MyThingManager(IMyThingStore<TMyThing> store) : base(store)
        {
        }

        private IMyThingStore<TMyThing> GetMyThingStore()
        {
            return (IMyThingStore<TMyThing>)Store;
        }

        public async Task<TMyThing> FindByUniqueAsync(TMyThing someThing)
        {
            return await GetMyThingStore().FindByNameAsync(someThing.Name);
        }

        public async Task<ManagerResult> CreateAsync(string name, string description)
        {
            TMyThing myThing = new TMyThing();
            myThing.Description = description;
            myThing.Name = name;

            return await DataUtils.CreateAsync(
                entity: myThing,
                store: GetMyThingStore(),
                findUniqueAsync: FindByUniqueAsync);
        }

        public async Task<ManagerResult> UpdateAsync(TMyThing myThing)
        {
            return await DataUtils.UpdateAsync(
                id: myThing.Id,
                store: GetMyThingStore(),
                findUniqueAsync: FindByUniqueAsync,
                fillNewValues: (existingMyThing) =>
                {
                    existingMyThing.Name = myThing.Name;
                    existingMyThing.Description = myThing.Description;
                });
        }

        public async Task<ManagerResult> DeleteAsync(int id)
        {
            return await DataUtils.DeleteAsync(
                id: id,
                store: GetMyThingStore());
        }

        public ResultSet<TViewModel> GetMyThings<TViewModel>(int page = 1, int pageSize = 12)
            where TViewModel : class, IViewModel<TMyThing, int>, new()
        {
            return DataUtils.GetMany<TMyThing, int, TViewModel>(
                filteredQueryable: GetMyThingStore().GetQueryableMyThings(),
                page: page,
                pageSize: pageSize);
        }
    }
}

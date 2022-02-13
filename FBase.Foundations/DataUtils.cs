namespace FBase.Foundations;

public static class DataUtils
{

    #region Create
    public static async Task<ManagerResult> CreateAsync<T, TKey>(T entity, IAsyncStore<T, TKey> store,
        Func<T, Task<T?>>? findUniqueAsync = null, Func<T, ManagerResult>? canCreate = null)
        where T : class, IIdentifiable<TKey>
    {
        if (findUniqueAsync != null)
        {
            T? duplicate = await findUniqueAsync.Invoke(entity);

            if (duplicate != null)
                return new ManagerResult(ManagerErrors.DuplicateOnCreate);
        }

        if (canCreate != null)
        {
            var createRes = canCreate.Invoke(entity);

            if (!createRes.Success)
                return createRes;
        }

        await store.CreateAsync(entity);

        return new ManagerResult();
    }

    public static async Task<ManagerResult<TKey>> CreateAsync<T, TKey, TModel>(TModel model, IAsyncStore<T, TKey> store,
        Func<T, Task<T?>>? findUniqueAsync = null, Func<T, ManagerResult>? canCreate = null, Action<T>? setNonModelData = null)
        where T : class, IIdentifiable<TKey>, new()
        where TModel : class, ISaveModel<T>
    {
        T newData = new();
        model.SetObjectValues(newData);

        if (setNonModelData != null)
            setNonModelData.Invoke(newData);

        var createRes = await CreateAsync(newData, store, findUniqueAsync, canCreate);

        if (!createRes.Success)
            return new ManagerResult<TKey>(createRes.Errors);

        return new ManagerResult<TKey>(newData.Id);
    }

    #endregion Create


    #region Delete
    public static async Task<ManagerResult> DeleteAsync<T, TKey>(TKey id, IAsyncStore<T, TKey> store,
        Func<T, ManagerResult>? canDelete = null)
         where T : class, IIdentifiable<TKey>
    {
        try
        {
            T? found = await store.FindByIdAsync(id);

            if (found == null)
                return new ManagerResult(ManagerErrors.RecordNotFound);

            if (canDelete != null)
            {
                var canDeleteRes = canDelete.Invoke(found);

                if (!canDeleteRes.Success)
                    return canDeleteRes;
            }

            await store.DeleteAsync(found);
        }
        catch (Exception e)
        {
            return e.CreateManagerResult();
        }

        return new ManagerResult();
    }

    #endregion Delete

    #region Update
    public static async Task<ManagerResult> UpdateAsync<T, TKey>(TKey id, IAsyncStore<T, TKey> store,
        Func<T, Task<T?>>? findUniqueAsync = null, Func<T, ManagerResult>? canUpdate = null,
        Action<T>? fillNewValues = null)
        where T : class, IIdentifiable<TKey>
    {
        T? recordToUpdate = await store.FindByIdAsync(id);

        if (recordToUpdate == null)
            return new ManagerResult(ManagerErrors.RecordNotFound);

        if (findUniqueAsync != null)
        {
            T? possibleDuplicate = await findUniqueAsync(recordToUpdate);
            /*
            if ((possibleDuplicate != null) && (!possibleDuplicate.Id.Equals(recordToUpdate.Id)))
                return new ManagerResult(ManagerErrors.DuplicateOnUpdate);
            */

            if (!possibleDuplicate?.Id?.Equals(recordToUpdate.Id) ?? false)
                return new ManagerResult(ManagerErrors.DuplicateOnUpdate);
            
        }

        if (canUpdate != null)
        {
            var canUpdateRes = canUpdate.Invoke(recordToUpdate);

            if (!canUpdateRes.Success)
                return canUpdateRes;
        }

        // Now, we allow updating of the values!
        if (fillNewValues != null)
            fillNewValues.Invoke(recordToUpdate);

        await store.UpdateAsync(recordToUpdate);

        return new ManagerResult();
    }

    public static async Task<ManagerResult> UpdateAsync<T, TKey, TModel>(TKey id, TModel model, IAsyncStore<T, TKey> store,
        Func<T, Task<T?>>? findUniqueAsync = null, Func<T, ManagerResult>? canUpdate = null, Action<T>? setNonModelData = null)
        where T : class, IIdentifiable<TKey>, new()
        where TModel : class, ISaveModel<T>
    {
        return await DataUtils.UpdateAsync(id, store, findUniqueAsync, canUpdate,
            fillNewValues: data =>
            {
                model.SetObjectValues(data);
                setNonModelData?.Invoke(data);
            });
    }

    #endregion Update

    #region Get
    public static async Task<ManagerResult<TModel>> GetOneRecordAsync<T, TKey, TModel>(TKey id, IAsyncStore<T, TKey> store,
        Func<T, ManagerResult>? canGet = null)
        where T : class, IIdentifiable<TKey>, new()
        where TModel : class, IDisplayModel<T>, new()
    {
        T? data = await store.FindByIdAsync(id);

        if (data == null)
            return new ManagerResult<TModel>(ManagerErrors.RecordNotFound);

        if (canGet != null)
        {
            var canGetResult = canGet.Invoke(data);
            if (!canGetResult.Success)
                return new ManagerResult<TModel>(canGetResult.Errors);
        }

        TModel model = new TModel();
        model.FillModel(data);

        return new ManagerResult<TModel>(model);
    }

    public static ResultSet<TModel> GetMany<T, TKey, TModel>(IQueryable<T> filteredQueryable, int page = 1, int pageSize = 10)
       where T : class, IIdentifiable<TKey>
       where TModel : class, IDisplayModel<T>, new()
    {
        return ResultSetHelper.Convert(ResultSetHelper.GetResults<T, TKey>(filteredQueryable, page, pageSize), data =>
        {
            TModel model = new TModel();
            model.FillModel(data);
            return model;
        });
    }

    #endregion Get
}

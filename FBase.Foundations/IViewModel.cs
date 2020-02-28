namespace FBase.Foundations
{
    public interface IViewModel<T, TKey>
       where T : class, IIdentifiable<TKey>
    {
        TKey Id { get; set; }

        void UpdateObject(T data);
        void FillViewModel(T data);
    }
}

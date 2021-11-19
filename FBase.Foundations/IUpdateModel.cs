namespace FBase.Foundations
{
    public interface IUpdateModel<T> : IDisplayModel<T>, ISaveModel<T>
       where T : class
    {
    }
}

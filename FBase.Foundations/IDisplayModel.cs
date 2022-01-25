namespace FBase.Foundations;

public interface IDisplayModel<T>
   where T : class
{
    void FillModel(T data);
}

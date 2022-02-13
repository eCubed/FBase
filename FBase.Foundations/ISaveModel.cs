namespace FBase.Foundations;

public interface ISaveModel<T>
    where T : class
{
    void SetObjectValues(T data);
}

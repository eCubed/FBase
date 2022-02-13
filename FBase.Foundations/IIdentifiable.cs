namespace FBase.Foundations;

public interface IIdentifiable<out TKey>
{
    TKey Id { get; }
}

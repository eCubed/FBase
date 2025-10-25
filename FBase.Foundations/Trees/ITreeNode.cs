namespace FBase.Foundations.Trees;
public interface ITreeNode<TKey>
  where TKey : struct
{
  TKey Id { get; }
  List<ITreeNode<TKey>> Children { get; } 
}

using FBase.Foundations.Trees;

namespace FBaseTests.Trees;
public sealed class Node<TKey> : ITreeNode<TKey> where TKey : struct
{
  public TKey Id { get; }
  public string? Name { get; }
  public List<ITreeNode<TKey>> Children { get; } = [];

  public Node(TKey id, string? name = null)
  {
    Id = id;
    Name = name;
  }

  public Node<TKey> Add(params Node<TKey>[] children)
  {
    Children.AddRange(children);
    return this;
  }

  public override string ToString() => Name ?? Id.ToString();
}
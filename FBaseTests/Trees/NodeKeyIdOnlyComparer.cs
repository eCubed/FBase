namespace FBaseTests.Trees;
public sealed class NodeKeyIdOnlyComparer : IEqualityComparer<NodeKey>
{
  public bool Equals(NodeKey x, NodeKey y) => x.Id == y.Id;
  public int GetHashCode(NodeKey obj) => obj.Id.GetHashCode();
}

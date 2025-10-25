namespace FBaseTests.Trees;
public readonly struct NodeKey
{
  public int Id { get; }
  public int Version { get; }

  public NodeKey(int id, int version)
  {
    Id = id;
    Version = version;
  }

  public override string ToString() => $"({Id}, v{Version})";
}
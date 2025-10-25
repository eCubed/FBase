namespace FBaseTests.Trees;
public class TreePathsTests
{
  /// <summary>
  /// Builds the reference tree (Ids shown):
  ///
  ///            1
  ///        /   |   \
  ///       2    3    4
  ///     /  \        |
  ///    5    6       7
  ///                  \
  ///                   8
  ///
  /// Pre-order node order: 1,2,5,6,3,4,7,8
  /// Expected paths in pre-order:
  ///  [1]
  ///  [1,2]
  ///  [1,2,5]
  ///  [1,2,6]
  ///  [1,3]
  ///  [1,4]
  ///  [1,4,7]
  ///  [1,4,7,8]
  /// </summary>
  private static Node<int> BuildSample()
  {
    var n1 = new Node<int>(1);
    var n2 = new Node<int>(2);
    var n3 = new Node<int>(3);
    var n4 = new Node<int>(4);
    var n5 = new Node<int>(5);
    var n6 = new Node<int>(6);
    var n7 = new Node<int>(7);
    var n8 = new Node<int>(8);

    n1.Add(n2, n3, n4);
    n2.Add(n5, n6);
    n4.Add(n7);
    n7.Add(n8);

    return n1;
  }

  private static string RenderPath(IEnumerable<Node<int>> path) =>
      string.Join(".", path.Select(p => p.Id));

  [Fact]
  public void PathsDepthFirst_ReturnsAllRootToNodePaths_InPreOrder()
  {
    var root = BuildSample();

    var paths = root.PathsDepthFirst<Node<int>, int>();
    var rendered = paths.Select(RenderPath).ToArray();

    Assert.Equal(new[]
    {
            "1",
            "1.2",
            "1.2.5",
            "1.2.6",
            "1.3",
            "1.4",
            "1.4.7",
            "1.4.7.8"
        }, rendered);
  }

  [Fact]
  public void PathsDepthFirst_PrunesSubtree_WhenDescendPredicateBlocks()
  {
    var root = BuildSample();

    // Do not descend into node 4 (so 7 and 8 never appear)
    var paths = root.PathsDepthFirst<Node<int>, int>(descendPredicate: n => n.Id != 4);
    var rendered = paths.Select(RenderPath).ToArray();

    Assert.Equal(new[]
    {
            "1",
            "1.2",
            "1.2.5",
            "1.2.6",
            "1.3",
            "1.4" // visited, but not descended into
        }, rendered);
  }

  [Fact]
  public void PathsDepthFirstLazy_Matches_EagerVersion()
  {
    var root = BuildSample();

    var eager = root.PathsDepthFirst<Node<int>, int>()
                    .Select(p => p.Select(x => x.Id).ToArray())
                    .ToArray();

    var lazy = root.PathsDepthFirstLazy<Node<int>, int>()
                   .Select(p => p.Select(x => x.Id).ToArray())
                   .ToArray();

    Assert.Equal(eager.Length, lazy.Length);
    for (int i = 0; i < eager.Length; i++)
      Assert.Equal(eager[i], lazy[i]);
  }

  [Fact]
  public void PathsDepthFirst_ReturnsIndependentSnapshots_PerPath()
  {
    var root = BuildSample();

    var paths = root.PathsDepthFirst<Node<int>, int>();

    // Ensure each inner list is a distinct list object (no shared references).
    Assert.True(paths.Count > 3);
    Assert.NotSame(paths[0], paths[1]);
    Assert.NotSame(paths[1], paths[2]);

    // Mutate one returned path list and ensure others unaffected.
    var copyBefore = paths.Select(p => p.Select(n => n.Id).ToArray()).ToArray();

    paths[2].Clear(); // modify a single snapshot

    // Others should remain the same (only index 2 changes)
    for (int i = 0; i < paths.Count; i++)
    {
      var current = paths[i].Select(n => n.Id).ToArray();
      if (i == 2)
        Assert.Empty(current);
      else
        Assert.Equal(copyBefore[i], current);
    }
  }

  [Fact]
  public void PathsDepthFirst_NullRoot_ReturnsEmpty()
  {
    Node<int>? root = null;

    var eager = root.PathsDepthFirst<Node<int>, int>();
    var lazy = root.PathsDepthFirstLazy<Node<int>, int>();

    Assert.Empty(eager);
    Assert.Empty(lazy);
  }

  [Fact]
  public void PathsDepthFirst_SingleNode_ReturnsSingleLength1Path()
  {
    var root = new Node<int>(42);

    var eager = root.PathsDepthFirst<Node<int>, int>();
    var lazy = root.PathsDepthFirstLazy<Node<int>, int>();

    Assert.Single(eager);
    Assert.Single(lazy);

    Assert.Equal(new[] { 42 }, eager.Single().Select(n => n.Id));
    Assert.Equal(new[] { 42 }, lazy.Single().Select(n => n.Id));
  }
}

using FBase.Foundations.Trees;

namespace FBaseTests.Trees;
public class TreeFindByIdTests
{
  /// <summary>
  /// Int-keyed sample:
  ///
  ///            1
  ///        /   |   \
  ///       2    3    4
  ///     /  \        |
  ///    5    6       7
  ///                  \
  ///                   8
  /// </summary>
  private static Node<int> BuildIntSample()
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

  [Fact]
  public void DFS_FindsExistingId_FirstPreorderMatch()
  {
    var root = BuildIntSample();
    var found = root.FindByIdDepthFirst<Node<int>, int>(7);

    Assert.NotNull(found);
    Assert.Equal(7, found!.Id);
  }

  [Fact]
  public void BFS_FindsExistingId_ShallowestMatch()
  {
    var root = BuildIntSample();
    var found = root.FindByIdBreadthFirst<Node<int>, int>(7);

    Assert.NotNull(found);
    Assert.Equal(7, found!.Id);
  }

  [Fact]
  public void Find_ReturnsNull_WhenNotFound()
  {
    var root = BuildIntSample();
    Assert.Null(root.FindByIdDepthFirst<Node<int>, int>(999));
    Assert.Null(root.FindByIdBreadthFirst<Node<int>, int>(999));
  }

  [Fact]
  public void TryFind_Works_ForFoundAndNotFound()
  {
    var root = BuildIntSample();

    Assert.True(root.TryFindByIdDepthFirst<Node<int>, int>(8, out var dfsHit));
    Assert.NotNull(dfsHit);
    Assert.Equal(8, dfsHit!.Id);

    Assert.False(root.TryFindByIdBreadthFirst<Node<int>, int>(999, out var bfsMiss));
    Assert.Null(bfsMiss);
  }

  [Fact]
  public void Pruning_PreventsReachingTarget_SubtreeSkipped()
  {
    var root = BuildIntSample();

    // Skip exploring children under node 4 -> cannot reach 7 or 8.
    var found = root.FindByIdDepthFirst<Node<int>, int>(
        id: 8,
        descendPredicate: n => n.Id != 4);

    Assert.Null(found);
  }

  [Fact]
  public void DFS_Vs_BFS_WithDuplicateIds_ReturnsDifferentFirsts()
  {
    // Build a shape where DFS hits a deeper left duplicate first,
    // but BFS hits the shallower right duplicate first.

    //      1
    //    /   \
    //   2     3
    //  /       \
    // 5         5   <-- duplicate IDs (left is deeper)
    var root = new Node<int>(1);
    var n2 = new Node<int>(2);
    var n3 = new Node<int>(3);
    var n5_deepLeft = new Node<int>(5, "deep-left");
    var n5_shallowRight = new Node<int>(5, "shallow-right");

    root.Add(n2, n3);
    n2.Add(n5_deepLeft);          // depth 2 (root->2->5)
    n3.Add(n5_shallowRight);      // depth 2 as well… make left deeper by adding one more level:
                                  // Adjust to make left deeper
    n2.Children.Clear();
    var mid = new Node<int>(42, "mid");
    n2.Add(mid);
    mid.Add(n5_deepLeft);         // now left is depth 3; right is depth 2

    // DFS (preorder) traverses left subtree fully first -> returns deep-left
    var dfsFound = root.FindByIdDepthFirst<Node<int>, int>(5);
    Assert.NotNull(dfsFound);
    Assert.Equal("deep-left", ((Node<int>)dfsFound!).Name);

    // BFS finds the shallower match first -> returns shallow-right
    var bfsFound = root.FindByIdBreadthFirst<Node<int>, int>(5);
    Assert.NotNull(bfsFound);
    Assert.Equal("shallow-right", ((Node<int>)bfsFound!).Name);
  }

  [Fact]
  public void DefaultComparer_WorksWithGuidKeys()
  {
    // Build a small Guid-keyed tree; ensure default EqualityComparer<Guid>.Default is used.
    var targetId = Guid.NewGuid();
    var root = new Node<Guid>(Guid.NewGuid())
        .Add(new Node<Guid>(Guid.NewGuid()),
             new Node<Guid>(Guid.NewGuid())
                 .Add(new Node<Guid>(targetId)));

    var hit = root.FindByIdDepthFirst<Node<Guid>, Guid>(targetId);
    Assert.NotNull(hit);
    Assert.Equal(targetId, hit!.Id);
  }

  [Fact]
  public void CustomComparer_CanMatchOnSubsetOfCompoundStructKey()
  {
    // Tree with NodeKey(Id, Version)
    // We search for (Id=10, Version=999) and want a match on Id only.
    var root = new Node<NodeKey>(new NodeKey(1, 0))
        .Add(new Node<NodeKey>(new NodeKey(10, 1), "candidate-v1"),
             new Node<NodeKey>(new NodeKey(2, 0))
                 .Add(new Node<NodeKey>(new NodeKey(10, 2), "candidate-v2")));

    var searchKey = new NodeKey(10, 999);

    // Without comparer (default struct equality compares all fields) -> no match
    var noComparerHit = root.FindByIdDepthFirst<Node<NodeKey>, NodeKey>(searchKey);
    Assert.Null(noComparerHit);

    // With custom comparer that ignores Version -> first preorder 10,* is "candidate-v1"
    var comparer = new NodeKeyIdOnlyComparer();
    var withComparerHit = root.FindByIdDepthFirst<Node<NodeKey>, NodeKey>(searchKey, comparer: comparer);

    Assert.NotNull(withComparerHit);
    Assert.Equal("candidate-v1", ((Node<NodeKey>)withComparerHit!).Name);
  }

  [Fact]
  public void NullRoot_ReturnsNull_FindAndTry()
  {
    Node<int>? root = null;

    Assert.Null(root.FindByIdDepthFirst<Node<int>, int>(1));
    Assert.Null(root.FindByIdBreadthFirst<Node<int>, int>(1));

    Assert.False(root.TryFindByIdDepthFirst<Node<int>, int>(1, out var dfs));
    Assert.Null(dfs);

    Assert.False(root.TryFindByIdBreadthFirst<Node<int>, int>(1, out var bfs));
    Assert.Null(bfs);
  }
}
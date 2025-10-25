using FBase.Foundations.Trees;

namespace FBaseTests.Trees;

public class TreeTraversalTests
{
  /// <summary>
  /// Creates this tree (Ids shown):
  /// 
  ///            1
  ///        /   |   \
  ///       2    3    4
  ///     /  \        |
  ///    5    6       7
  ///                  \
  ///                   8
  /// 
  /// Pre-Order (DFS):     1,2,5,6,3,4,7,8
  /// Post-Order (DFS):    5,6,2,3,8,7,4,1
  /// Breadth-First (BFS): 1,2,3,4,5,6,7,8
  /// </summary>
  private static SampleNode BuildSample()
  {
    var n1 = new SampleNode(1);
    var n2 = new SampleNode(2);
    var n3 = new SampleNode(3);
    var n4 = new SampleNode(4);
    var n5 = new SampleNode(5);
    var n6 = new SampleNode(6);
    var n7 = new SampleNode(7);
    var n8 = new SampleNode(8);

    n1.Add(n2, n3, n4);
    n2.Add(n5, n6);
    n4.Add(n7);
    n7.Add(n8);

    return n1;
  }

  [Fact]
  public void DepthFirst_PreOrder_ActionVisitor_WalksExpectedOrder()
  {
    var root = BuildSample();
    var visited = new List<int>();

    // visit parent before children
    root.TraverseDepthFirst<SampleNode, int>(
        visit: n => visited.Add(n.Id));

    Assert.Equal(new[] { 1, 2, 5, 6, 3, 4, 7, 8 }, visited);
  }

  [Fact]
  public void DepthFirst_PostOrder_ActionVisitor_WalksExpectedOrder()
  {
    var root = BuildSample();
    var visited = new List<int>();

    // visit after all children are processed
    root.TraverseDepthFirstPostOrder<SampleNode, int>(
        visit: n => visited.Add(n.Id));

    Assert.Equal(new[] { 5, 6, 2, 3, 8, 7, 4, 1 }, visited);
  }

  [Fact]
  public void BreadthFirst_ActionVisitor_WalksExpectedOrder()
  {
    var root = BuildSample();
    var visited = new List<int>();

    root.TraverseBreadthFirst<SampleNode, int>(
        visit: n => visited.Add(n.Id));

    Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, visited);
  }

  [Fact]
  public void DepthFirst_IEnumerable_YieldsExpectedOrder()
  {
    var root = BuildSample();

    var order = root
        .DepthFirst<SampleNode, int>()  // pre-order
        .Select(n => n.Id)
        .ToArray();

    Assert.Equal(new[] { 1, 2, 5, 6, 3, 4, 7, 8 }, order);
  }

  [Fact]
  public void BreadthFirst_IEnumerable_YieldsExpectedOrder()
  {
    var root = BuildSample();

    var order = root
        .BreadthFirst<SampleNode, int>()
        .Select(n => n.Id)
        .ToArray();

    Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, order);
  }

  [Fact]
  public void Pruning_SkipsChildrenButStillVisitsNode()
  {
    var root = BuildSample();
    var visited = new List<int>();

    // Skip exploring children of node 4 (but still visit 4 itself).
    root.TraverseBreadthFirst<SampleNode, int>(
        visit: n => visited.Add(n.Id),
        descendPredicate: n => n.Id != 4);

    // Expect 7 and 8 (children of 4) to be missing.
    Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, visited);
  }

  [Fact]
  public void Pruning_CanSkipLargeSubtree_DepthFirst()
  {
    var root = BuildSample();
    var visited = new List<int>();

    // Skip exploring children under node 2 (but still visit 2)
    root.TraverseDepthFirst<SampleNode, int>(
        visit: n => visited.Add(n.Id),
        descendPredicate: n => n.Id != 2);

    Assert.Equal(new[] { 1, 2, 3, 4, 7, 8 }, visited);
  }

  [Fact]
  public void NullRoot_ActionBased_ThrowsArgumentNullException()
  {
    SampleNode? root = null;

    // Extension methods can be called on null receivers; inside we guard.
    Assert.Throws<ArgumentNullException>(() =>
        TreeTraversalExtensions.TraverseDepthFirst<SampleNode, int>(
            root!, n => { }));
  }

  [Fact]
  public void NullRoot_IEnumerableBased_YieldsEmpty()
  {
    SampleNode? root = null;

    // IEnumerable version should safely yield no elements.
    var ids = root.DepthFirst<SampleNode, int>()
                  .Select(n => n.Id)
                  .ToArray();

    Assert.Empty(ids);
  }

  [Fact]
  public void ChildrenOrder_IsPreserved_ForDepthFirstPreOrder()
  {
    // Ensure that pushing children in reverse results in natural left-to-right processing
    // i.e., first child visited before later siblings.
    var root = new SampleNode(10)
        .Add(new SampleNode(11), new SampleNode(12), new SampleNode(13));

    var order = root.DepthFirst<SampleNode, int>().Select(n => n.Id).ToArray();

    Assert.Equal(new[] { 10, 11, 12, 13 }, order);
  }

  [Fact]
  public void DescendPredicate_CanImplement_VisitedSet_ForGraphSafety()
  {
    // a -> b -> c -> b (cycle)
    var a = new SampleNode(1);
    var b = new SampleNode(2);
    var c = new SampleNode(3);
    a.Add(b);
    b.Add(c);
    c.Add(b); // not a tree, intentionally cyclic for test

    var visitedIds = new List<int>();
    var seenForOutput = new HashSet<int>();   // controls what we record as "visited" (dedupe visit output)
    var seenForDescent = new HashSet<int>();  // controls whether we descend (prevents infinite loops)

    a.TraverseDepthFirst<SampleNode, int>(
        visit: n =>
        {
          // Pre-order calls visit before descendPredicate.
          // Only record the first time we see a node.
          if (seenForOutput.Add(n.Id))
            visitedIds.Add(n.Id);
        },
        descendPredicate: n =>
        {
          // Only descend the first time we pop a node.
          // Subsequent pops of the same node won't re-enqueue children.
          return seenForDescent.Add(n.Id);
        });

    Assert.Equal(new[] { 1, 2, 3 }, visitedIds);
  }
}
namespace FBaseTests.Trees;

public class TreeFindByPredicateTests
{
  private static Node<int> BuildSample()
  {
    //            1
    //        /   |   \
    //       2    3    4
    //     /  \        |
    //    5    6       7
    //                  \
    //                   8
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
  public void WhereDepthFirst_FiltersByPredicate_Streamed()
  {
    var root = BuildSample();

    // All even IDs, DFS order
    var ids = root.WhereDepthFirst<Node<int>, int>(n => n.Id % 2 == 0)
                  .Select(n => n.Id)
                  .ToArray();

    // DFS preorder of sample is: 1,2,5,6,3,4,7,8
    Assert.Equal(new[] { 2, 6, 4, 8 }, ids);
  }

  [Fact]
  public void WhereBreadthFirst_FiltersByPredicate_Streamed()
  {
    var root = BuildSample();

    // All even IDs, BFS order
    var ids = root.WhereBreadthFirst<Node<int>, int>(n => n.Id % 2 == 0)
                  .Select(n => n.Id)
                  .ToArray();

    // BFS order of sample: 1,2,3,4,5,6,7,8
    Assert.Equal(new[] { 2, 4, 6, 8 }, ids);
  }

  [Fact]
  public void FindAllDepthFirst_PrunesSubtree()
  {
    var root = BuildSample();

    // Skip exploring children under node 4 -> excludes 7 and 8 from consideration
    var hits = root.FindAllDepthFirst<Node<int>, int>(
        predicate: n => n.Id >= 7,
        descendPredicate: n => n.Id != 4);

    Assert.Empty(hits); // 7 and 8 are under 4
  }

  [Fact]
  public void FindAllBreadthFirst_MaxCount_EarlyStops()
  {
    var root = BuildSample();

    // Collect first two nodes >= 3 in BFS order: BFS is 1,2,3,4,5,6,7,8 -> matches: 3,4,...
    var hits = root.FindAllBreadthFirst<Node<int>, int>(
        predicate: n => n.Id >= 3,
        maxCount: 2);

    Assert.Equal(new[] { 3, 4 }, hits.Select(n => n.Id).ToArray());
  }

  [Fact]
  public void NullRoot_ProducesEmptySequences()
  {
    Node<int>? root = null;

    var dfsSeq = root.WhereDepthFirst<Node<int>, int>(n => true).ToArray();
    var bfsSeq = root.WhereBreadthFirst<Node<int>, int>(n => true).ToArray();
    var dfsList = root.FindAllDepthFirst<Node<int>, int>(n => true);
    var bfsList = root.FindAllBreadthFirst<Node<int>, int>(n => true);

    Assert.Empty(dfsSeq);
    Assert.Empty(bfsSeq);
    Assert.Empty(dfsList);
    Assert.Empty(bfsList);
  }
}
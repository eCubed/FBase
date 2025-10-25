using FBase.Foundations.Trees;
using System.Collections.Generic;

namespace FBaseTests.Trees;
public class TreeTransformTests
{
  public sealed class Node2<TKey> : ITreeNode<TKey> where TKey : struct
  {
    public TKey Id { get; }
    public string Label { get; }
    public List<ITreeNode<TKey>> Children { get; } = new();

    public Node2(TKey id, string label) { Id = id; Label = label; }

    public Node2<TKey> Add(params Node2<TKey>[] children)
    {
      Children.AddRange(children);
      return this;
    }

    public override string ToString() => $"{Id}:{Label}";
  }


  private static Node<int> BuildSample()
  {
    //            1
    //        /   |   \
    //       2    3    4
    //     /  \        |
    //    5    6       7
    //                  \
    //                   8
    var n1 = new Node<int>(1, "r");
    var n2 = new Node<int>(2, "a");
    var n3 = new Node<int>(3, "c");
    var n4 = new Node<int>(4, "b");
    var n5 = new Node<int>(5, "x");
    var n6 = new Node<int>(6, "y");
    var n7 = new Node<int>(7, "m");
    var n8 = new Node<int>(8, "n");

    n1.Add(n2, n3, n4);
    n2.Add(n5, n6);
    n4.Add(n7);
    n7.Add(n8);

    return n1;
  }

  // ---------- DeepCopy ----------
  [Fact]
  public void DeepCopy_ProducesIndependentStructure()
  {
    var root = BuildSample();

    var clone = root.DeepCopy<Node<int>, int>(
        cloneFactory: n => new Node<int>(n.Id, n.Name),  // copy fields (no children yet)
        addChild: (p, c) => p.Add(c));

    // Structure equal (Ids in pre-order)
    IEnumerable<int> Pre(Node<int> r)
    {
      var stack = new Stack<Node<int>>();
      stack.Push(r);
      while (stack.Count > 0)
      {
        var x = stack.Pop();
        yield return x.Id;
        var children = x.Children.Cast<Node<int>>().ToList();
        for (int i = children.Count - 1; i >= 0; i--) stack.Push(children[i]);
      }
    }
    Assert.Equal(Pre(root), Pre(clone));

    // Different instances
    Assert.NotSame(root, clone);
    // Mutate clone only
    ((Node<int>)clone.Children[0]).Add(new Node<int>(99, "new"));
    // Original unaffected
    var a2 = (Node<int>)root.Children[0];
    Assert.DoesNotContain(a2.Children.Cast<Node<int>>(), c => c.Id == 99);
  }

  // ---------- Map ----------
  [Fact]
  public void Map_TransformsNodeType_PreservingStructure()
  {
    var root = BuildSample();

    var mapped = root.Map<Node<int>, Node2<int>, int>(
        mapFactory: n => new Node2<int>(n.Id, $"mapped:{n.Name}"),
        addChild: (p, c) => p.Add(c));

    // Check a few spots
    Assert.Equal(1, mapped.Id);
    Assert.Equal("mapped:r", mapped.Label);

    var mappedChildrenIds = mapped.Children.Cast<Node2<int>>().Select(n => n.Id).ToArray();
    Assert.Equal(new[] { 2, 3, 4 }, mappedChildrenIds);

    // Ensure structure depth preserved (1->4->7->8)
    var node4 = (Node2<int>)mapped.Children[2];
    var node7 = (Node2<int>)node4.Children.Single();
    var node8 = (Node2<int>)node7.Children.Single();
    Assert.Equal(8, node8.Id);
    Assert.StartsWith("mapped:", node8.Label);
  }

  // ---------- Filter (structure-preserving) ----------
  [Fact]
  public void Filter_KeepsMatchingNodes_AndNecessaryAncestors()
  {
    var root = BuildSample();

    // Keep even IDs. Ancestors retained to preserve paths.
    var filtered = root.Filter<Node<int>, int>(
        predicate: n => n.Id % 2 == 0,
        buildParent: n => new Node<int>(n.Id, n.Name),
        addChild: (p, c) => p.Add(c));

    // Root (1) is NOT even but should appear if some descendants are kept (2,4,6,8)
    Assert.NotNull(filtered);
    Assert.Equal(1, filtered!.Id);

    // Top-level kept children: 2 and 4 (3 removed)
    var top = filtered.Children.Cast<Node<int>>().Select(n => n.Id).ToArray();
    Assert.Equal(new[] { 2, 4 }, top);

    // Under node 2: keep 6 (5 pruned)
    var node2 = (Node<int>)filtered.Children[0];
    var under2 = node2.Children.Cast<Node<int>>().Select(n => n.Id).ToArray();
    Assert.Equal(new[] { 6 }, under2);

    // Under node 4: keep 8 (via 7 as ancestor retained)
    var node4 = (Node<int>)filtered.Children[1];
    var node7 = (Node<int>)node4.Children.Single();       // 7 kept as ancestor (not matching)
    var node8 = (Node<int>)node7.Children.Single();
    Assert.Equal(7, node7.Id);
    Assert.Equal(8, node8.Id);
  }

  [Fact]
  public void Filter_ReturnsNull_WhenNoMatchesAnywhere()
  {
    var root = BuildSample();
    // Predicate that matches nothing
    var filtered = root.Filter<Node<int>, int>(
        predicate: n => false,
        buildParent: n => new Node<int>(n.Id, n.Name),
        addChild: (p, c) => p.Add(c));

    Assert.Null(filtered);
  }

  // ---------- SortChildren (recursive) ----------
  [Fact]
  public void SortChildrenRecursive_SortsAtEveryLevel_Ascending()
  {
    // Create intentionally unsorted children by Name at multiple levels
    var root = new Node<int>(1, "root");
    var a = new Node<int>(10, "delta");
    var b = new Node<int>(11, "charlie");
    var c = new Node<int>(12, "bravo");
    var d = new Node<int>(13, "alpha");

    root.Add(a, b, c, d);            // delta, charlie, bravo, alpha
    a.Add(new Node<int>(20, "zoo"), new Node<int>(21, "yak"), new Node<int>(22, "ant"));

    // Sort everywhere by Name ascending
    root.SortChildrenRecursive<Node<int>, int, string>(n => n.Name!);

    // Level 1 sorted: alpha, bravo, charlie, delta  -> ids: 13,12,11,10
    var level1 = root.Children.Cast<Node<int>>().Select(n => n.Id).ToArray();
    Assert.Equal(new[] { 13, 12, 11, 10 }, level1);

    // Under node 'delta' (id 10): ant, yak, zoo -> ids: 22,21,20
    var nodeDelta = (Node<int>)root.Children.Last();
    var underDelta = nodeDelta.Children.Cast<Node<int>>().Select(n => n.Id).ToArray();
    Assert.Equal(new[] { 22, 21, 20 }, underDelta);
  }

  [Fact]
  public void SortChildrenRecursive_SortsDescending_WithComparer()
  {
    var root = BuildSample(); // names: r, a, c, b, x, y, m, n

    // Descending by Name using default comparer + descending flag
    root.SortChildrenRecursive<Node<int>, int, string>(
        n => n.Name!, descending: true);

    // Top-level children of root are a(2), c(3), b(4)
    // Descending by Name -> c(3), b(4), a(2)
    var ids = root.Children.Cast<Node<int>>().Select(n => n.Id).ToArray();

    Assert.Equal(new[] { 3, 4, 2 }, ids);
  }
}
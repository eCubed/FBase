namespace FBaseTests.Trees;
public class TreeDeepMergeTests
{
  [Fact]
  public void Merge_Throws_OnRootMismatch()
  {
    var a = new Node<int>(1).Add(new Node<int>(10));
    var b = new Node<int>(2).Add(new Node<int>(20));

    Assert.Throws<ArgumentException>(() => a.MergeWith<Node<int>, int>(b));
  }

  [Fact]
  public void Merge_Allows_NullSides()
  {
    Node<int>? a = null;
    var b = new Node<int>(1).Add(new Node<int>(20));
    var merged = TreeMergeExtensions.MergeWith<Node<int>, int>(a!, b);
    Assert.Same(b, merged);

    a = new Node<int>(1).Add(new Node<int>(10));
    Node<int>? bNull = null;
    merged = a.MergeWith<Node<int>, int>(bNull!);
    Assert.Same(a, merged);
  }

  [Fact]
  public void Merge_Adds_UniqueChildren_FromB()
  {
    var a = new Node<int>(1).Add(new Node<int>(10), new Node<int>(11));
    var b = new Node<int>(1).Add(new Node<int>(12), new Node<int>(13));

    var merged = a.MergeWith<Node<int>, int>(b);
    Assert.Equal(new[] { 10, 11, 12, 13 }, merged.Children.Select(c => ((Node<int>)c).Id).ToArray());
  }

  [Fact]
  public void Merge_OnKeyCollision_Recurses_AndKeeps_A_Instance()
  {
    // A: 1 -> 10(A) -> {100(A)}
    var a = new Node<int>(1);
    var a10 = new Node<int>(10, "A-10");
    a.Add(a10);
    a10.Add(new Node<int>(100, "A-100"));

    // B: 1 -> 10(B) -> {100(B), 101(B)}
    var b = new Node<int>(1);
    var b10 = new Node<int>(10, "B-10");
    b.Add(b10);
    b10.Add(new Node<int>(100, "B-100"), new Node<int>(101, "B-101"));

    var merged = a.MergeWith<Node<int>, int>(b);

    var merged10 = (Node<int>)merged.Children.Single();
    Assert.Same(a10, merged10);                 // keep A's node instance
    var under10Ids = merged10.Children.Select(c => ((Node<int>)c).Id).OrderBy(x => x).ToArray();

    // Expect union under 10: 100 (existing A child) + 101 (new from B)
    Assert.Equal(new[] { 100, 101 }, under10Ids);

    // Existing A child 100 remains A's instance/name
    var child100 = (Node<int>)merged10.Children.First(c => ((Node<int>)c).Id == 100);
    Assert.Equal("A-100", child100.Name);
  }

  [Fact]
  public void Merge_DeepCollision_Prefers_A_AtEveryLevel_But_AddsNewFromB()
  {
    // A: 1 -> 10 -> 200 -> {300(A)}
    var a = new Node<int>(1);
    var a10 = new Node<int>(10, "A-10");
    var a200 = new Node<int>(200, "A-200");
    var a300 = new Node<int>(300, "A-300");
    a.Add(a10); a10.Add(a200); a200.Add(a300);

    // B: 1 -> 10 -> 200 -> {300(B), 301(B)}
    var b = new Node<int>(1);
    var b10 = new Node<int>(10, "B-10");
    var b200 = new Node<int>(200, "B-200");
    var b300 = new Node<int>(300, "B-300");
    var b301 = new Node<int>(301, "B-301");
    b.Add(b10); b10.Add(b200); b200.Add(b300, b301);

    var merged = a.MergeWith<Node<int>, int>(b);

    // Ensure instances from A are kept at 10 and 200 and 300
    var m10 = (Node<int>)merged.Children.Single();
    var m200 = (Node<int>)m10.Children.Single();
    var mChildren200 = m200.Children.Select(c => (Node<int>)c).ToArray();

    Assert.Same(a10, m10);
    Assert.Same(a200, m200);
    Assert.Contains(mChildren200, n => ReferenceEquals(n, a300)); // 300 kept from A
    Assert.Contains(mChildren200, n => n.Id == 301 && n.Name == "B-301"); // 301 added from B
  }

  [Fact]
  public void Merge_Respects_CustomComparer_By_IdOnly()
  {
    // A and B roots equal by Id (ignore Version)
    var a = new Node<NodeKey>(new NodeKey(1, 0))
        .Add(new Node<NodeKey>(new NodeKey(10, 1), "A-10-v1"));

    var b = new Node<NodeKey>(new NodeKey(1, 99))
        .Add(new Node<NodeKey>(new NodeKey(10, 2), "B-10-v2"),
             new Node<NodeKey>(new NodeKey(12, 1), "B-12"));

    var merged = a.MergeWith<Node<NodeKey>, NodeKey>(b, new NodeKeyIdOnlyComparer());

    var names = merged.Children.Select(c => ((Node<NodeKey>)c).Name!).OrderBy(x => x).ToArray();
    // Expect A's 10 kept, plus 12 from B
    Assert.Equal(new[] { "A-10-v1", "B-12" }, names);
  }
}
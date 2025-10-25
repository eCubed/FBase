namespace FBase.Foundations.Trees;
public static class TreeTraversalExtensions
{
  // -------- Depth-First (Pre-Order) --------
  public static void TraverseDepthFirst<TNode, TKey>(
      this TNode root,
      Action<TNode> visit,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) throw new ArgumentNullException(nameof(root));
    if (visit == null) throw new ArgumentNullException(nameof(visit));

    var stack = new Stack<TNode>();
    stack.Push(root);

    while (stack.Count > 0)
    {
      var node = stack.Pop();
      visit(node);

      // Optional pruning: if provided and returns false, don't push children.
      if (descendPredicate != null && !descendPredicate(node))
        continue;

      // Push children in reverse so the first child is processed first.
      var children = node.Children;
      if (children != null && children.Count > 0)
      {
        for (int i = children.Count - 1; i >= 0; i--)
        {
          stack.Push((TNode)children[i]);
        }
      }
    }
  }

  // -------- Depth-First (Post-Order) --------
  public static void TraverseDepthFirstPostOrder<TNode, TKey>(
      this TNode root,
      Action<TNode> visit,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) throw new ArgumentNullException(nameof(root));
    if (visit == null) throw new ArgumentNullException(nameof(visit));

    var stack = new Stack<(TNode node, int childIndex)>();
    stack.Push((root, 0));

    while (stack.Count > 0)
    {
      var (node, idx) = stack.Pop();

      var children = (descendPredicate == null || descendPredicate(node))
          ? node.Children
          : null;

      if (children != null && idx < children.Count)
      {
        // Re-push this node to continue after its next child
        stack.Push((node, idx + 1));
        // Dive into the next child
        stack.Push(((TNode)children[idx], 0));
      }
      else
      {
        // All children processed -> visit node
        visit(node);
      }
    }
  }

  // -------- Breadth-First (Level-Order) --------
  public static void TraverseBreadthFirst<TNode, TKey>(
      this TNode root,
      Action<TNode> visit,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) throw new ArgumentNullException(nameof(root));
    if (visit == null) throw new ArgumentNullException(nameof(visit));

    var queue = new Queue<TNode>();
    queue.Enqueue(root);

    while (queue.Count > 0)
    {
      var node = queue.Dequeue();
      visit(node);

      if (descendPredicate != null && !descendPredicate(node))
        continue;

      var children = node.Children;
      if (children != null && children.Count > 0)
      {
        foreach (var child in children)
        {
          queue.Enqueue((TNode)child);
        }
      }
    }
  }

  // -------- IEnumerable versions (handy for LINQ) --------
  public static IEnumerable<TNode> DepthFirst<TNode, TKey>(
      this TNode root,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) yield break;

    var stack = new Stack<TNode>();
    stack.Push(root);

    while (stack.Count > 0)
    {
      var node = stack.Pop();
      yield return node;

      if (descendPredicate != null && !descendPredicate(node))
        continue;

      var children = node.Children;
      if (children != null && children.Count > 0)
      {
        for (int i = children.Count - 1; i >= 0; i--)
          stack.Push((TNode)children[i]);
      }
    }
  }

  public static IEnumerable<TNode> BreadthFirst<TNode, TKey>(
      this TNode root,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) yield break;

    var queue = new Queue<TNode>();
    queue.Enqueue(root);

    while (queue.Count > 0)
    {
      var node = queue.Dequeue();
      yield return node;

      if (descendPredicate != null && !descendPredicate(node))
        continue;

      var children = node.Children;
      if (children != null && children.Count > 0)
      {
        foreach (var child in children)
          queue.Enqueue((TNode)child);
      }
    }
  }
}

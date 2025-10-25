namespace FBase.Foundations.Trees;
public static class TreeFindExtensions
{
  /// <summary>
  /// Depth-first (pre-order) search for the first node with the given id.
  /// Returns null if not found.
  /// </summary>
  public static TNode? FindByIdDepthFirst<TNode, TKey>(
      this TNode root,
      TKey id,
      Func<TNode, bool>? descendPredicate = null,
      IEqualityComparer<TKey>? comparer = null)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) return null;

    comparer ??= EqualityComparer<TKey>.Default;

    var stack = new Stack<TNode>();
    stack.Push(root);

    while (stack.Count > 0)
    {
      var node = stack.Pop();
      if (comparer.Equals(node.Id, id))
        return node;

      if (descendPredicate != null && !descendPredicate(node))
        continue;

      var children = node.Children;
      if (children != null && children.Count > 0)
      {
        // reverse push for natural left-to-right order
        for (int i = children.Count - 1; i >= 0; i--)
          stack.Push((TNode)children[i]);
      }
    }

    return null;
  }

  /// <summary>
  /// Breadth-first (level-order) search for the first node with the given id.
  /// Returns null if not found.
  /// </summary>
  public static TNode? FindByIdBreadthFirst<TNode, TKey>(
      this TNode root,
      TKey id,
      Func<TNode, bool>? descendPredicate = null,
      IEqualityComparer<TKey>? comparer = null)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) return null;

    comparer ??= EqualityComparer<TKey>.Default;

    var queue = new Queue<TNode>();
    queue.Enqueue(root);

    while (queue.Count > 0)
    {
      var node = queue.Dequeue();
      if (comparer.Equals(node.Id, id))
        return node;

      if (descendPredicate != null && !descendPredicate(node))
        continue;

      var children = node.Children;
      if (children != null && children.Count > 0)
      {
        foreach (var child in children)
          queue.Enqueue((TNode)child);
      }
    }

    return null;
  }

  /// <summary>
  /// Try-pattern wrapper for DFS.
  /// </summary>
  public static bool TryFindByIdDepthFirst<TNode, TKey>(
      this TNode root,
      TKey id,
      out TNode? result,
      Func<TNode, bool>? descendPredicate = null,
      IEqualityComparer<TKey>? comparer = null)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    result = FindByIdDepthFirst<TNode, TKey>(root, id, descendPredicate, comparer);
    return result != null;
  }

  /// <summary>
  /// Try-pattern wrapper for BFS.
  /// </summary>
  public static bool TryFindByIdBreadthFirst<TNode, TKey>(
      this TNode root,
      TKey id,
      out TNode? result,
      Func<TNode, bool>? descendPredicate = null,
      IEqualityComparer<TKey>? comparer = null)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    result = FindByIdBreadthFirst<TNode, TKey>(root, id, descendPredicate, comparer);
    return result != null;
  }
}

using FBase.Foundations.Trees;

public static class TreePathExtensions
{
  /// <summary>
  /// Returns all root-to-node paths in pre-order (parent before children).
  /// Each inner list is a snapshot of the path at that node.
  /// Respects descendPredicate for pruning.
  /// </summary>
  public static List<List<TNode>> PathsDepthFirst<TNode, TKey>(
      this TNode root,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    var results = new List<List<TNode>>();
    if (root == null) return results;

    var stack = new Stack<(TNode node, int childIndex)>();
    var path = new List<TNode>();

    stack.Push((root, -1));

    while (stack.Count > 0)
    {
      var (node, idx) = stack.Pop();

      if (idx == -1)
      {
        // First time we see this node: enter it
        path.Add(node);
        results.Add(new List<TNode>(path)); // snapshot

        // If we shouldn't descend, exit immediately
        if (descendPredicate != null && !descendPredicate(node))
        {
          path.RemoveAt(path.Count - 1);
          continue;
        }

        // Re-push this node with childIndex=0 to process its children
        stack.Push((node, 0));

        // If it has children, push first child frame
        var children = node.Children;
        if (children != null && children.Count > 0)
        {
          // We’ll push the first child now via parent’s 0 index handling
          // (Handled in the idx >= 0 branch below)
        }
      }
      else
      {
        var children = node.Children;
        if (children != null && idx < children.Count)
        {
          // We will revisit this node for the next child
          stack.Push((node, idx + 1));
          // Dive into current child
          stack.Push(((TNode)children[idx], -1));
        }
        else
        {
          // All children of this node processed: leave it
          path.RemoveAt(path.Count - 1);
        }
      }
    }

    return results;
  }

  /// <summary>
  /// Lazy version: yields each root-to-node path (as a read-only copy) in pre-order.
  /// Respects descendPredicate for pruning.
  /// </summary>
  public static IEnumerable<IReadOnlyList<TNode>> PathsDepthFirstLazy<TNode, TKey>(
      this TNode root,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) yield break;

    var stack = new Stack<(TNode node, int childIndex)>();
    var path = new List<TNode>();

    stack.Push((root, -1));

    while (stack.Count > 0)
    {
      var (node, idx) = stack.Pop();

      if (idx == -1)
      {
        path.Add(node);
        // expose a copy snapshot; caller can enumerate safely
        yield return path.ToArray();

        if (descendPredicate != null && !descendPredicate(node))
        {
          path.RemoveAt(path.Count - 1);
          continue;
        }

        stack.Push((node, 0));
      }
      else
      {
        var children = node.Children;
        if (children != null && idx < children.Count)
        {
          stack.Push((node, idx + 1));
          stack.Push(((TNode)children[idx], -1));
        }
        else
        {
          path.RemoveAt(path.Count - 1);
        }
      }
    }
  }
}

using FBase.Foundations.Trees;
public static class TreeFindByPredicateExtensions
{
  // -------- Streaming (IEnumerable) versions --------

  /// <summary>
  /// Depth-first (pre-order) streaming filter of nodes that satisfy predicate.
  /// Respects optional descendPredicate for pruning.
  /// </summary>
  public static IEnumerable<TNode> WhereDepthFirst<TNode, TKey>(
      this TNode root,
      Func<TNode, bool> predicate,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));
    return root.DepthFirst<TNode, TKey>(descendPredicate).Where(predicate);
  }

  /// <summary>
  /// Breadth-first (level-order) streaming filter of nodes that satisfy predicate.
  /// Respects optional descendPredicate for pruning.
  /// </summary>
  public static IEnumerable<TNode> WhereBreadthFirst<TNode, TKey>(
      this TNode root,
      Func<TNode, bool> predicate,
      Func<TNode, bool>? descendPredicate = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));
    return root.BreadthFirst<TNode, TKey>(descendPredicate).Where(predicate);
  }

  // -------- Materialized (List) versions --------

  /// <summary>
  /// Depth-first (pre-order) find all matches.
  /// Optional maxCount for early exit.
  /// </summary>
  public static List<TNode> FindAllDepthFirst<TNode, TKey>(
      this TNode root,
      Func<TNode, bool> predicate,
      Func<TNode, bool>? descendPredicate = null,
      int? maxCount = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));
    var results = new List<TNode>();
    if (root == null) return results;

    foreach (var node in root.DepthFirst<TNode, TKey>(descendPredicate))
    {
      if (predicate(node))
      {
        results.Add(node);
        if (maxCount.HasValue && results.Count >= maxCount.Value)
          break; // early exit for performance
      }
    }
    return results;
  }

  /// <summary>
  /// Breadth-first (level-order) find all matches.
  /// Optional maxCount for early exit.
  /// </summary>
  public static List<TNode> FindAllBreadthFirst<TNode, TKey>(
      this TNode root,
      Func<TNode, bool> predicate,
      Func<TNode, bool>? descendPredicate = null,
      int? maxCount = null)
      where TNode : ITreeNode<TKey>
      where TKey : struct
  {
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));
    var results = new List<TNode>();
    if (root == null) return results;

    foreach (var node in root.BreadthFirst<TNode, TKey>(descendPredicate))
    {
      if (predicate(node))
      {
        results.Add(node);
        if (maxCount.HasValue && results.Count >= maxCount.Value)
          break;
      }
    }
    return results;
  }
}

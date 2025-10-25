using FBase.Foundations.Trees;

public static class TreeMergeExtensions
{
  /// <summary>
  /// Deep-merges tree 'b' into tree 'a' and returns 'a'.
  /// Rules:
  /// - Root keys must be equal (by comparer).
  /// - Children are unioned by key.
  /// - On key collision at any depth: keep A's node instance and recursively merge children from B into it.
  /// - Children that exist only in B are appended to A.
  /// 
  /// This mutates 'a' in place.
  /// </summary>
  public static TNode MergeWith<TNode, TKey>(
      this TNode a,
      TNode b,
      IEqualityComparer<TKey>? keyComparer = null)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    keyComparer ??= EqualityComparer<TKey>.Default;

    if (a is null) return b;   // allow "merge into null" -> just return B
    if (b is null) return a;

    if (!keyComparer.Equals(a.Id, b.Id))
      throw new ArgumentException(
          $"Root mismatch: {a.Id} vs {b.Id}. Merge requires equal root keys.");

    MergeChildren(a, b, keyComparer);
    return a;
  }

  private static void MergeChildren<TNode, TKey>(
      TNode a,
      TNode b,
      IEqualityComparer<TKey> keyComparer)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    var aChildren = a.Children ?? new List<ITreeNode<TKey>>();
    var bChildren = b.Children ?? new List<ITreeNode<TKey>>();

    if (bChildren.Count == 0)
      return;

    // Index A's children by key (first occurrence wins)
    var aIndex = new Dictionary<TKey, TNode>(keyComparer);
    for (int i = 0; i < aChildren.Count; i++)
    {
      if (aChildren[i] is null) continue;
      if (aChildren[i] is not TNode aTyped)
        throw new InvalidOperationException(
            $"Child of A with key {aChildren[i].Id} is not {typeof(TNode).Name}.");
      if (!aIndex.ContainsKey(aTyped.Id))
        aIndex[aTyped.Id] = aTyped;
    }

    foreach (var bChildBase in bChildren)
    {
      if (bChildBase is null) continue;
      if (bChildBase is not TNode bChild)
        throw new InvalidOperationException(
            $"Child of B with key {bChildBase.Id} is not {typeof(TNode).Name}.");

      // If A already has this key -> recurse into the existing A child.
      if (aIndex.TryGetValue(bChild.Id, out var aExisting))
      {
        // Deep-merge: keep A's instance, merge B's subtree into it.
        MergeChildren<TNode, TKey>(aExisting, bChild, keyComparer);
      }
      else
      {
        // Unique to B -> append B's subtree under A.
        aChildren.Add(bChild);
        aIndex[bChild.Id] = bChild;
      }
    }
  }
}

using FBase.Foundations.Trees;


public static class TreeTransformExtensions
{
  // ========== DeepCopy ==========
  /// <summary>
  /// Deep-copies a tree.
  /// You provide:
  ///   - cloneFactory: how to clone a single node (no children yet)
  ///   - addChild: how to append a child to a parent clone
  /// </summary>
  public static TNode DeepCopy<TNode, TKey>(
      this TNode root,
      Func<TNode, TNode> cloneFactory,
      Action<TNode, TNode> addChild)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) return null!;
    if (cloneFactory == null) throw new ArgumentNullException(nameof(cloneFactory));
    if (addChild == null) throw new ArgumentNullException(nameof(addChild));

    var clone = cloneFactory(root);
    var children = root.Children;
    if (children != null && children.Count > 0)
    {
      foreach (var ch in children)
      {
        var childClone = ((TNode)ch).DeepCopy<TNode, TKey>(cloneFactory, addChild);
        addChild(clone, childClone);
      }
    }
    return clone;
  }

  // ========== Map ==========
  /// <summary>
  /// Maps a tree of TIn into a tree of TOut, preserving structure.
  /// mapFactory maps a single node (no children yet).
  /// addChild wires TOut children onto a TOut parent.
  /// </summary>
  public static TOut Map<TIn, TOut, TKey>(
      this TIn root,
      Func<TIn, TOut> mapFactory,
      Action<TOut, TOut> addChild)
      where TIn : class, ITreeNode<TKey>
      where TOut : class, ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) return null!;
    if (mapFactory == null) throw new ArgumentNullException(nameof(mapFactory));
    if (addChild == null) throw new ArgumentNullException(nameof(addChild));

    var mapped = mapFactory(root);
    var children = root.Children;
    if (children != null && children.Count > 0)
    {
      foreach (var ch in children)
      {
        var mappedChild = ((TIn)ch).Map<TIn, TOut, TKey>(mapFactory, addChild);
        addChild(mapped, mappedChild);
      }
    }
    return mapped;
  }

  // ========== Filter (structure-preserving) ==========
  /// <summary>
  /// Returns a subtree that keeps nodes that match predicate OR have any kept descendants.
  /// If neither applies, returns null.
  /// buildParent: how to create a new node instance when keeping this node.
  /// addChild: how to append kept children to the new parent.
  /// </summary>
  public static TNode? Filter<TNode, TKey>(
      this TNode root,
      Func<TNode, bool> predicate,
      Func<TNode, TNode> buildParent,
      Action<TNode, TNode> addChild)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) return null;
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));
    if (buildParent == null) throw new ArgumentNullException(nameof(buildParent));
    if (addChild == null) throw new ArgumentNullException(nameof(addChild));

    // Recurse into children first
    var keptChildren = new List<TNode>();
    var children = root.Children;
    if (children != null && children.Count > 0)
    {
      foreach (var ch in children)
      {
        var kept = ((TNode)ch).Filter<TNode, TKey>(predicate, buildParent, addChild);
        if (kept != null) keptChildren.Add(kept);
      }
    }

    var keepSelf = predicate(root);
    if (!keepSelf && keptChildren.Count == 0)
      return null;

    // Keep this node (even if predicate == false) to connect its kept descendants.
    var newNode = buildParent(root);
    foreach (var kc in keptChildren)
      addChild(newNode, kc);

    return newNode;
  }

  // ========== SortChildren (recursive, in-place) ==========
  /// <summary>
  /// Recursively sorts each node's Children list by a key selector.
  /// </summary>
  public static void SortChildrenRecursive<TNode, TKey, TOrderKey>(
      this TNode root,
      Func<TNode, TOrderKey> keySelector,
      IComparer<TOrderKey>? comparer = null,
      bool descending = false)
      where TNode : class, ITreeNode<TKey>
      where TKey : struct
  {
    if (root == null) return;
    if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

    comparer ??= Comparer<TOrderKey>.Default;

    var children = root.Children;
    if (children != null && children.Count > 0)
    {
      // Convert to TNode list to use selector
      var typed = children.Cast<TNode>().ToList();
      typed.Sort((a, b) =>
      {
        var cmp = comparer.Compare(keySelector(a), keySelector(b));
        return descending ? -cmp : cmp;
      });

      // Write back preserving the ITreeNode list type
      children.Clear();
      foreach (var ch in typed) children.Add(ch);

      // Recurse
      foreach (var ch in typed)
        ch.SortChildrenRecursive<TNode, TKey, TOrderKey>(keySelector, comparer, descending);
    }
  }
}

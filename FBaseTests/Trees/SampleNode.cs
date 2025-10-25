using FBase.Foundations.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBaseTests.Trees;
public sealed class SampleNode : ITreeNode<int>
{
  public int Id { get; }
  public string Name { get; }
  public List<ITreeNode<int>> Children { get; } = new();

  public SampleNode(int id, string? name = null)
  {
    Id = id;
    Name = name ?? id.ToString();
  }

  public SampleNode Add(params SampleNode[] children)
  {
    Children.AddRange(children);
    return this;
  }

  public override string ToString() => Name;
}
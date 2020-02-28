using System;
using System.Collections.Generic;
using System.Text;

namespace FBase.Foundations
{
    public interface IIdentifiable<out TKey>
    {
        TKey Id { get; }
    }
}

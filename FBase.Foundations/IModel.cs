using System;
using System.Collections.Generic;
using System.Text;

namespace FBase.Foundations
{    public interface IModel<T>
       where T : class
    {
        void UpdateObject(T data);
        void FillModel(T data);
    }
}

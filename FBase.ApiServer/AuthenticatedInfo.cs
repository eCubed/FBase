using System;

namespace FBase.ApiServer
{
    public class AuthenticatedInfo<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey RequestorId { get; set; }
        public string RequestorName { get; set; }
    }
}

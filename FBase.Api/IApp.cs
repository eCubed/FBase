using FBase.Foundations;

namespace FBase.Api
{
    public interface IApp<TUserKey> : IIdentifiable<long>
        where TUserKey : IEquatable<TUserKey>
    {
        TUserKey UserId { get; set; }
        string Name { get; set; }
        DateTime? CreatedDate { get; set; }
    }
}

namespace FBase.ApiServer
{
    public interface IAuthenticatedInfo<TKey>
    {
        TKey RequestorId { get; set; }
        string RequestorName { get; set; }
    }
}

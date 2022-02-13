using FBase.Foundations;

namespace NetCore6WebApi.Data;

public interface IThing : IIdentifiable<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string UserId { get; set; }
}

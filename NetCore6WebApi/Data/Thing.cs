namespace NetCore6WebApi.Data;

public class Thing : IThing
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string UserId { get; set; } = null!;
    public virtual TestingUser User { get; set; } = null!;
    public int Id { get; set; }
}

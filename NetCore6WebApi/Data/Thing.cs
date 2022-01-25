namespace NetCore6WebApi.Data;

public class Thing : IThing
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Id { get; set; }
}

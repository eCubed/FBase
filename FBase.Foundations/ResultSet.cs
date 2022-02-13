namespace FBase.Foundations;

public class ResultSet<T>
    where T : class
{
    public int TotalRecordsCount { get; set; }
    public bool HasPreviousResults { get; set; }
    public bool HasNextResults { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public List<T>? Records { get; set; }
    public int NumPages { get; set; }
}

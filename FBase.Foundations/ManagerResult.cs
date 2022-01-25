namespace FBase.Foundations;

public class ManagerResult
{
    public bool Success { get; private set; }

    public IEnumerable<string>? Errors { get; private set; }

    public ManagerResult()
    {
        Success = true;
    }

    public ManagerResult(params string[]? errors)
    {
        if (errors == null)
        {
            Success = true;
        }
        else
        {
            Errors = errors;
            Success = false;
        }
    }
}

public class ManagerResult<TData> : ManagerResult
{
    public TData? Data { get; private set; }

    public ManagerResult(params string[]? errors) : base(errors)
    {
    }

    public ManagerResult(IEnumerable<string>? errors) : base(errors?.ToArray())
    {
    }

    public ManagerResult(TData? data) : base()
    {
        Data = data;
    }

    public ManagerResult(TData data, params string[]? errors) : base(errors)
    {
        Data = data;
    }
}

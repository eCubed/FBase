namespace FBase.Foundations;

public static class ManagerExtensions
{
    public static ManagerResult CreateManagerResult(this Exception e, string primaryMessage = "")
    {
        List<string> errors = new List<string>();

        if (!string.IsNullOrEmpty(primaryMessage))
            errors.Add(primaryMessage);

        errors.Add(e.Message);

        Exception? inner = e.InnerException;
        while (inner != null)
        {
            errors.Add(inner.Message);
            inner = inner.InnerException;
        }

        return new ManagerResult(errors.ToArray());
    }
}

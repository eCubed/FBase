namespace FBase.Foundations;

public static class DynamicTypes
{
    public static T Cast<T>(object o, T type)
    {
        return (T)o;
    }

    public static T? Instantiate<T>(params object[] args)
    {
        return (T?)Activator.CreateInstance(typeof(T), args);
    }

    public static T To<T>(this object obj)
    {
        Type t = typeof(T);
        Type? u = Nullable.GetUnderlyingType(t);

        if (u != null)
        {
            return (obj == null) ? default : (T)Convert.ChangeType(obj, u);
        }
        else
        {
            return (T)Convert.ChangeType(obj, t);
        }
    }
}

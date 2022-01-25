namespace FBase.Foundations;

public static class Basics
{
    /// <summary>
    /// Switches the values of A and B
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static void Switch<T>(ref T A, ref T B)
    {
        T C = A;
        A = B;
        B = C;
    }

    /// <summary>
    /// A is set equal to the value of B if B's value
    /// does not equal A's value in the first place.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static void SetIfNotEqual<T>(ref T? A, T B)
    {
        if (!A?.Equals(B) ?? false)
            A = B;
    }

    /// <summary>
    /// Sets A equal to B only if A was null to begin with.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static void SetIfNull<T>(T A, T B)
    {
        A = (A == null) ? B : A;
    }

    /// <summary>
    /// Sets A equal to B only if A was null to begin with.
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static void SetIfEmptyString(string A, string B)
    {
        A = (String.IsNullOrEmpty(A)) ? B : A;
    }

    // We currently don't know why this is necessary.
    public static void SetIfEmptyStringRef(ref string? A, string B)
    {
        A = (String.IsNullOrEmpty(A)) ? B : A;
    }

    /// <summary>
    /// REINSTATED.... We'll remove this function now 12/01/2015
    /// because .net Core T[] already has has no ToList() function!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>        
    public static List<T> ArrayToList<T>(T[] array)
    {
        List<T> List = new List<T>();
        for (int i = 0; i < array.Length; i++)
        {
            List.Add(array[i]);
        }

        return List;
    }
}

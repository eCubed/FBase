namespace FBase.Foundations;

public static class ListTools
{
    /// <summary>
    /// Returns whether all items in checkFor are in testList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="testList"></param>
    /// <param name="checkFor"></param>
    /// <returns></returns>
    public static bool ContainsAll<T>(this IEnumerable<T> testList, IEnumerable<T> checkFor)
    {
        return !checkFor.Except(testList).Any();
    }

    /// <summary>
    /// Returns whether any item in checkFor is in testList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="testList"></param>
    /// <param name="checkFor"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(this IEnumerable<T> testList, IEnumerable<T> checkFor)
    {
        return testList.Intersect(checkFor).Any();
    }

    /// <summary>
    /// A wrapper for Except() with a friendlier and more intuitive name.
    /// Returns a list of remaining items after removing all items from
    /// supposedDuplicates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="testList"></param>
    /// <param name="supposedDuplicates"></param>
    /// <returns></returns>
    public static IEnumerable<T> RemoveCommon<T>(this IEnumerable<T> testList, IEnumerable<T> supposedDuplicates)
    {
        return testList.Except(supposedDuplicates);
    }

    public static IEnumerable<T> RemoveWhere<T>(this List<T> masterList, Predicate<T> criteria)
    {
        masterList.RemoveAll(criteria);

        return masterList;
    }

    public static IEnumerable<T> ExceptWhere<T>(this IList<T> masterList, Func<T, bool> criteria)
    {
        var whereStuff = masterList.Where(criteria);
        return masterList.RemoveCommon(whereStuff);
    }

    /// <summary>
    /// Would return an empty array if masterList were only one element.
    /// If masterList has no elements, we return itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="masterList"></param>
    /// <returns></returns>
    public static T[] ExceptFirst<T>(this T[] masterList)
    {
        if (masterList.Length == 0)
            return masterList;

        T[] noFirst = new T[masterList.Length - 1];

        for (int i = 1; i < masterList.Length; i++)
        {
            noFirst[i - 1] = masterList[i];
        }

        return noFirst;
    }

    public static T[] ExceptLast<T>(this T[] masterList)
    {
        if (masterList.Length == 0)
            return masterList;

        T[] noLast = new T[masterList.Length - 1];

        for (int i = 0; i < masterList.Length - 1; i++)
        {
            noLast[i] = masterList[i];
        }

        return noLast;
    }
}

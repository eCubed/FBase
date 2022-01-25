namespace FBase.Foundations;

public static class DateTools
{
    public static DateTime GetDateOrCurrentDate(DateTime? someDate)
    {
        if (someDate == null)
            return DateTime.Now;

        if (someDate.HasValue)
            return someDate.Value;

        return DateTime.Now;
    }
}

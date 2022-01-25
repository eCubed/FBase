using System.Text;
using System.Text.RegularExpressions;

namespace FBase.Foundations;

public static class StringTools
{
    public static string StripPunctuation(this string text)
    {
        var sb = new StringBuilder();
        foreach (char c in text)
        {
            if (!char.IsPunctuation(c))
                sb.Append(c);
        }
        return sb.ToString();
    }

    public static string ToKebabCase(this string text)
    {
        return Regex.Replace(text.ToLower().StripPunctuation(), @"\s+", "-");
    }

    public static string ToValidFilename(this string f)
    {
        // replace all the whitespace characters in a row with underscore.
        f = Regex.Replace(f, @"\s+", "_");

        // stage 2, replace all the illegal characters with "_"
        char[] illegalFilenameChars = Path.GetInvalidFileNameChars();
        foreach (char c in illegalFilenameChars)
        {
            f = f.Replace(c, '_');
        }

        return f;
    }

    public static bool ContainsAny(this string str, IEnumerable<string> searchTerms)
    {
        return searchTerms.Any(searchTerm => str.ToLower().Contains(searchTerm.ToLower()));
    }

    public static bool ContainsAll(this string str, IEnumerable<string> searchTerms)
    {
        return searchTerms.All(searchTerm => str.ToLower().Contains(searchTerm.ToLower()));
    }

    public static string ToCamelCase(this string the_string)
    {
        if (the_string == null || the_string.Length < 2)
            return the_string;

        // Split the string into words.
        string[] words = the_string.Split(
            new char[] { },
            StringSplitOptions.RemoveEmptyEntries);

        // Combine the words.
        string result = words[0].Substring(0, 1).ToLower() + words[0].Substring(1); //.ToLower();
        for (int i = 1; i < words.Length; i++)
        {
            result +=
                words[i].Substring(0, 1).ToUpper() +
                words[i].Substring(1);
        }

        return result;
    }
}

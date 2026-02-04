namespace OpenStar.Core.Extensions;

/// <summary>
/// String extension methods
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string to a slug
    /// </summary>
    /// <param name="s">The string to convert</param>
    /// <returns>The string as a slug</returns>
    public static string ToSlug(this string s)
    {
        var ss = s.ToLower().AsSpan();

        Span<char> o = stackalloc char[ss.Length];

        int j = 0;
        foreach (char c in ss)
            switch (c)
            {
                case >= '!' and <= ','
                  or >= '.' and <= '/'
                  or >= ':' and <= '@'
                  or >= '[' and <= '^'
                  or '`'
                  or >= '{':
                    continue;
                case ' ':
                    o[j++] = '-';
                    break;
                default:
                    o[j++] = c;
                    break;
            }

        return new string(o[..j]);
    }

    /// <summary>
    /// Turns the first char to lowercase in a string
    /// </summary>
    /// <param name="str">The string</param>
    /// <returns>The same string with the char as lowercase</returns>
    /// <url>https://stackoverflow.com/a/21755933</url>
    public static string? FirstCharToLowerCase(this string? str)
    {
        if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
            return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

        return str;
    }
}
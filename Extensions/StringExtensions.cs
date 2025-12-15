namespace OpenStar.Extensions;

public static class StringExtensions
{
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

    // https://stackoverflow.com/a/21755933
    public static string? FirstCharToLowerCase(this string? str)
    {
        if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
            return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

        return str;
    }
}
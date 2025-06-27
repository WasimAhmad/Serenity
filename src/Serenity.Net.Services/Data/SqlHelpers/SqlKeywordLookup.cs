namespace Serenity.Data;

public static class SqlKeywordLookup
{
    public static bool IsReserved(string[] keywords, string value)
    {
        if (string.IsNullOrEmpty(value) || keywords.Length == 0)
            return false;
        return IsReserved((ReadOnlySpan<string>)keywords, value);
    }

    public static bool IsReserved(ReadOnlySpan<string> keywords, string value)
    {
        if (string.IsNullOrEmpty(value) || keywords.Length == 0)
            return false;

        int left = 0;
        int right = keywords.Length - 1;
        while (left <= right)
        {
            int mid = (left + right) >> 1;
            int cmp = string.Compare(keywords[mid], value, StringComparison.OrdinalIgnoreCase);
            if (cmp == 0)
                return true;
            if (cmp < 0)
                left = mid + 1;
            else
                right = mid - 1;
        }
        return false;
    }
}

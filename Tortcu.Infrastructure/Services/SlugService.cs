using System.Text;
using System.Text.RegularExpressions;

namespace Tortcu.Infrastructure.Services;

public sealed partial class SlugService : ISlugService
{
    public string Slugify(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        var normalized = input.Trim().ToLowerInvariant();
        normalized = MapAzChars(normalized);

        var sb = new StringBuilder(normalized.Length);
        foreach (var ch in normalized)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9'))
            {
                sb.Append(ch);
                continue;
            }

            if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_' || ch == '.')
            {
                sb.Append('-');
            }
        }

        var slug = MultiDash().Replace(sb.ToString(), "-").Trim('-');
        return slug;
    }

    private static string MapAzChars(string s)
        => s
            .Replace('ə', 'e')
            .Replace('ı', 'i')
            .Replace('ö', 'o')
            .Replace('ü', 'u')
            .Replace('ç', 'c')
            .Replace('ş', 's')
            .Replace('ğ', 'g');

    [GeneratedRegex("-{2,}", RegexOptions.Compiled)]
    private static partial Regex MultiDash();
}


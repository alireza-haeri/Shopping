using System.Text.RegularExpressions;

namespace Shopping.WebFramework.Extensions;

public static class RegExHelpers
{
    public static bool MatchesApiVersion(string apiVersion, string text)
    {
        var pattern = $@"(?<=\/|^){Regex.Escape(apiVersion)}(?=\/|$)";

        var exactMatchRegex = new Regex(pattern, RegexOptions.Compiled);

        return exactMatchRegex.IsMatch(text);
    }
}
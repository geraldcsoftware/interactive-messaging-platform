using System.Text.RegularExpressions;

namespace MessageInteractionService.Core;

public partial class CommonValidators
{
    private const string NamePattern = @"^([A-Z][a-z]+)([\s]?[A-Z][a-z]+)$";

    [GeneratedRegex(NamePattern, RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 1000)]
    private static partial Regex GetNameRegex();

    public static bool IsNameValid(string? name)
    {
        return !string.IsNullOrEmpty(name) &&
               GetNameRegex().IsMatch(name);
    }
}
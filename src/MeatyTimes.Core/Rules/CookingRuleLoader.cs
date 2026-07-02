using System.Reflection;
using System.Text.Json;
using MeatyTimes.Core.Domain;

namespace MeatyTimes.Core.Rules;

/// <summary>
/// Loads and provides access to embedded cooking rules from cooking-rules.json.
/// </summary>
public sealed class CookingRuleLoader
{
    private readonly IReadOnlyList<CookingRule> _rules;
    private readonly IReadOnlyList<MeatTypeInfo> _meatTypes;

    public CookingRuleLoader()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "MeatyTimes.Core.Rules.cooking-rules.json";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var document = JsonSerializer.Deserialize<CookingRulesDocument>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize cooking rules.");

        if (document.Rules.Count == 0)
        {
            throw new InvalidOperationException("Cooking rules file contains no rules.");
        }

        _rules = document.Rules;
        _meatTypes = document.Rules
            .Select(r => new MeatTypeInfo(
                ParseMeatType(r.MeatType),
                r.DisplayName,
                r.SupportsDoneness,
                r.SupportsDoneness
                    ? [Domain.Doneness.Rare, Domain.Doneness.Medium, Domain.Doneness.WellDone]
                    : [],
                r.MinWeightKg,
                r.MaxWeightKg,
                ParseCookingMethod(r.CookingMethod)))
            .ToList();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public IReadOnlyList<MeatTypeInfo> GetMeatTypes() => _meatTypes;

    public CookingRule GetRule(MeatTypeId meatType, CookingMethod method = CookingMethod.TraditionalRoast)
    {
        var meatKey = meatType.ToString().ToLowerInvariant();
        var methodKey = method.ToString();

        var rule = _rules.FirstOrDefault(r =>
            string.Equals(r.MeatType, meatKey, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(r.CookingMethod, methodKey, StringComparison.OrdinalIgnoreCase));

        return rule ?? throw new RoastValidationException($"Unsupported meat type '{meatType}'.");
    }

    private static MeatTypeId ParseMeatType(string value) =>
        Enum.Parse<MeatTypeId>(value, ignoreCase: true);

    private static CookingMethod ParseCookingMethod(string value) =>
        Enum.Parse<CookingMethod>(value, ignoreCase: true);

    private sealed class CookingRulesDocument
    {
        public List<CookingRule> Rules { get; init; } = [];
    }
}

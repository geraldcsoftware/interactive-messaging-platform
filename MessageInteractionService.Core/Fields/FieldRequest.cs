namespace MessageInteractionService.Core.Fields;

public class FieldRequest
{
    private readonly IList<FieldValidationRule> _fieldValidationRules = new List<FieldValidationRule>();
    public required string Key { get; init; }
    public string Type { get; set; } = "Text";
    public string? PromptMessage { get; set; }
    public IReadOnlyCollection<FieldValidationRule> ValidationRules => _fieldValidationRules.AsReadOnly();

    public void AddValidationRule(FieldValidationRule rule)
    {
        _fieldValidationRules.Add(rule);
    }

    public class FieldValidationRule
    {
        private readonly Func<string?, bool> _action;

        public FieldValidationRule(Func<string?, bool> action, string validationFailureMessage)
        {
            _action = action;
            ValidationFailureMessage = validationFailureMessage;
        }

        public string ValidationFailureMessage { get; }

        public bool Validate(string? rawValue)
        {
            return _action(rawValue);
        }
    }
}
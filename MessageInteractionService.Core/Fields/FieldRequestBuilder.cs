namespace MessageInteractionService.Core.Fields;

public class FieldRequestBuilder
{
    private readonly ISessionFieldStore _sessionFieldStore;
    private readonly FieldRequest _fieldRequest;
    private string? _suppliedInput;
    private bool _inputSupplied;

    private FieldRequestBuilder(ISessionFieldStore fieldStore, FieldRequest fieldRequest)
    {
        _sessionFieldStore = fieldStore;
        _fieldRequest = fieldRequest;
    }

    public static FieldRequestBuilder New(ISessionFieldStore fieldStore, string fieldKey)
    {
        return new(fieldStore, new()
        {
            Key = fieldKey
        });
    }

    public FieldRequestBuilder WithPromptMessage(string promptMessage)
    {
        _fieldRequest.PromtMessage = promptMessage;
        return this;
    }

    public FieldRequestBuilder WithValidation(Func<string?, bool> action, string validationMessage)
    {
        _fieldRequest.AddValidationRule(new(action, validationMessage));
        return this;
    }

    public FieldRequestBuilder WithInput(string? inputContent)
    {
        _inputSupplied = true;
        _suppliedInput = inputContent;
        return this;
    }

    public async Task<FieldRequestResult> RequestField()
    {
        var fieldKey = _fieldRequest.Key;
        if (!_sessionFieldStore.HasRequestedField(fieldKey))
        {
            await _sessionFieldStore.AddFieldRequestToSession(fieldKey);
            var promptResponse = _sessionFieldStore.Prompt(_fieldRequest.PromtMessage);
            return new(null, false, promptResponse);
        }

        var rawValue = _sessionFieldStore.GetRawValue(fieldKey);
        if (rawValue is { ValidationResult: true }) return new(rawValue.Value, true, null);

        if (_inputSupplied == false)
            throw new
                InvalidOperationException($"{nameof(WithInput)} must be called on the builder before calling {nameof(RequestField)}");

        var isFieldValid = ValidateFieldValue(_suppliedInput, out var validationFailureResult);
        await _sessionFieldStore.UpdateFieldValue(fieldKey, new()
        {
            Value = _suppliedInput,
            ValidationResult = isFieldValid
        });
        return !isFieldValid ? validationFailureResult : new(_suppliedInput, true, null);
    }


    private bool ValidateFieldValue(string? rawValue, out FieldRequestResult validationFailureResult)
    {
        foreach (var validationRule in _fieldRequest.ValidationRules)
        {
            var isValid = validationRule.Validate(rawValue);
            if (isValid) continue;

            var promptResponse = _sessionFieldStore.Prompt(validationRule.ValidationFailureMessage);
            {
                validationFailureResult = new(rawValue, false, promptResponse);
                return true;
            }
        }

        validationFailureResult = null!;
        return false;
    }
}
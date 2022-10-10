using FluentValidation;
using MessageInteractionService.Api.Endpoints.Models;

namespace MessageInteractionService.Api.Endpoints.Validation;

public class IncomingMessageValidator : AbstractValidator<IncomingMessage>
{
    public IncomingMessageValidator()
    {
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.Sender).NotEmpty();
    }
}
using FastEndpoints;
using FluentValidation;
using MessageInteractionService.Api.Endpoints.Models;

namespace MessageInteractionService.Api.Endpoints.Validation;

public class AddMenuItemRequestValidator : Validator<AddMenuItemRequest>
{
    public AddMenuItemRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty()
                             .MaximumLength(50);
    }
}
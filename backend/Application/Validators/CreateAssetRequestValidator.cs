using FluentValidation;
using backend.DTOs.Assets; 

namespace backend.Validators;

public class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Domain)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Type)
            .IsInEnum();
    }
}
using FluentValidation;
using backend.DTOs.Assets;

namespace backend.Validators;

public class UpdateAssetRequestValidator : AbstractValidator<UpdateAssetRequest>
{
    public UpdateAssetRequestValidator()
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

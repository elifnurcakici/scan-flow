using FluentValidation;
using backend.DTOs.Scans;

namespace backend.Validators;

public class CreateScanRequestValidator : AbstractValidator<CreateScanRequest>
{
    public CreateScanRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.AssetId)
            .GreaterThan(0);
    }
}

using FluentValidation;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Domain.Validators
{
    public class RedactionValidator : AbstractValidator<Redaction>
    {
        public RedactionValidator()
        {
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Width).GreaterThan(0);
            RuleFor(x => x.Height).GreaterThan(0);
            RuleFor(x => x.RedactionCoordinates).NotEmpty();
            RuleForEach(x => x.RedactionCoordinates).SetValidator(new RedactionCoordinatesValidator());
        }
    }
}

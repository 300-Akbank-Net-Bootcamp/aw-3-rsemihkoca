using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateContactValidator : AbstractValidator<ContactRequest>
{
    public CreateContactValidator()
    {
        
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.ContactType).NotEmpty().MaximumLength(50);
        
        When(x => x.ContactType.ToUpper() == "EMAIL", () =>
        {
            RuleFor(x => x.Information).NotEmpty().EmailAddress().WithMessage("Invalid email address format");
        });

        When(x => x.ContactType.ToUpper() == "PHONE", () =>
        {
            RuleFor(x => x.Information).NotEmpty().Matches(@"^((?:[0-9]\-?){6,14}[0-9])|((?:[0-9]\x20?){6,14}[0-9])$").WithMessage("Invalid phone number format");
        });
        
        RuleFor(x => x.IsDefault).NotEmpty();

    }
}
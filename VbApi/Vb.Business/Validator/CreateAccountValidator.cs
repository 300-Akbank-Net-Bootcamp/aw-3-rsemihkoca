using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateAccountValidator : AbstractValidator<AccountRequest>
{
    public CreateAccountValidator()
    {
        
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.IBAN).NotEmpty().MaximumLength(26);
        RuleFor(x => x.Balance).NotEmpty();
        RuleFor(x => x.CurrencyType).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);

    }
}
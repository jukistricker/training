using FluentValidation;

namespace BankAccountSimulation.ViewModels.Validators;

public class LoginAccountValidator : AbstractValidator<LoginAccountViewModel>
{
    public LoginAccountValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.")
            .Length(6, 20).WithMessage("Account number must be between 6 and 20 characters long. ")
            .Matches(@"^[A-Z0-9]*$").WithMessage("Account number can only include uppercase letters and numbers.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 20).WithMessage("Password must be between 6 and 20 characters long.");
    }
}

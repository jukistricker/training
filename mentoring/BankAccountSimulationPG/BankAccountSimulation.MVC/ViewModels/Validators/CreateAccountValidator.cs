using FluentValidation;

namespace BankAccountSimulation.ViewModels.Validators;

public class CreateAccountValidator : AbstractValidator<CreateAccountViewModel>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.")
            .Length(6, 20).WithMessage("Account number must be between 6 and 20 characters long. ")
            .Matches(@"^[A-Z0-9]*$").WithMessage("Account number can only include uppercase letters and numbers.");

        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Owner name is required.")
            .MinimumLength(3).WithMessage("Owner name is too short.");

        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Initial balance can not be negative.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 20).WithMessage("Password must be between 6 and 20 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirmation password is required.")
            .Equal(x => x.Password).WithMessage("Confirmation password doesn't match.");
    }
}

using FluentValidation;

namespace BankAccountSimulation.ViewModels.Validators;

public class TransactionValidator : AbstractValidator<TransactionViewModel>
{
    public TransactionValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.")
            .Length(6, 20).WithMessage("Account number must be between 6 and 20 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.")
            .Must(x => decimal.GetBits(x)[3] >> 16 <= 2)
            .WithMessage("Amount cannot have more than 2 decimal places.");

        RuleFor(x => x.Description)
            .MaximumLength(150).WithMessage("Description is limited to 150 characters.");
    }
}

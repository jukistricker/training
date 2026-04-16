using FluentValidation;

namespace BankAccountSimulation.ViewModels.Validators;

public class TransferValidator : AbstractValidator<TransferViewModel>
{
    public TransferValidator()
    {
        RuleFor(x => x.SourceAccountNumber)
            .NotEmpty().WithMessage("Source account number is required.")
            .Length(6, 20).WithMessage("Source account must be between 6 and 20 characters.");

        RuleFor(x => x.DestinationAccountNumber)
            .NotEmpty().WithMessage("Destination account is required.")
            .Length(6, 20).WithMessage("Destination account must be between 6 and 20 characters.")
            .NotEqual(x => x.SourceAccountNumber).WithMessage("You cannot transfer money to the same account.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.")
            .LessThanOrEqualTo(1000000000000).WithMessage("Amount can not be greater than 1000000000000")
            .Must(x => decimal.GetBits(x)[3] >> 16 <= 2)
            .WithMessage("Amount cannot have more than 2 decimal places.");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Transfer description is too long.");
    }
}

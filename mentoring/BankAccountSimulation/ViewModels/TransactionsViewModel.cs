using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulation.ViewModels
{
    public class TransactionViewModel
    {
        [Required(ErrorMessage = "Account number is required")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Please enter the amount")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }

    public class TransferViewModel
    {
        [Required(ErrorMessage = "Source account number is required")]
        public string SourceAccountNumber { get; set; }

        [Required(ErrorMessage = "Destination account is required")]
        public string DestinationAccountNumber { get; set; }

        [Required(ErrorMessage = "Please enter the amount")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
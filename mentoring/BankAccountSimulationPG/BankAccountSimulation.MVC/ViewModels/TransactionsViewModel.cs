
namespace BankAccountSimulation.ViewModels
{
    public class TransactionViewModel
    {
        public string AccountNumber { get; set; }

        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }

    public class TransferViewModel
    {
        public string SourceAccountNumber { get; set; }

        public string DestinationAccountNumber { get; set; }

        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
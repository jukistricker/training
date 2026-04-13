using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulation.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Account number is required")]
    public string AccountNumber { get; set; }

    public TransactionType Type { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
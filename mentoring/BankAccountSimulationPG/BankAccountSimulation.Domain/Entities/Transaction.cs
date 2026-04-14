using BankAccountSimulation.Domain.Enums;

namespace BankAccountSimulation.Domain.Entities;

public class Transaction:BaseEntity
{
    public string AccountNumber { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}

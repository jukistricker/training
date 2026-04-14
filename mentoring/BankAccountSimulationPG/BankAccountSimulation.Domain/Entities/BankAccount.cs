using BankAccountSimulation.Domain.Enums;

namespace BankAccountSimulation.Domain.Entities;

public class BankAccount:BaseEntity
{
    public string AccountNumber { get; set; }

    public string OwnerName { get; set; }

    public string PasswordHash { get; set; }

    public string Role { get; set; } // "Admin", "User"

    public decimal Balance { get; set; }

    public AccountStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}

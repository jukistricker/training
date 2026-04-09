using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank_simulation;

public class BankAccount
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; set; }
    public AccountStatus Status { get; set; }
    public Role Role { get; set; }
    public DateTime CreatedAt { get; set; }= DateTime.Now;
    public DateTime? LastInterestDate { get; set; }
    public static BankAccount FromCsvRow(string[] parts)
    {
        return new BankAccount
        {
            AccountNumber = parts[0],
            OwnerName = parts[1],
            Balance = decimal.Parse(parts[2]),
            Status = Enum.Parse<AccountStatus>(parts[3]),
            Role = Enum.Parse<Role>(parts[4]),
            CreatedAt = DateTime.Parse(parts[5]),
            LastInterestDate = string.IsNullOrWhiteSpace(parts[6]) ? (DateTime?)null : DateTime.Parse(parts[6])
        };
    }

    public string ToCsvRow() => $"{AccountNumber},{OwnerName},{Balance},{Status},{Role},{CreatedAt:O},{(LastInterestDate.HasValue ? LastInterestDate.Value.ToString("O") : "")}";
}

public class Transaction
{
    public int TransactionId { get; set; }
    public string AccountNumber { get; set; }
    public TransactionType Type { get; set; } // Deposit, Withdraw, Transfer
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public static Transaction FromCsvRow(string[] parts)
    {
        return new Transaction
        {
            TransactionId = int.Parse(parts[0]),
            AccountNumber = parts[1],
            Type = Enum.Parse<TransactionType>(parts[2]),
            Amount = decimal.Parse(parts[3]),
            Date = DateTime.Parse(parts[4]),
            Description = parts[5]
        };
    }

    public string ToCsvRow() => $"{TransactionId},{AccountNumber},{Type},{Amount},{Date:O},{Description}";
}

public enum TransactionType
{
    Deposit,
    Withdraw,
    Transfer
}

public enum AccountStatus
{
    Active,
    Frozen
}

public enum Role
{
    Admin,
    Customer
}


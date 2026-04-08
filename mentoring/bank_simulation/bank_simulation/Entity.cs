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
}

public class Transaction
{
    public int TransactionId { get; set; }
    public string AccountNumber { get; set; }
    public TransactionType Type { get; set; } // Deposit, Withdraw, Transfer
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
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


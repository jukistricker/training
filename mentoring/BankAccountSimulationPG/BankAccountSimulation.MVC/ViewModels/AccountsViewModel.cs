
namespace BankAccountSimulation.ViewModels;

public class AccountsViewModel
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public string Role { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } 
    public DateTimeOffset CreatedAt { get; set; }
}

public class CreateAccountViewModel
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public decimal InitialBalance { get; set; }
}

public class LoginAccountViewModel
{
    public string AccountNumber { get; set; }
    public string Password { get; set; }
}

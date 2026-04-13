using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulation.ViewModels;

public class AccountsViewModel
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public string Role { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } 
    public DateTime CreatedAt { get; set; }
}

public class CreateAccountViewModel
{
    [Required(ErrorMessage = "Account number is required")]
    [StringLength(20)]
    public string AccountNumber { get; set; }

    [Required(ErrorMessage = "Owner name is requierd")]
    public string OwnerName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirmation password doesn't match.")]
    public string ConfirmPassword { get; set; }

    [Range(100, double.MaxValue, ErrorMessage = "Minimum initial balance is 100.")]
    public decimal InitialBalance { get; set; }
}

public class LoginAccountViewModel
{
    [Required(ErrorMessage = "Account number is required")]
    [StringLength(20)]
    public string AccountNumber { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

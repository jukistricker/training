using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulation.Models;

public class BankAccount
{
    [Required(ErrorMessage = "Account number is required")]
    [StringLength(20, ErrorMessage = "Account number must not exceed 20 characters")]
    public string AccountNumber { get; set; }

    [Required(ErrorMessage = "Owner name is required")]
    public string OwnerName { get; set; }

    public string PasswordHash { get; set; }

    public string Role { get; set; } // "Admin", "User"

    [Range(0, double.MaxValue, ErrorMessage = "Balance must be greater than or equal to 0")]
    public decimal Balance { get; set; }

    public AccountStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
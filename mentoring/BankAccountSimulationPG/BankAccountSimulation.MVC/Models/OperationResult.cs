namespace BankAccountSimulation.Models;

public class OperationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorKey { get; set; }

    public static OperationResult Success() => new() { IsSuccess = true };
    public static OperationResult Fail(string message, string? key = null)
        => new() { IsSuccess = false, ErrorMessage = message, ErrorKey = key };
}
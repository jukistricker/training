using BankAccountSimulation.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BankAccountSimulation.Data;

public class JsonDbContext
{
    private readonly string _accountsPath;
    private readonly string _transactionsPath;
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    public List<BankAccount> Accounts { get; private set; } = new();
    public List<Transaction> Transactions { get; private set; } = new();

    public JsonDbContext(IWebHostEnvironment env)
    {
        var dataFolder = Path.Combine(env.ContentRootPath, "Data");
        _accountsPath = Path.Combine(dataFolder, "bank_accounts.json");
        _transactionsPath = Path.Combine(dataFolder, "transactions.json");

        if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);

        LoadAllData();
    }

    private void LoadAllData()
    {
        Accounts = LoadFromFile<BankAccount>(_accountsPath);
        Transactions = LoadFromFile<Transaction>(_transactionsPath);
        if (!Accounts.Any())
        {
            SeedAdmin();
        }
    }

    private List<T> LoadFromFile<T>(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "[]");
            return new List<T>();
        }
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public void SaveAccounts()
    {
        SaveToFile(_accountsPath, Accounts);
    }

    public void SaveTransactions()
    {
        SaveToFile(_transactionsPath, Transactions);
    }

    private void SaveToFile<T>(string path, List<T> data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        File.WriteAllText(path, json);
    }

    private void SeedAdmin()
    {
        var adminAccount = new BankAccount
        {
            AccountNumber = "ADMIN001",
            OwnerName = "System Administrator",
            Balance = 0,
            PasswordHash = HashPassword("admin123"),
            Role = "Admin", 
            Status = AccountStatus.Active,
            CreatedAt = DateTime.Now
        };

        Accounts.Add(adminAccount);
        SaveAccounts();
    }

    public string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
using System;
using System.Transactions;
namespace bank_simulation;

public class Program
{
    public static readonly string bankAccountsPath = "bank_accounts.csv";
    public static readonly string transactionsPath = "transactions.csv";
    public static readonly string bankAccountsFields = "AccountNumber,OwnerName,Balance,AccountStatus,Role,CreatedAt,LastInterestDate";
    public static readonly string transactionsFields = "ID,AccountNumber,Type,Amount,Date,Description";
    public static readonly string adminPin = "01234";
    const decimal DAILY_LIMIT = 50000;
    public static List<BankAccount> Accounts = [];
    public static List<Transaction> Transactions = [];

    static bool isRunning = true;
    public static void Main(string[] args)
    {
        CreateEntityInCsv(bankAccountsPath, bankAccountsFields);
        CreateEntityInCsv(transactionsPath, transactionsFields);
        LoadData();
        SeedAdminData();
        _ = RunBackgroundMonthlyInterestAsync(); //loại bỏ warning và Unobserved Task Exception
        while (isRunning)
        {
            RunApp();
        }
    }
    public static void RunApp()
    {
        Dashboard();
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                CreateAccount();
                break;
            case "2":
                Deposit();
                break;
            case "3":
                Withdraw();
                break;
            case "4":
                Transfer();
                break;
            case "5":
                ViewAccountDetails();
                break;
            case "6":
                ViewTransactionHistory();
                break;
            case "7":
                ChangeAccountStatus();
                break;
            case "8":
                Console.WriteLine("Exiting...");
                isRunning = false;
                break;
            default:
                WriteColor("Invalid choice. Please try again.", ConsoleColor.Red);
                break;
        }
    }
    public static void CreateEntityInCsv(string filePath, string fields)
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllLines(filePath, new[] { fields });
        }
    }


    public static void SeedAdminData()
    {
        string adminAccNo = "ADMIN001";

        var existingAdmin = Accounts.FirstOrDefault(a => a.AccountNumber == adminAccNo);

        if (existingAdmin == null)
        {
            var adminAcc = new BankAccount
            {
                AccountNumber = adminAccNo,
                OwnerName = "System Administrator",
                Balance = 99999999999, 
                Status = AccountStatus.Active,
                Role = Role.Admin,
                CreatedAt = DateTime.Now
            };

            Accounts.Add(adminAcc);
            _=SaveDataAsync();
        }
    }

    public static void LoadData()
    {
        if (File.Exists(bankAccountsPath))
        {
            Accounts = File.ReadAllLines(bankAccountsPath)
                           .Skip(1) 
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .Select(line => BankAccount.FromCsvRow(line.Split(',')))
                            .ToList();
        }

        if (File.Exists(transactionsPath))
        {
            Transactions = File.ReadAllLines(transactionsPath)
                               .Skip(1)
                                .Where(line => !string.IsNullOrWhiteSpace(line))
                                .Select(line => Transaction.FromCsvRow(line.Split(',')))
                                .ToList();
        }
    }

    public static async Task SaveDataAsync()
    {
        try
        {
            var accLines = Accounts.Select(a => a.ToCsvRow()).Prepend(bankAccountsFields);
            await File.WriteAllLinesAsync(bankAccountsPath, accLines);

            var transLines = Transactions.Select(t => t.ToCsvRow()).Prepend(transactionsFields);
            await File.WriteAllLinesAsync(transactionsPath, transLines);
        }
        catch (Exception ex)
        {
            await File.AppendAllTextAsync("system_log.txt", $"{DateTime.Now}: Save Error - {ex.Message}\n");
        }
    }

    public static void Dashboard()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" === Bank Account System ===");
        Console.WriteLine("""
            1. Create new account
            2. Deposit money
            3. Withdraw money
            4. Transfer money
            5. View account details
            6. View transaction history
            7. Freeze / Unfreeze account
            8. Exit
            """);
        Console.ResetColor();
        Console.Write("Enter your choice: ");
    }

    public static void CreateAccount()
    {
        Console.Write("Enter account number: ");
        string accNo = Console.ReadLine();
        if(string.IsNullOrWhiteSpace(accNo))
        {
            WriteColor("Error: Account number cannot be empty!", ConsoleColor.Red);
            return;
        }
        if (Accounts.FirstOrDefault(a => a.AccountNumber == accNo) != null)
        {
            WriteColor("Error: Account number must be unique!", ConsoleColor.Red);
            return;
        }

        Console.Write("Enter owner name: ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            WriteColor("Error: Owner name cannot be empty!", ConsoleColor.Red);
            return;
        }

        Console.Write("Initial balance: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal balance) || balance < 0)
        {
            WriteColor("Error: Initial balance must be >= 0!", ConsoleColor.Red);
            return;
        }

        var newAcc = new BankAccount
        {
            AccountNumber = accNo,
            OwnerName = name,
            Balance = balance,
            Status = AccountStatus.Active,
            Role = Role.Customer,
            CreatedAt = DateTime.Now
        };
        Accounts.Add(newAcc);
        _ = SaveDataAsync();
        Console.WriteLine("Account created successfully.");
    }

    // 5.2 Deposit Money
    public static void Deposit()
    {
        var acc = FindAccount(); 
        if (acc == null || IsFrozen(acc)) return;

        Console.Write("Enter amount to deposit: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
        {
            acc.Balance += amount;

            _ = LogTransactionAsync(acc.AccountNumber, TransactionType.Deposit, amount, "Deposit to account");
            _ = SaveDataAsync();

            Console.WriteLine($"Deposit successful. New Balance: {acc.Balance:N0}");
        }
        else WriteColor("Invalid amount.", ConsoleColor.Red);
    }

    // 5.3 Withdraw Money
    public static void Withdraw()
    {
        var acc = FindAccount();
        if (acc == null || IsFrozen(acc)) return;

        Console.Write("Enter amount to withdraw: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
        {
            if (acc.Balance - amount < 100)
            {
                WriteColor("Insufficient funds.", ConsoleColor.Red);
                return;
            }
            if (acc.Role != Role.Admin && IsOverDailyLimit(acc.AccountNumber, amount)) return;

            acc.Balance -= amount;
            _ = LogTransactionAsync(acc.AccountNumber, TransactionType.Withdraw, amount, "Withdraw from account");

            _ = SaveDataAsync();

            Console.WriteLine($"Withdraw successful. Remaining Balance: {acc.Balance:N0}");
        }
        else WriteColor("Invalid amount.", ConsoleColor.Red);
    }

    // 5.4 Transfer Money
    public static void Transfer()
    {
        Console.WriteLine("Source Account:");
        var source = FindAccount();
        if (source == null || IsFrozen(source)) return;

        Console.WriteLine("Destination Account:");
        var dest = FindAccount();
        if (dest == null) return;

        if (dest.AccountNumber == source.AccountNumber)
        {
            WriteColor("Error: Cannot transfer to the same account.", ConsoleColor.Red);
            return;
        }

        Console.Write("Enter amount to transfer: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
        {
            if (amount > source.Balance || source.Balance - amount < 100)
            {
                WriteColor("Insufficient funds.", ConsoleColor.Red);
                return;
            }
            source.Balance -= amount;
            dest.Balance += amount;

            _=LogTransactionAsync(source.AccountNumber, TransactionType.Transfer, amount, $"Transfer to {dest.AccountNumber}");
            _ = LogTransactionAsync(dest.AccountNumber, TransactionType.Transfer, amount, $"Transfer from {source.AccountNumber}");

            _ = SaveDataAsync();

            Console.WriteLine("Transfer completed successfully.");
        }
    }

    // 5.5 View Account Details
    public static void ViewAccountDetails()
    {
        var acc = FindAccount();
        if (acc == null) return;

        Console.WriteLine("\n--- Account Details ---");
        Console.WriteLine($"Account Number: {acc.AccountNumber}");
        Console.WriteLine($"Owner : {acc.OwnerName}");
        Console.WriteLine($"Balance: {acc.Balance:N0}");
        Console.WriteLine($"Status : {acc.Status}");
        Console.WriteLine($"Created: {acc.CreatedAt}");
    }

    // 5.6 View Transaction History
    public static void ViewTransactionHistory()
    {
        var acc = FindAccount();
        if (acc == null) return;

        bool isAdmin = acc.Role == Role.Admin;

        Console.WriteLine("Filter: 1. All | 2. Deposits | 3. Withdraws");
        string filter = Console.ReadLine();
        var logs = isAdmin
            ? Transactions.OrderByDescending(t => t.Date).ToList()
            : Transactions.Where(t => t.AccountNumber == acc.AccountNumber).OrderByDescending(t => t.Date).ToList();

        if (filter == "2") logs = logs.Where(t => t.Type == TransactionType.Deposit).ToList();
        if (filter == "3") logs = logs.Where(t => t.Type == TransactionType.Withdraw).ToList();

        string header = isAdmin
            ? "Date             | Acc No  | Type       | Amount     | Description"
            : "Date             | Type       | Amount     | Description";

        Console.WriteLine($"\n{header}");
        Console.WriteLine(new string('-', header.Length));

        foreach (var t in logs)
        {
            string row = isAdmin
                ? $"{t.Date:yyyy-MM-dd HH:mm} | {t.AccountNumber,-7} | {t.Type,-10} | {t.Amount,10:N0} | {t.Description}"
                : $"{t.Date:yyyy-MM-dd HH:mm} | {t.Type,-10} | {t.Amount,10:N0} | {t.Description}";

            Console.ForegroundColor = t.Type switch
            {
                TransactionType.Deposit => ConsoleColor.DarkGreen,
                TransactionType.Withdraw => ConsoleColor.Red,
                _ => ConsoleColor.Cyan
            };

            Console.WriteLine(row);
            Console.ResetColor();
        }
    }

    // 5.7 Freeze / Unfreeze
    public static void ChangeAccountStatus()
    {
        var acc = FindAccount();
        if (acc == null) return;

        Console.WriteLine($"Current status: {acc.Status}");
        Console.WriteLine("1. Freeze | 2. Unfreeze");
        string op = Console.ReadLine();

        acc.Status = (op == "1") ? AccountStatus.Frozen : AccountStatus.Active;
        _ = SaveDataAsync();
        Console.WriteLine($"Account status updated to {acc.Status}.");
    }

    //monthly interest calculation
    public static async Task RunBackgroundMonthlyInterestAsync()
    {
        while (true)
        {
            try
            {
                bool hasChanges = false;
                foreach (var acc in Accounts)
                {
                    if (acc.Role != Role.Customer || acc.Status != AccountStatus.Active) continue;

                    bool isOldEnough = (DateTime.Now - acc.CreatedAt).TotalDays >= 30;
                    bool monthAlreadyPaid = acc.LastInterestDate?.Month == DateTime.Now.Month &&
                                            acc.LastInterestDate?.Year == DateTime.Now.Year;

                    if (isOldEnough && !monthAlreadyPaid && acc.Balance > 0)
                    {
                        decimal interest = acc.Balance * 0.005m;
                        acc.Balance += interest;
                        acc.LastInterestDate = DateTime.Now;

                        await LogTransactionAsync(acc.AccountNumber, TransactionType.Deposit, interest, "Auto Monthly Interest");
                        WriteColor($"Interest added to {acc.AccountNumber}.", ConsoleColor.Cyan);

                        hasChanges = true;
                    }
                    else
                    {
                        WriteColor($"Monthly interest skipped for account {acc.AccountNumber}.", ConsoleColor.Magenta);
                    }
                }

                if (hasChanges) await SaveDataAsync();
            }
            catch (Exception ex)
            {
                File.AppendAllText("system_log.txt", $"{DateTime.Now}: Interest Error - {ex.Message}\n");
            }

            await Task.Delay(TimeSpan.FromHours(12));
        }
    }

    //daily withdrawal limit check
    public static bool IsOverDailyLimit(string accNo, decimal requestAmount)
    {
        var todayTransactions = Transactions.Where(t => t.AccountNumber == accNo &&
                        t.Date.Date == DateTime.Today &&
                        (t.Type == TransactionType.Withdraw));

        decimal totalSpentToday = todayTransactions.Sum(t => t.Amount);

        if (totalSpentToday + requestAmount > DAILY_LIMIT)
        {
            WriteColor($"Error: Daily limit exceeded! You can only withdraw/transfer {DAILY_LIMIT - totalSpentToday:N0} more today.", ConsoleColor.Red);
            return true;
        }
        return false;
    }

    public static BankAccount FindAccount()
    {
        Console.Write("Enter Account Number: ");
        string no = Console.ReadLine();
        var acc = Accounts.FirstOrDefault(a => a.AccountNumber == no);
        if (acc == null)
        {
            WriteColor("Account not found.", ConsoleColor.Red);
            return null;
        }
        if(acc.Role ==Role.Admin)
            {
                if (!IsAdmin(acc)) return null;
        }
        return acc;
    }

    public static bool IsAdmin(BankAccount acc)
    {
        WriteColor("Please enter PIN to proceed: ", ConsoleColor.Yellow);
        string pin = Console.ReadLine();
        if (pin != adminPin)
        {
            WriteColor("Incorrect PIN. Operation not allowed.", ConsoleColor.Red);
            return false;
        }
        return true;
    }

    public static bool IsFrozen(BankAccount acc)
    {
        if (acc.Status == AccountStatus.Frozen)
        {
            WriteColor("This account is frozen. Operation not allowed.", ConsoleColor.Red);
            return true;
        }
        return false;
    }

    public static async Task LogTransactionAsync(string accNo, TransactionType type, decimal amount, string desc)
    {
        int nextId = Transactions.Count + 1;
        var trans = new Transaction
        {
            TransactionId = nextId,
            AccountNumber = accNo,
            Type = type,
            Amount = amount,
            Date = DateTime.Now,
            Description = desc
        };
        Transactions.Add(trans);
        await SaveDataAsync(); // Cần await ở đây
    }

    public static void WriteColor(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}


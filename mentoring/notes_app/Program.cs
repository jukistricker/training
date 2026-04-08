using static System.Net.Mime.MediaTypeNames;

public class Program
{
    public static void Main(string[] args)
    {
        Program myApp = new Program();
        string filePath = "notes.csv";
        myApp.createCsv(filePath);
        bool isRunning = true;
        while (isRunning) {
            myApp.Dashboard();
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    myApp.AddNote(filePath);
                    break;
                case "2":
                    myApp.handleReadNotes(filePath);
                    break;
                case "3":
                    Console.WriteLine("Mark note as done");
                    break;
                case "4":
                    Console.WriteLine("Delete a note by ID");
                    break;
                case "5":
                    Console.WriteLine("Clear all notes");
                    break;
                case "6":
                    Console.WriteLine("Exiting...");
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        
        
    }

    public class Note
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public string Status { get; set; } 
        public string Content { get; set; }
    }

    public void Dashboard()
    {
        Console.WriteLine("=== Notes Application ===");
        Console.WriteLine("1.Add a new note");
        Console.WriteLine("2.View notes");
        Console.WriteLine("3.Mark note as done");
        Console.WriteLine("4.Delete a note by ID");
        Console.WriteLine("5.Clear all notes");
        Console.WriteLine("6.Exit");
        Console.Write("Enter your choice: ");
    }

    public void createCsv(string filePath)
    {
        if (!File.Exists(filePath))
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine("ID,CreatedAt,IsDone,Content");
            }
        }
    }

    public void AddNote(string filePath)
    {
        Console.WriteLine("--- Add New Note ---");
        Console.Write("Enter content: ");
        string content = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(content))
        {
            Console.WriteLine("Content cannot be empty!");
            return;
        }

        int nextId = 1;
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            if (lines.Length > 1)
            {
                var lastLine = lines[lines.Length - 1];
                if (int.TryParse(lastLine.Split(',')[0], out int lastId))
                {
                    nextId = lastId + 1;
                }
            }
        }

        string createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string status = "NOT_DONE";

        string escapedContent = content.Replace("\"", "\"\"");

        string csvLine = $"{nextId},{createdAt},{status},\"{escapedContent}\"";

        File.AppendAllLines(filePath, new[] { csvLine });

        Console.WriteLine("Note added successfully!");
    }

    public void handleReadNotes(string filePath) {         
        Console.WriteLine("Choose reading options:");
        Console.WriteLine("1. View all notes");
        Console.WriteLine("2. View only not done notes");
        string input = Console.ReadLine();
        bool onlyNotDone = input.Trim().ToLower() == "2";
        ReadNotes(filePath, onlyNotDone);
    }

    public void ReadNotes(string filePath, bool onlyNotDone = false)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("No notes found.");
            return;
        }

        var lines = File.ReadAllLines(filePath);
        if (lines.Length <= 1)
        {
            Console.WriteLine("No notes found.");
            return;
        }

        int pageSize = 50;
        int displayCount = 0; 
        string title = onlyNotDone ? "--- Not Done Notes List ---" : "--- All Notes List ---";
        Console.WriteLine(title);

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            if (parts.Length >= 4)
            {
                string status = parts[2].Trim();

                if (onlyNotDone && status == "DONE") continue;

                displayCount++;
                string content = parts[3].Trim('"');

                Console.ForegroundColor = (status == "DONE") ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"[ID: {parts[0]}] [{parts[1]}] [Status: {status}]");
                Console.ResetColor();
                Console.WriteLine($"{content}");
                Console.WriteLine("----------------------------------");

                if (displayCount % pageSize == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n--- Nhấn ENTER để xem tiếp... ---");
                    Console.ResetColor();
                    Console.ReadLine();
                    Console.Clear();
                    Console.WriteLine($"{title} (Tiếp theo)");
                }
            }
        }

        if (displayCount == 0)
        {
            Console.WriteLine("No notes found matching your filter.");
        }
    }



}
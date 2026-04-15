# 🚀 Project Execution Guide: BankAccountSimulation

This guide provides step-by-step instructions to set up, migrate, and run the BankAccountSimulation project from the root directory.

---

## 1. Prerequisites
* **.NET 10.0 SDK** (or the latest version installed on your machine).
* **PostgreSQL Instance**: Ensure a database is created and accessible.
* **EF Core Tools**: Install globally via:
  `dotnet tool install --global dotnet-ef`

---

## 2. Database Configuration
Before launching, update the connection string in the following file:
`BankAccountSimulation.MVC/appsettings.Development.json`

Ensure it points to your local PostgreSQL instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost; Port=5432; Database=public; Username=postgres; Password=postgres"
}
```
## 3. Deployment Steps (From Project Root)
Open your terminal in the folder containing the .sln file and execute the following commands:

#### Step 1: Apply Database Migrations
This command creates the necessary tables, indexes, and constraints in your PostgreSQL database:
```bash
dotnet ef database update -p BankAccountSimulation.Infra -s BankAccountSimulation.MVC
```
<b>Don't worry if the message is like this, it's just ef core's pre-check:</b>

![alt text](image.png)


### Step 2: Launch the Application
Start the MVC web interface:

```bash
dotnet run --project BankAccountSimulation.MVC
```
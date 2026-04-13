
# 📝 Project Progress Report: Bank Account Simulation System

This document summarizes the milestones achieved during the development of the **Bank Account Simulation System**, built on **ASP.NET Core 8.0 MVC**. The project focuses on a clean architecture, robust transaction logic, and a modern user experience.

---

## 🚀 Completed Features

### 1. Account Management
* **Account Dashboard:** A centralized view displaying all active accounts using professional Bootstrap cards.
* **Account Details:** Dedicated view for each account showing real-time balances, account status (Active/Frozen), and owner information.
* **Dynamic Routing:** Seamless navigation between the account list and specific transaction forms using `accountNumber` parameters.

### 2. Transaction Engine
We have implemented a centralized `TransactionsService` to handle core banking logic:
* **Deposit:** Securely add funds to any account with customizable descriptions.
* **Withdrawal:** * Automated balance verification.
    * **Minimum Balance Rule:** Logic ensures accounts maintain at least **100₫** after any withdrawal.
* **Transfer:** * Dual-account processing (Source and Destination).
    * Atomic operations to ensure data integrity during funds movement.
* **Transaction History:** A comprehensive log for each account, tracking type, amount, date, and description.

### 3. UI/UX & Global Layout
* **Modern Sidebar:** A persistent navigation bar for quick access to Dashboard, Transactions, and History.
* **Theme Engine:** Integrated **Dark/Light Mode** switcher with persistence using `localStorage`.
* **Responsive Design:** Fully mobile-friendly layout with a toggleable sidebar for smaller screens.
* **Real-time Validation:** Client-side and Server-side validation to prevent invalid amounts or missing account data.

---

## 🛠 Tech Stack & Architecture

### Backend
* **C# / ASP.NET Core MVC:** Core framework for routing and logic.
* **ViewModel Pattern:** Utilized `TransactionViewModel` and `TransferViewModel` to separate UI concerns from data models.
* **JSON Persistence:** Lightweight data storage using local JSON files, ensuring data survives application restarts.
* **Dependency Injection (DI):** Decoupled services for better maintainability and testing.

### Frontend
* **Bootstrap 5:** For layout, components, and responsive grid system.
* **Bootstrap Icons:** For intuitive visual cues across the application.
* **Razor Syntax:** For dynamic content rendering and secure form handling (`@Html.AntiForgeryToken`).

---

## 🛡 Business Logic & Safety Measures
1.  **Overdraft Protection:** Systems prevent any transaction that exceeds the available balance (minus the 100₫ reserve).
2.  **Anti-Forgery Protection:** All POST requests are protected with tokens to prevent CSRF attacks.
3.  **Automatic Data Mapping:** Controllers are optimized to "remember" the active account number from the URL, reducing user input errors.
4.  **Error Handling:** Custom error messages integrated into the UI to guide users through failed validations.

---

## 📅 Latest Updates
* **Fixed:** Model Mismatch errors between Transaction and Transfer views.
* **Improved:** Navigation flow from "Confirm" buttons back to the Account Details page.
* **Added:** Professional breadcrumb navigation for better user orientation.

---
*End of Report*

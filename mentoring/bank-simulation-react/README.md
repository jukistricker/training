# 🏦 Bank Simulation - React Redux Toolkit

A modern banking simulation application featuring secure authentication, real-time balance management, transaction history tracking, and interactive financial analytics.

## 📋 Requirements

Before setting up the project, ensure you have the following installed:

* **Node.js**: version `18.x` or higher (LTS recommended)
* **npm**: version `9.x` or higher
* **Browser**: Latest versions of Chrome, Edge, or Firefox

---

## 🚀 Quick Start (Recommended)

The project is configured with `concurrently`, allowing you to boot up both the **Backend (JSON Server)** and the **Frontend (Vite)** with a single command.

1.  **Install dependencies:**
    ```bash
    npm install
    ```

2.  **Run the full system:**
    ```bash
    npm start
    ```
    * **Frontend**: Accessible at [http://localhost:5173](http://localhost:5173)
    * **JSON Server (API)**: Accessible at [http://localhost:5000](http://localhost:5000)

---

## 🛠 Manual Execution

If you prefer to run the services in separate terminal windows for easier debugging:

### Step 1: Start the Database (JSON Server)
```bash
npm run server
```
This command watches db.json and hosts the REST API at port 5000.

Step 2: Start the Web Application (Vite)
```bash
npm run dev
```

```bash
D:\...\BANK-SIMULATION-REACT\SRC
│   App.tsx                 # Root Component - Entry point for Providers (Redux, Router, etc.)
│   ErrorBoundary.tsx       # Global Error Handling - Catches and handles UI crashes
│   global.css              # Global Styles - CSS resets, Bootstrap overrides, and shared styles
│   router.tsx              # Routing Configuration - Defines main application routes and URL mapping
│
├───assets                  # Static Assets - Images (png, svg), icons, and custom fonts
│
├───components              # Shared Components - Generic UI elements used across the app
│   ├───layouts             # Page Wrappers - Layout templates (AuthLayout for Login/Register, MainLayout for Dashboard)
│   └───ui                  # Reusable UI Atoms - Low-level components (Dialogs, Inputs, Buttons, Pagination)
│
├───config                  # App Configuration - Environment-specific settings
│   └───constants           # Shared Constants - API endpoints, Storage keys, and URL Route paths
│
├───features                # CORE LOGIC: Business Logic organized by domain/feature
│   ├───accounts            # Account Feature - Management of profiles, Login, and Registration
│   │   ├───components      # Feature-specific components for Accounts (Forms, Lists)
│   │   └───types           # Internal Types - TypeScript interfaces local to the Account feature
│   │
│   ├───dashboard           # Dashboard Feature - Analytics, charts, and balance summaries
│   │
│   └───transactions        # Transaction Feature - Business logic for Deposits, Withdrawals, Transfers
│       ├───components      # Transaction forms and history table components
│       └───types           # Internal Types - TypeScript interfaces local to Transactions
│
├───hooks                   # Custom Hooks - Shared logic across components (e.g., usePagination)
│
├───lib                     # External Libraries - Config for 3rd party SDKs (Axios instances, API clients)
│
├───routes                  # Access Control - Navigation guards (e.g., ProtectedRoute for Auth check)
│
├───store                   # Global State Management - Redux Toolkit implementation
│   │   store.tsx           # Central Store Config - Root reducer and middleware setup
│   ├───slices              # State Logic - Reducers for Account and Transaction states
│   └───thunks              # Async Actions - Side effects (API calls for Login, Transactions, etc.)
│
├───types                   # Shared Types - Global TypeScript interfaces and Models
│
└───utils                   # Utility Functions - Helper methods (Storage wrappers, Currency formatters, CSV Export)
```
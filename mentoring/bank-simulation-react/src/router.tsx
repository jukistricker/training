import { createBrowserRouter, Navigate } from "react-router-dom";
import { ACCOUNT_BASE, ROUTES, TRANSACTION_BASE } from "./config/constants/url_routes";
import DashboardPage from "./features/dashboard/components/dashboard";
import { ProtectedRoute } from "./routes/ProtectedRoute";
import AuthLayout from "./components/layouts/AuthLayout"; 
import { LoginForm } from "./features/accounts/components/LoginForm";
import MainLayout from "./components/layouts/MainLayout";
import { RegisterForm } from "./features/accounts/components/RegisterForm";
import { AccountDetails } from "./features/accounts/components/AccountDetails";
import { AccountList } from "./features/accounts/components/AccountList";
import { TransferForm } from "./features/transactions/components/TransferForm";
import { DepositForm } from "./features/transactions/components/DepositForm";
import { WithdrawForm } from "./features/transactions/components/WithdrawForm";

export const router = createBrowserRouter([
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <MainLayout />,
        children: [{ path: ROUTES.HOME, element: <DashboardPage /> }]
      }
    ]
  },
  {
    path: ACCOUNT_BASE, 
    children: [
      {
        element: <AuthLayout />,
        children: [
          { path: "login", element: <LoginForm /> },
          { path: "create", element: <RegisterForm /> },
        ]
      },
      
      {
        element: <ProtectedRoute />,
        children: [
          {
            element: <MainLayout />,
            children: [
              { index: true, element: <AccountList /> }, 
              { path: ":id", element: <AccountDetails /> },
            ]
          }
        ]
      }
    ]
  },
  {
    path: TRANSACTION_BASE,
    element: <ProtectedRoute />,
    children: [
      {
        element: <MainLayout />,
        children: [
          { path: "transfer", element: <TransferForm /> },
          { path: "deposit", element: <DepositForm /> },
          { path: "withdraw", element: <WithdrawForm /> },
        ]
      }
    ]
  },

  { path: "*", element: <Navigate to={ROUTES.ACCOUNT.LOGIN} replace /> },
]);
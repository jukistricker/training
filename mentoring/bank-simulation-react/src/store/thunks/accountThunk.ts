import { createAsyncThunk } from "@reduxjs/toolkit";
import CryptoJS from "crypto-js";
import { api } from "../../lib/api";
import { ROUTES } from "../../config/constants/url_routes";
import { LoginFormValues, RegisterFormValues } from "../../features/accounts/types";
import { AccountStatus, BankAccount } from "../../types/account";
import { JSON_SERVER_ACCOUNTS } from "../../config/constants/json_server_routes";
import { genericStorage } from "../../utils/storage";
import { USER_KEY } from "../../config/constants/storage_key";

const hashPassword = (password: string) => {
  const hash = CryptoJS.SHA256(password);
  return hash.toString(CryptoJS.enc.Hex).toUpperCase();
};

// <Kiểu_Trả_Về, Kiểu_Tham_Số_Truyền_Vào>
export const loginUser = createAsyncThunk<BankAccount, LoginFormValues>(
  ROUTES.ACCOUNT.LOGIN,
  async (request, { rejectWithValue }) => {
    try {
      const data = await api.get<BankAccount[]>("/accounts", { 
        account_number: request.account_number 
      });

      const account = data[0];

      if (!account) {
        return rejectWithValue({ message: "Account number does not exist", field: "account_number" });
      }

      if (account.password_hash !== hashPassword(request.password)) {
        return rejectWithValue({ message: "Invalid password", field: "password" });
      }

      genericStorage.save(USER_KEY, account);
      return account;
    } catch (error: any) {
      return rejectWithValue({ message: "Server error" });
    }
  }
);

export const registerUser = createAsyncThunk<{ success: boolean }, RegisterFormValues>(
  ROUTES.ACCOUNT.REGISTER,
  async (request, { rejectWithValue }) => {
    try {
      const existing = await api.get<BankAccount[]>("/accounts", { 
        account_number: request.account_number 
      });

      if (existing.length > 0) {
        return rejectWithValue({ message: "Account number already exists", field: "account_number" });
      }

      const newAccount: BankAccount = {
        id: "",
        account_number: request.account_number,
        owner_name: request.ownerName,
        balance: request.initialBalance,
        password_hash: hashPassword(request.password),
        role: "User",
        status: AccountStatus.Active,
        created_at: new Date().toISOString()
      };

      await api.post("/accounts", newAccount);
      return { success: true };
    } catch (error: any) {
      return rejectWithValue({ message: "Server error, please try again" });
    }
  }
);


export const fetchAccountById = createAsyncThunk<BankAccount, string>(
  "account/fetchAccountById",
  async (accountId, { rejectWithValue }) => {
    try {
      const account = await api.get<BankAccount>(`${JSON_SERVER_ACCOUNTS}/${accountId}`);
      return account;
    } catch (error: any) {
      return rejectWithValue({ message: "Account not found" });
    }
  }
);

export const toggleAccountStatus = createAsyncThunk<BankAccount, { id: string; currentStatus: number }>(
  "account/toggleStatus",
  async ({ id, currentStatus }, { rejectWithValue }) => {
    try {
      const newStatus = currentStatus === 0 ? 1 : 0; 
      const updatedAccount = await api.patch<BankAccount>(`${JSON_SERVER_ACCOUNTS}/${id}`, { 
        status: newStatus 
      });
      return updatedAccount;
    } catch (error: any) {
      return rejectWithValue({ message: "Update status failed" });
    }
  }
);

export const fetchAllAccounts = createAsyncThunk<BankAccount[], void>(
  "account/fetchAll",
  async (_, { rejectWithValue }) => {
    try {
      const data = await api.get<BankAccount[]>("/accounts");
      return Array.isArray(data) ? data : [];
    } catch (error: any) {
      return rejectWithValue({ message: "Failed to fetch accounts" });
    }
  }
);
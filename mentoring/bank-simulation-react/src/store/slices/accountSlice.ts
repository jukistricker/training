import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { BankAccount } from "../../types/account";
import {
  fetchAccountById,
  fetchAllAccounts,
  loginUser,
  registerUser,
  toggleAccountStatus,
} from "../thunks/accountThunk";
import { executeTransaction } from "../thunks/transactionThunk";
import { TransactionType } from "../../types/transaction";
import { userStorage } from "../../utils/storage";

interface AccountState {
  user: BankAccount | null;
  accounts: BankAccount[];
  loading: boolean;
  error: any;
}

const getSavedUser = (): BankAccount | null => {
  const saved = localStorage.getItem("user");
  if (!saved) return null;
  try {
    return JSON.parse(saved);
  } catch {
    return null;
  }
};

const initialState: AccountState = {
  user: getSavedUser(),
  accounts: [],
  loading: false,
  error: null,
};

const accountSlice = createSlice({
  name: "account",
  initialState,
  reducers: {
    logout: (state) => {
      state.user = null;
      localStorage.removeItem("user");
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Login flow
      .addCase(loginUser.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(
        loginUser.fulfilled,
        (state, action: PayloadAction<BankAccount>) => {
          state.loading = false;
          state.user = action.payload;
        },
      )
      .addCase(loginUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(registerUser.pending, (state) => {
        state.loading = true;
      })
      .addCase(registerUser.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(registerUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(
        fetchAccountById.fulfilled,
        (state, action: PayloadAction<BankAccount>) => {
          state.loading = false;
          if (state.user?.id === action.payload.id) {
            state.user = action.payload;
          }
        },
      )
      .addCase(fetchAllAccounts.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchAllAccounts.fulfilled, (state, action) => {
        state.loading = false;
        state.accounts = action.payload;
      })
      .addCase(fetchAllAccounts.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Toggle Status
      .addCase(toggleAccountStatus.fulfilled, (state, action: PayloadAction<BankAccount>) => {
        const updatedAcc = action.payload;
        const index = state.accounts.findIndex(a => a.id === updatedAcc.id);
        if (index !== -1) {
          state.accounts[index] = updatedAcc;
        }
        if (state.user?.id === updatedAcc.id) {
          state.user = updatedAcc;
          localStorage.setItem("user", JSON.stringify(updatedAcc));
        }
      })
      builder.addCase(executeTransaction.fulfilled, (state, action) => {
      
      if (state.user && action.meta.arg) {
        const { type, amount } = action.meta.arg; 
        
        if (type === TransactionType.Deposit) {
          state.user.balance += amount;
        } else {
          state.user.balance -= amount;
        }
        userStorage.updateBalance(state.user.balance);
      }
    });;
  },
});

export const { logout } = accountSlice.actions;
export default accountSlice.reducer;

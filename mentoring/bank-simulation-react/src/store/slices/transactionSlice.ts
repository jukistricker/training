import { createSlice } from "@reduxjs/toolkit";
import { Transaction } from "../../types/transaction";
import {
  executeTransaction,
  fetchTransactionHistory,
} from "../thunks/transactionThunk";

interface TransactionState {
  items: Transaction[];
  totalCount: number;
  loading: boolean;
  isProcessing: boolean;
  error: string | null;
}

const initialState: TransactionState = {
  items: [],
  totalCount: 0,
  loading: false,
  isProcessing: false,
  error: null,
};

const transactionSlice = createSlice({
  name: "transaction",
  initialState,
  reducers: {
    clearHistory: (state) => {
      state.items = [];
      state.totalCount = 0;
    },
    clearError: (state) => {
      state.error = null;
    },
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchTransactionHistory.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchTransactionHistory.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload.items || [];
        state.totalCount = action.payload.totalCount || 0;
      })
      .addCase(fetchTransactionHistory.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || "Failed to fetch transactions";
      })
      .addCase(executeTransaction.pending, (state) => {
        state.isProcessing = true;
        state.error = null;
      })
      .addCase(executeTransaction.fulfilled, (state) => {
        state.isProcessing = false;
        state.error = null;
      })
      .addCase(executeTransaction.rejected, (state, action) => {
        state.isProcessing = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearHistory } = transactionSlice.actions;
export default transactionSlice.reducer;

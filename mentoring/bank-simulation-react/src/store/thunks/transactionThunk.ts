import { createAsyncThunk } from "@reduxjs/toolkit";
import { TransactionType } from "../../types/transaction";
import { api } from "../../lib/api";
import { AccountStatus } from "../../types/account";
import { RootState } from "../store";
import { TransactionFormValues } from "../../features/transactions/types";
import { JSON_SERVER_ACCOUNTS } from "../../config/constants/json_server_routes";
export const fetchTransactionHistory = createAsyncThunk(
  "transaction/fetchHistory",
  async ({
    account_number,
    type,
    page,
  }: {
    account_number?: number;
    type?: TransactionType;
    page: number;
  }) => {
    const queryParams: any = {
      account_number,
      _page: page,
      _per_page: 10,
      _sort: "-created_at", 
    };

    if(account_number){
        queryParams.account_number = account_number
    }

    if (type !== undefined) {
      queryParams.transaction_type = type;
    }

    const response = await api.get<any>(`/transactions`, queryParams);

    return {
      items: response.data || [],
      totalCount: response.items || 0
    };
  }
);

export const executeTransaction = createAsyncThunk(
  "transaction/execute",
  async (payload: TransactionFormValues, { getState, rejectWithValue, dispatch }) => {
    const state = getState() as RootState;
    const sourceAcc = state.account.user;
    let action = "";
    switch (payload.type) {
      case TransactionType.Deposit:
        action = "Deposit";
        break;
      case TransactionType.Withdraw:
        action = "Withdraw";
        break;
      case TransactionType.Transfer:
        action = "Transfer";
        break;
    }

    if (!sourceAcc) return rejectWithValue({message:"Account does not exist.", field: "account_number"});
    if (sourceAcc.status === AccountStatus.Frozen) return rejectWithValue({message:"Account is frozen.", field: "account_number"});
    if (payload.amount <= 0) {
      return rejectWithValue({message:"Amount must be greater than 0.", field: "amount"});
    }
    if(payload.type === TransactionType.Transfer && payload.destinationaccount_number === undefined){
      return rejectWithValue({message:"Destination account number is required.", field: "destinationaccount_number"});
    }

    if (payload.type !== TransactionType.Deposit) {
      if (sourceAcc.balance - payload.amount < 100)
        return rejectWithValue({message:"Current balance is not enough, minimum balance is 100.", field: "amount"});
    }

    try {
  let destAcc = null;

  if (payload.type === TransactionType.Transfer) {
    console.log("--- Start Transfer Check ---");
    console.log("Searching for destination account:", payload.destinationaccount_number);

    if (!payload.destinationaccount_number) {
      return rejectWithValue({ message: "Destination account number is required.", field: "destinationaccount_number" });
    }

    const allAccs = await api.get<any[]>(`${JSON_SERVER_ACCOUNTS}`, { 
      account_number: payload.destinationaccount_number 
    });

    console.log("API Search Result:", allAccs);

    destAcc = Array.isArray(allAccs) ? allAccs[0] : null;

    if (!destAcc) {
      console.error("Validation Failed: Destination account not found in DB");
      return rejectWithValue({ message: "Destination account does not exist.", field: "destinationaccount_number" });
    }

    if (Number(destAcc.account_number) === Number(sourceAcc.account_number)) {
      console.error("Validation Failed: Source and Destination are identical");
      return rejectWithValue({ message: "Cannot transfer to the same account.", field: "destinationaccount_number" });
    }
    
    console.log("Transfer Validation OK. Destination:", destAcc.owner_name);
  }

  console.log("--- Processing Source Update ---");
  const newSourceBalance = payload.type === TransactionType.Deposit 
    ? sourceAcc.balance + payload.amount 
    : sourceAcc.balance - payload.amount;

  console.log(`New Source Balance: ${sourceAcc.balance} -> ${newSourceBalance}`);

  await api.patch(`${JSON_SERVER_ACCOUNTS}/${sourceAcc.id}`, { balance: newSourceBalance });
  console.log("Source balance patched successfully");

  await api.post('/transactions', {
    account_number: sourceAcc.account_number,
    transaction_type: payload.type,
    amount: payload.amount,
    description: payload.description?.trim() ? `${action} money, ${payload.description}` : `${action} money`,
    created_at: new Date().toISOString()
  });
  console.log("Source transaction logged");

  if (payload.type === TransactionType.Transfer && destAcc) {
    console.log("--- Processing Destination Update ---");
    await api.patch(`${JSON_SERVER_ACCOUNTS}/${destAcc.id}`, { balance: destAcc.balance + payload.amount });
    console.log("Destination balance patched successfully");

    await api.post('/transactions', {
      account_number: destAcc.account_number,
      transaction_type: TransactionType.Transfer,
      amount: payload.amount,
      description: payload.description ? `Receive money from: ${sourceAcc.account_number}, ${payload.description}` : `Receive money from: ${sourceAcc.account_number}`,
      created_at: new Date().toISOString()
    });
    console.log("Destination transaction logged");
  }

  return { success: true };
} catch (error: any) {
  console.error("--- TRANSACTION CRASHED ---");
  console.error("Error Detail:", error);
  return rejectWithValue(error.message || error || "Transaction failed");
}
  }
);
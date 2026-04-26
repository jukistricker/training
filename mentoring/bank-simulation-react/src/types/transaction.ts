export enum TransactionType {
  Deposit = 0,
  Withdraw = 1,
  Transfer = 2
}

export interface Transaction {
  id: string;
  account_number: number;
  transaction_type: TransactionType;
  amount: number;
  description: string;
  created_at: string;
}


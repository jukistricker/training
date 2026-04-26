// em định nghĩa interface ra 1 file riêng để ngầm định sẽ sử dụng giữa các component, 
// còn schema của zod thì chỉ dùng để validate dữ liệu trong component đó thôi
export interface BankAccount {
  id: string;
  account_number: number;
  owner_name: string;
  balance: number;
  role: string;
  status: AccountStatus; 
  created_at: string;
  password_hash?: string;
}

export enum AccountStatus{
  Active = 0,
  Frozen = 1
}
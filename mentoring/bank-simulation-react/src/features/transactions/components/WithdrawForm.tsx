import { useSelector } from "react-redux";
import { TransactionForm } from "./TransactionForm";
import { RootState } from "../../../store/store";

export const WithdrawForm= () => {
  const account_number = useSelector((state: RootState) => state.account.user?.account_number);

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-md-6 col-lg-4">
          <TransactionForm 
            type="withdraw" 
            initialaccount_number={account_number || 0} 
          />
        </div>
      </div>
    </div>
  );
};
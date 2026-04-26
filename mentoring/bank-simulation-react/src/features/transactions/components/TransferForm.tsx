import { useParams } from "react-router-dom";
import { TransactionForm } from "../../../components/ui/TransactionForm";
import { RootState } from "../../../store/store";
import { useSelector } from "react-redux";
import { ACCOUNT_BASE, ROUTES } from "../../../config/constants/url_routes";

export const TransferForm = () => {
  const account_number = useSelector((state: RootState) => state.account.user?.account_number);

  return (
    <div className="container py-4">
      <div className="row justify-content-center">
        <div className="col-md-8 col-lg-5">

          <TransactionForm 
            type="transfer" 
            initialaccount_number={account_number || 0} 
          />
        </div>
      </div>
    </div>
  );
};
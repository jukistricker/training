import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { fetchAccountById } from "../../../store/thunks/accountThunk";
import { RootState, AppDispatch } from "../../../store/store";
import {ActionTile} from "../../../components/ui/ActionTile";
import { ROUTES } from "../../../config/constants/url_routes";
import { TransactionHistory } from "../../transactions/components/TransactionHistory";

export const AccountDetails = () => {
  const { accountId } = useParams<{ accountId: string }>();
  const dispatch = useDispatch<AppDispatch>();
  
  const { user: account, loading } = useSelector((state: RootState) => state.account);

  useEffect(() => {
    if (accountId) {
      dispatch(fetchAccountById(accountId));
    }
  }, [accountId, dispatch]);

  if (loading) return <div className="text-center p-5"><div className="spinner-border text-primary"></div></div>;
  
  if (!account) return <div className="container mt-4"><div className="alert alert-warning">Account not found.</div></div>;

  const isFrozen = account.status === 1;

  return (
    <div className="container py-4">
      <div className="card shadow-sm border-0 mb-4 overflow-hidden">
        <div className="card-header py-3">
          <h5 className="mb-0 fw-bold">
            <i className="bi bi-info-circle me-2"></i>Account Details
          </h5>
        </div>
        <div className="card-body p-4">
          <div className="row align-items-center">
            <div className="col-md-8">
              <div className="d-flex align-items-center mb-4">
                <div className="bg-primary text-white p-3 rounded-circle me-3">
                  <i className="bi bi-wallet2 fs-2"></i>
                </div>
                <div>
                  <h2 className="fw-bold mb-0 text-primary">{account.owner_name}</h2>
                  <span className="text-muted small">Account Number: {account.account_number}</span>
                </div>
              </div>

              <div className="row g-3">
                <div className="col-6 col-sm-4">
                  <label className="text-muted small d-block">Status</label>
                  <span className={`badge ${isFrozen ? "bg-danger" : "bg-success"} rounded-pill`}>
                    {isFrozen ? "Frozen" : "Active"}
                  </span>
                </div>
                <div className="col-6 col-sm-4">
                  <label className="text-muted small d-block">Created At</label>
                  <span className="fw-bold">
                    {new Date(account.created_at).toLocaleString("en-US")}
                  </span>
                </div>
              </div>
            </div>

            <div className="col-md-4 text-md-end mt-4 mt-md-0 border-start ps-md-4">
              <label className="text-muted small d-block">Available Balance</label>
              <h1 className={`fw-bold ${account.balance < 100 ? 'text-danger': 'text-success' } mb-0`}>
                {account.balance.toLocaleString("en-US")} <small className="fs-6">$</small>
              </h1>
            </div>
          </div>
        </div>
      </div>

      <h5 className="fw-bold mb-3 text-secondary">Quick Actions</h5>
      <div className="row g-3">
        <ActionTile 
          to={ROUTES.TRANSACTIONS.DEPOSIT}
          icon="bi-plus-circle" 
          label="Deposit" 
          description="Add funds to your account"
          theme="text-success"
          disabled={isFrozen}
        />
        <ActionTile 
          to={ROUTES.TRANSACTIONS.WITHDRAW}
          icon="bi-dash-circle" 
          label="Withdraw" 
          description="Take cash out of your account"
          theme="text-danger"
          disabled={isFrozen}
        />
        <ActionTile 
          to={ROUTES.TRANSACTIONS.TRANSFER}
          icon="bi-arrow-left-right" 
          label="Transfer" 
          description="Send money to another account"
          theme="text-info"
          disabled={isFrozen}
        />
      </div>
      <hr className="my-5 opacity-25" />
      <TransactionHistory account_number={account.account_number} />
    </div>
  );
};
import { BankAccount } from "../../../types/account";

interface Props {
  account: BankAccount;
  onClose: () => void;
}

export const AccountDetailsDialog = ({ account, onClose }: Props) => {
  return (
    <div
      className="modal fade show d-block account-details-modal">
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content border-0 shadow-lg">
          <div className="modal-header border-bottom-0 py-3">
            <h5 className="modal-title fw-bold">
              <i className="bi bi-info-circle me-2 text-primary"></i>
              Account Details
            </h5>
          </div>
          <div className="modal-body p-0">
            <ul className="list-group list-group-flush">
              <li className="list-group-item d-flex justify-content-between align-items-center py-3">
                <span className="text-muted small">Account Number</span>
                <span className="fw-bold text-primary">
                  {account.account_number}
                </span>
              </li>
              <li className="list-group-item d-flex justify-content-between align-items-center py-3">
                <span className="text-muted small">Owner's Name</span>
                <span className="fw-bold">{account.owner_name}</span>
              </li>
              <li className="list-group-item d-flex justify-content-between align-items-center py-3">
                <span className="text-muted small">System Role</span>
                <span
                  className={`badge ${account.role === "Admin" ? "bg-dark" : "bg-secondary"}`}
                >
                  {account.role}
                </span>
              </li>
              <li className="list-group-item d-flex justify-content-between align-items-center py-3">
                <span className="text-muted small">Current Balance</span>
                <span className="fw-bold text-success fs-5">
                  {account.balance.toLocaleString("en-US")} $
                </span>
              </li>
              <li className="list-group-item d-flex justify-content-between align-items-center py-3">
                <span className="text-muted small">Created Date</span>
                <span>
                  {new Date(account.created_at).toLocaleDateString("en-US", {
                    day: "2-digit",
                    month: "2-digit",
                    year: "numeric",
                    hour: "2-digit",
                    minute: "2-digit",
                  })}
                </span>
              </li>
              <li className="list-group-item d-flex justify-content-between align-items-center py-3 border-bottom-0">
                <span className="text-muted small">Account Status</span>
                <span
                  className={`badge ${account.status === 0 ? "bg-success" : "bg-danger"}`}
                >
                  {account.status === 0 ? "Active" : "Frozen"}
                </span>
              </li>
            </ul>
          </div>
          <div className="modal-footer border-top-0 p-3">
            <button
              type="button"
              className="btn btn-secondary w-100 shadow-sm"
              onClick={onClose}
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

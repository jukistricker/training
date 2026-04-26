import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchTransactionHistory } from "../../../store/thunks/transactionThunk";
import { AppDispatch, RootState } from "../../../store/store";
import { TransactionType } from "../../../types/transaction";
import { FilterGroup } from "../../../components/ui/FilterGroup";
import { Pagination } from "../../../components/ui/Pagination";

interface Props {
  account_number: number;
}

const TRANSACTION_FILTERS = [
  { label: "All", value: undefined },
  { label: "Deposits", value: TransactionType.Deposit },
  { label: "Withdrawals", value: TransactionType.Withdraw },
  { label: "Transfers", value: TransactionType.Transfer },
];

export const TransactionHistory = ({ account_number }: Props) => {
  const dispatch = useDispatch<AppDispatch>();

  const [viewAll, setViewAll] = useState(false);
  const [activeFilter, setActiveFilter] = useState<TransactionType | undefined>(
    undefined,
  );

  const handleFilterChange = (value: TransactionType | undefined) => {
    setActiveFilter(value);
    setPage(1);
  };
  const [page, setPage] = useState(1);

  const transactionState = useSelector(
    (state: RootState) => state.transactions,
  );
  const isAdmin = useSelector(
    (state: RootState) => state.account.user?.role === "Admin",
  );

  const { items = [], loading, totalCount = 0 } = transactionState || {};
  const renderAmountPrefix = (type: TransactionType) => {
    if (type === TransactionType.Deposit) return "+";
    if (type === TransactionType.Withdraw) return "-";
    return ""; // Cho Transfer hoặc các loại khác
  };
  const renderAmountColor = (type: TransactionType) => {
    if (type === TransactionType.Deposit) return "text-success";
    if (type === TransactionType.Withdraw) return "text-danger";
    return "text-info";
  };

  useEffect(() => {
    dispatch(
      fetchTransactionHistory({
        account_number: viewAll ? undefined : account_number,
        type: activeFilter,
        page,
      }),
    );
  }, [dispatch, account_number, activeFilter, page, viewAll]);

  const getBadgeClass = (type: number) => {
    switch (type) {
      case TransactionType.Deposit:
        return "bg-success-subtle text-success";
      case TransactionType.Withdraw:
        return "bg-danger-subtle text-danger";
      default:
        return "bg-info-subtle text-info";
    }
  };

  return (
    <div className="mt-5">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h5 className="fw-bold mb-0">
            {viewAll ? "Global Transaction History" : "Transaction History"}
          </h5>
          <small className="text-muted">
            {viewAll ? "Showing all users" : `Account: ${account_number}`}
          </small>
        </div>

        <div className="d-flex gap-2">
          {isAdmin && (
            <button
              className={`btn btn-sm ${viewAll ? "btn-warning" : "btn-outline-secondary"}`}
              onClick={() => {
                setViewAll(!viewAll);
                setPage(1);
              }}
            >
              <i
                className={`bi ${viewAll ? "bi-person-fill" : "bi-people-fill"} me-1`}
              ></i>
              {viewAll ? "Back to Current User" : "View All Transactions"}
            </button>
          )}

          <FilterGroup
            options={TRANSACTION_FILTERS}
            activeValue={activeFilter}
            onChange={handleFilterChange}
          />
        </div>
      </div>

      <div className="card border-0 shadow-sm overflow-hidden">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead>
              <tr className="small text-uppercase text-muted">
                <th className="ps-4 py-3">Date</th>
                <th>Type</th>
                <th>Amount</th>
                <th className="pe-4">Description</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={4} className="text-center py-5">
                    <div className="spinner-border spinner-border-sm text-primary me-2"></div>
                    Loading history...
                  </td>
                </tr>
              ) : items.length === 0 ? (
                <tr>
                  <td colSpan={4} className="text-center py-5 text-muted">
                    No transactions found.
                  </td>
                </tr>
              ) : (
                items.map((t) => (
                  <tr key={t.id}>
                    <td className="ps-4 small text-muted">
                      {t.created_at
                        ? new Date(t.created_at).toLocaleString("en-US")
                        : "N/A"}
                    </td>
                    <td>
                      <span
                        className={`badge rounded-pill ${getBadgeClass(t.transaction_type)}`}
                      >
                        {TransactionType[t.transaction_type]}
                      </span>
                    </td>
                    <td
                      className={`fw-bold ${renderAmountColor(t.transaction_type)}`}
                    >
                      {renderAmountPrefix(t.transaction_type)}{" "}
                      {t.amount?.toLocaleString()} $
                    </td>

                    <td
                      className="text-muted small pe-4 text-truncate"
                      style={{ maxWidth: "200px" }}
                    >
                      {t.description}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
      {!loading && items.length > 0 && (
        <div className="px-2 pb-4">
          <Pagination
            totalItems={totalCount}
            itemsPerPage={10}
            currentPage={page}
            onPageChange={(newPage) => setPage(newPage)}
          />
        </div>
      )}
    </div>
  );
};

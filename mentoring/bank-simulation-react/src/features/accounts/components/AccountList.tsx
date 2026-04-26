import { useEffect, useMemo, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { AppDispatch, RootState } from "../../../store/store";
import {
  fetchAllAccounts,
  toggleAccountStatus,
} from "../../../store/thunks/accountThunk";
import { BankAccount } from "../../../types/account";
import { AccountDetailsDialog } from "../../../components/ui/AccountDetailsDialog";
import { CreateAccountDialog } from "../../../components/ui/CreateAccountDialog";
import { usePagination } from "../../../hooks/usePagination";
import { Pagination } from "../../../components/ui/Pagination";
import { BaseDialog } from "../../../components/ui/BaseDialog";

export const AccountList = () => {
  const dispatch = useDispatch<AppDispatch>();

  const {
    accounts,
    user: currentUser,
    loading: fetching,
  } = useSelector((state: RootState) => state.account);
  const isAdmin = currentUser?.role === "Admin";

  const [searchTerm, setSearchTerm] = useState("");
  const [selectedAccount, setSelectedAccount] = useState<BankAccount | null>(
    null,
  );
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [confirmData, setConfirmData] = useState<{
    id: string;
    status: number;
    name: string;
  } | null>(null);

  useEffect(() => {
    dispatch(fetchAllAccounts());
  }, [dispatch]);

  const confirmToggleStatus = async () => {
    if (!confirmData) return;
    await dispatch(
      toggleAccountStatus({
        id: confirmData.id,
        currentStatus: confirmData.status,
      }),
    );
    setConfirmData(null);
  };

  const filteredAccounts = useMemo(() => {
    return accounts.filter(
      (acc) =>
        acc.owner_name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        acc.account_number?.toString().includes(searchTerm),
    );
  }, [accounts, searchTerm]);

  const { currentItems, currentPage, setCurrentPage, totalItems } =
    usePagination(filteredAccounts, 10);

  return (
    <div className="container py-4">
      
          <div className="d-flex justify-content-between align-items-center mb-3">
            <div>
              <h4 className="fw-bold mb-0">User Management</h4>
              <p className="text-muted small mb-0">
                Manage information of all bank users
              </p>
            </div>
            <div className="d-flex gap-2">
              <div style={{ width: "200px" }}>
                <input
                  type="text"
                  className="form-control form-control-sm shadow-sm"
                  placeholder="Search..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
              <button
                className="btn btn-sm btn-success shadow-sm px-3"
                onClick={() => setIsCreateOpen(true)} // Thêm state này
              >
                <i className="bi bi-plus-lg me-1"></i> Create
              </button>
            </div>
          </div>

          <div className="card shadow-sm border-0">
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0">
                <thead className=" border-bottom">
                  <tr>
                    <th className="ps-4">Account Number</th>
                    <th>Owner's Name</th>
                    <th>Balance</th>
                    <th>Status</th>
                    <th className="text-end pe-4">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {fetching ? (
                    <tr>
                      <td colSpan={5} className="text-center py-5">
                        Loading...
                      </td>
                    </tr>
                  ) : (
                    currentItems.map((acc, index) => (
                      <tr key={acc.id}>
                        <td className="fw-bold ps-4">{acc.account_number}</td>
                        <td>{acc.owner_name}</td>
                        <td className={`fw-bold ${acc.balance<100?'text-danger':'text-success'}`}>
                          {acc.balance.toLocaleString()} $
                        </td>
                        <td>
                          <span
                            className={`badge ${acc.status === 1 ? "bg-danger" : "bg-success"} rounded-pill`}
                          >
                            {acc.status === 1 ? "Frozen" : "Active"}
                          </span>
                        </td>
                        <td className="text-end pe-4">
                          <div className="btn-group">
                            <button
                              className="btn btn-sm border"
                              onClick={() => setSelectedAccount(acc)}
                              title="View details"
                            >
                              <i className="bi bi-eye text-primary"></i>
                            </button>
                            <button
                              onClick={() =>
                                setConfirmData({
                                  id: acc.id,
                                  status: acc.status,
                                  name: acc.owner_name,
                                })
                              }
                              className="btn btn-sm border ms-1"
                              title={acc.status === 0 ? "Freeze" : "Unfreeze"}
                            >
                              {acc.status === 0 ? (
                                <i className="bi bi-pause-fill text-danger"></i>
                              ) : (
                                <i className="bi bi-play-fill text-success"></i>
                              )}
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
            <div className="p-3 border-top">
              <Pagination
                totalItems={totalItems}
                itemsPerPage={10}
                currentPage={currentPage}
                onPageChange={setCurrentPage}
              />
            </div>
          </div>
        

      <BaseDialog
        isOpen={!!confirmData}
        onClose={() => setConfirmData(null)}
        title="Confirm Status Change"
        footer={
          <div className="d-flex gap-2">
            <button
              className="btn btn-light btn-sm px-3"
              onClick={() => setConfirmData(null)}
            >
              Cancel
            </button>
            <button
              className={`btn btn-sm px-3 ${confirmData?.status === 0 ? "btn-danger" : "btn-success"}`}
              onClick={confirmToggleStatus}
            >
              {confirmData?.status === 0 ? "Freeze Now" : "Unfreeze Now"}
            </button>
          </div>
        }
      >
        <div className="text-center py-3">
          <i
            className={`bi ${confirmData?.status === 0 ? "bi-exclamation-triangle text-danger" : "bi-info-circle text-primary"} display-4 mb-3 d-block`}
          ></i>
          <p className="mb-1">
            Are you sure you want to{" "}
            <b>{confirmData?.status === 0 ? "Freeze" : "Unfreeze"}</b> this
            account?
          </p>
          <p className="text-muted small">
            Account: <b>{confirmData?.name}</b>
          </p>
        </div>
      </BaseDialog>

      {/* Modal xem chi tiết */}
      {selectedAccount && (
        <AccountDetailsDialog
          account={selectedAccount}
          onClose={() => setSelectedAccount(null)}
        />
      )}

      {/* Modal tạo tài khoản mới */}
      {isCreateOpen && (
        <CreateAccountDialog
          onClose={() => setIsCreateOpen(false)}
          onSuccess={() => dispatch(fetchAllAccounts())}
        />
      )}
    </div>
  );
};

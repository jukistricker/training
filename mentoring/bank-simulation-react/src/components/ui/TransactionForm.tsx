import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AppDispatch, RootState } from "../../store/store";
import { executeTransaction } from "../../store/thunks/transactionThunk";
import { TransactionType } from "../../types/transaction";
import { InputField } from "./InputField";
import {
  TransactionFormValues,
  transactionSchema,
} from "../../features/transactions/types";
import { ROUTES } from "../../config/constants/url_routes";
import toast from "react-hot-toast";
import { JSON_SERVER_ACCOUNTS } from "../../config/constants/json_server_routes";
import { api } from "../../lib/api";

interface TransactionFormProps {
  type: "deposit" | "withdraw" | "transfer";
  initialaccount_number: number;
}

export const TransactionForm = ({
  type,
  initialaccount_number,
}: TransactionFormProps) => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const isTransfer = type === "transfer";

  const { isProcessing, error } = useSelector(
    (state: any) => state.transactions,
  );
  const userId = useSelector((state: RootState) => state.account.user?.id);
  const [suggestions, setSuggestions] = React.useState<any[]>([]);
  const [showSuggestions, setShowSuggestions] = React.useState(false);
  // Trong TransactionForm.tsx

  const searchAccounts = async (query: string) => {
    try {
      const response = await api.get<any[]>(JSON_SERVER_ACCOUNTS, {
        account_number: query,
        account_number_ne: initialaccount_number,
        _per_page: 10,
      });
      setSuggestions(response);
    } catch (err) {
      console.error("Search failed", err);
    }
  };

  const [searchTerm, setSearchTerm] = React.useState("");

  React.useEffect(() => {
    if (isTransfer && searchTerm.length > 0) {
      const delay = setTimeout(() => searchAccounts(searchTerm), 500);
      return () => clearTimeout(delay);
    } else {
      setSuggestions([]);
    }
  }, [searchTerm]);

  const config = {
    deposit: {
      color: "success",
      label: "Deposit",
      type: TransactionType.Deposit,
    },
    withdraw: {
      color: "warning",
      label: "Withdraw",
      type: TransactionType.Withdraw,
    },
    transfer: {
      color: "primary",
      label: "Transfer",
      type: TransactionType.Transfer,
    },
  }[type];

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<TransactionFormValues>({
    resolver: zodResolver(transactionSchema),
    defaultValues: {
      account_number: initialaccount_number,
      destinationaccount_number: 0,
      amount: 0,
      description: "",
    },
  });

  useEffect(() => {
    setValue("account_number", initialaccount_number);
  }, [initialaccount_number, setValue]);

  const onSubmit = async (data: TransactionFormValues) => {
    const resultAction = await dispatch(
      executeTransaction({
        account_number: data.account_number,
        destinationaccount_number: isTransfer
          ? data.destinationaccount_number
          : undefined,
        amount: data.amount,
        type: config.type,
        description: data.description || "",
      }),
    );

    if (executeTransaction.fulfilled.match(resultAction)) {
      navigate(`${ROUTES.ACCOUNT.DETAILS}`, {
        state: { id: userId },
      });
      toast.success(`${config.label} successfully!`);
    }
  };

  return (
    <div className="card shadow-sm border-0 mt-3">
      <div className={`card-header bg-${config.color} text-white py-3`}>
        <h5 className="mb-0">{config.label} Money</h5>
      </div>
      <div className="card-body p-4">
        <form onSubmit={handleSubmit(onSubmit)}>
          <InputField
            label={isTransfer ? "Source Account" : "Account Number"}
            readOnly
            className="bg-light fw-bold"
            {...register("account_number")}
            error={
              errors.account_number?.message ||
              (error?.field === "account_number" ? error.message : "")
            }
          />

          {isTransfer && (
            <div className="mb-3 position-relative">
              <label className="form-label small fw-bold">
                Destination Account
              </label>
              <input
                type="text"
                className={`form-control ${
                  errors.destinationaccount_number ||
                  error?.field === "destinationaccount_number"
                    ? "is-invalid"
                    : ""
                }`}
                placeholder="Search by account number..."
                autoComplete="off"
                value={searchTerm}
                onChange={(e) => {
                  const val = e.target.value;
                  setSearchTerm(val);

                  const numericVal = val === "" ? 0 : Number(val);
                  setValue("destinationaccount_number", numericVal, {
                    shouldValidate: true,
                    shouldDirty: true,
                  });
                }}
                onFocus={() => setShowSuggestions(true)}
                onBlur={() => setTimeout(() => setShowSuggestions(false), 200)}
              />

              {showSuggestions && suggestions.length > 0 && (
                <ul
                  className="list-group position-absolute w-100 shadow-sm z-3"
                  style={{ maxHeight: "200px", overflowY: "auto" }}
                >
                  {suggestions.map((acc) => (
                    <li
                      key={acc.id}
                      className="list-group-item list-group-item-action cursor-pointer"
                      onMouseDown={(e) => {
                        const selectedAcc = Number(acc.account_number);
                        setValue("destinationaccount_number", selectedAcc, {
                          shouldValidate: true,
                        });
                        setSearchTerm(acc.account_number.toString());
                        setShowSuggestions(false);
                      }}
                    >
                      <div className="fw-bold text-primary">
                        {acc.account_number}
                      </div>
                      <small className="text-muted">{acc.owner_name}</small>
                    </li>
                  ))}
                </ul>
              )}

              <input
                type="hidden"
                {...register("destinationaccount_number", {
                  valueAsNumber: true,
                })}
              />

              {(errors.destinationaccount_number?.message ||
                (error?.field === "destinationaccount_number" &&
                  error.message)) && (
                <div className="invalid-feedback d-block">
                  {errors.destinationaccount_number?.message || error.message}
                </div>
              )}
            </div>
          )}
          <InputField
            label={`Amount to ${type} ($)`}
            type="number"
            placeholder="0"
            {...register("amount", {
              valueAsNumber: true,
              onChange: (e) => {
                let val = e.target.value;

                if (val === "") {
                  setValue("amount", 0);
                  return;
                }
                if (val.length > 1 && val.startsWith("0")) {
                  const sanitized = Number(val);
                  setValue("amount", sanitized);
                }
              },
            })}
            onFocus={(e) => {
              if (Number(e.target.value) === 0) {
                setValue("amount", "" as any);
              }
            }}
            error={
              errors.amount?.message ||
              (error?.field === "amount" ? error?.message : "")
            }
          />

          <div className="mb-4">
            <label className="form-label small fw-bold">Description</label>
            <textarea
              className={`form-control ${errors.description || error?.field === "description" ? "is-invalid" : ""}`}
              rows={2}
              {...register("description")}
              placeholder="Enter description..."
            />
            {(errors.description?.message ||
              (error?.field === "description" ? error.message : "")) && (
              <div className="invalid-feedback">
                {errors.description?.message || error.message}
              </div>
            )}
          </div>

          <div className="d-grid gap-2">
            <button
              type="submit"
              className={`btn btn-${config.color} py-2 fw-bold`}
              disabled={isProcessing}
            >
              {isProcessing ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2"></span>
                  Processing...
                </>
              ) : (
                config.label
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

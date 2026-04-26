import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useDispatch, useSelector } from "react-redux";
import { registerUser } from "../../store/thunks/accountThunk";
import { registerSchema, RegisterFormValues } from "../../features/accounts/types";
import { InputField } from "./InputField";
import toast from "react-hot-toast";

interface Props {
  onClose: () => void;
  onSuccess: () => void;
}

export const CreateAccountDialog = ({ onClose, onSuccess }: Props) => {
  const dispatch = useDispatch<any>();
  const { loading, error } = useSelector((state: any) => state.account);

  const {
    register,
    setValue,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      account_number: 0,
      ownerName: "",
      initialBalance: 100,
      password: "",
      confirmPassword: "",
    },
  });

  const onSubmit = async (data: RegisterFormValues) => {
    const result = await dispatch(registerUser(data));
    if (registerUser.fulfilled.match(result)) {
      toast.success("Account created successfully!");
      onSuccess();
      onClose();
    }
  };

  return (
    <div className="modal fade show d-block" style={{ backgroundColor: "rgba(0,0,0,0.2)", backdropFilter: "blur(4px)" }}>
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content border-0 shadow-lg">
          <div className="modal-header bg-success text-white">
            <h5 className="modal-title fw-bold">
              <i className="bi bi-person-plus-fill me-2"></i>New Account
            </h5>
            <button type="button" className="btn-close btn-close-white" onClick={onClose}></button>
          </div>
          <div className="modal-body p-4">
            <form onSubmit={handleSubmit(onSubmit)}>
              {error && !error.field && (
                <div className="alert alert-danger p-2 small">{error.message}</div>
              )}

              <InputField
                label="Account Number"
                placeholder="Ví dụ: 1903..."
                {...register("account_number", {
                    valueAsNumber: true,
                    onChange: (e) => {
                      const val = e.target.value;
                      if (val === "") {
                        setValue("account_number", "" as any);
                        return;
                      }
                      if (val.length > 1 && val.startsWith("0")) {
                        setValue("account_number", Number(val));
                      }
                    },
                  })}
                error={errors.account_number?.message || (error?.field === "account_number" ? error.message : "")}
              />

              <InputField
                label="Owner Name"
                placeholder="NGUYEN VAN A"
                {...register("ownerName")}
                error={errors.ownerName?.message}
              />

              <InputField
                label="Initial Balance"
                type="number"
                {...register("initialBalance", {
                  valueAsNumber: true,
                  onChange: (e) => {
                    if (e.target.value === "") setValue("initialBalance", 0);
                  },
                })}
                error={errors.initialBalance?.message}
              />

              <div className="row">
                <div className="col-md-6">
                  <InputField
                    label="Password"
                    type="password"
                    {...register("password")}
                    error={errors.password?.message}
                  />
                </div>
                <div className="col-md-6">
                  <InputField
                    label="Confirm"
                    type="password"
                    {...register("confirmPassword")}
                    error={errors.confirmPassword?.message}
                  />
                </div>
              </div>

              <div className="d-grid gap-2 mt-4">
                <button type="submit" className="btn btn-success py-2 fw-bold" disabled={loading}>
                  {loading ? (
                    <span className="spinner-border spinner-border-sm me-2"></span>
                  ) : null}
                  Create Account
                </button>
                <button type="button" className="btn btn-light" onClick={onClose}>
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};
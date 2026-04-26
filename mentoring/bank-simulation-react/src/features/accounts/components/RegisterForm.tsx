import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate, Link } from "react-router-dom";
import { registerUser } from "../../../store/thunks/accountThunk";
import { registerSchema, RegisterFormValues } from "../types";
import { ROUTES } from "../../../config/constants/url_routes";
import toast from "react-hot-toast";
import { InputField } from "../../../components/ui/InputField";

export const RegisterForm = () => {
  const dispatch = useDispatch<any>();
  const navigate = useNavigate();
  const { loading, error } = useSelector((state: any) => state.account);

  const {
    register,
    setValue,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      account_number: "",
      ownerName: "",
      initialBalance: 100,
      password: "",
      confirmPassword: "",
    },
  });

  const onSubmit = async (data: RegisterFormValues) => {
    const result = await dispatch(registerUser(data));
    if (registerUser.fulfilled.match(result)) {
      toast.success("Account created! Redirecting to login...");
      setTimeout(() => navigate(ROUTES.ACCOUNT.LOGIN), 2000);
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-5">
          <div className="card shadow border-0">
            <div className="card-header bg-success text-white text-center py-3">
              <h4 className="mb-0 text-uppercase">Create New Bank Account</h4>
            </div>
            <div className="card-body p-4">
              <form onSubmit={handleSubmit(onSubmit)}>
                {/* Global Error Message */}
                {error && !error.field && (
                  <div className="alert alert-danger p-2 small">
                    {error.message}
                  </div>
                )}

                <InputField
                label="Account Number"
                placeholder="Eg: 1903..."
                {...register("account_number")}
                error={
                  errors.account_number?.message ||
                  (error?.field === "account_number" ? error.message : "")
                }
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
                      const val = e.target.value;
                      if (val === "") {
                        setValue("initialBalance", "" as any);
                        return;
                      }
                      if (val.length > 1 && val.startsWith("0")) {
                        setValue("initialBalance", Number(val));
                      }
                    },
                  })}
                  error={errors.initialBalance?.message}
                />

                <InputField
                  label="Password"
                  type="password"
                  {...register("password")}
                  error={errors.password?.message}
                />

                <InputField
                  label="Confirm Password"
                  type="password"
                  {...register("confirmPassword")}
                  error={errors.confirmPassword?.message}
                />

                <div className="d-grid gap-2 mt-4">
                  <button
                    type="submit"
                    className="btn btn-success"
                    disabled={loading}
                  >
                    {loading ? "Creating..." : "Create Account"}
                  </button>
                  <div className="text-center mt-3">
                    <span className="small text-muted">Already have an account? </span>
                    <Link to={ROUTES.ACCOUNT.LOGIN} className="small text-decoration-none fw-bold">
                      Login
                    </Link>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

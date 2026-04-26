import React, { forwardRef } from "react";

interface InputFieldProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
}

export const InputField = forwardRef<HTMLInputElement, InputFieldProps>(
  ({ label, error, className, ...props }, ref) => {
    return (
      <div className="mb-3">
        <label className="form-label">{label}</label>
        <input
          {...props}
          ref={ref}
          className={`form-control ${error ? "is-invalid" : ""} ${className}`}
        />
        {error && <div className="invalid-feedback small">{error}</div>}
      </div>
    );
  }
);

InputField.displayName = "InputField"; 
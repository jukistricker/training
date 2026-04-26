import { z } from 'zod';

export const loginSchema = z.object({
  account_number: z.number()
    .min(1, "Account number is required"),
  password: z.string()
    .min(1, "Password is required"),
});

export type LoginFormValues = z.infer<typeof loginSchema>;

export const registerSchema = z.object({
  account_number: z.string()
    .min(1, "Account number is required")
    .regex(/^[0-9]+$/, "Account number must contain only digits"),
  ownerName: z.string()
    .min(1, "Owner name is required"),
  initialBalance: z.number()
    .min(1, "Initial balance must be at least 1$"),
  password: z.string()
    .min(1, "Password is required"),
  confirmPassword: z.string()
    .min(1, "Confirmation password is required"),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Confirmation password doesn't match.",
  path: ["confirmPassword"], 
});

export type RegisterFormValues = z.infer<typeof registerSchema>;
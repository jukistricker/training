import z from "zod";
import { TransactionType } from "../../../types/transaction";

export const transactionSchema = (type: "deposit" | "withdraw" | "transfer") =>
  z.object({
    account_number: z.number()
      .min(1, "Account number is required"),
    amount: z.number()
      .min(1, "Amount must be at least 1$")
      .max(1000000, "Amount must be less than 1,000,000$"),
    destinationaccount_number: type === "transfer"
      ? z.number().min(1, "Destination account is required")
      : z.number().optional(),
    description: type === "transfer"
      ? z.string().max(300, "Description must be less than 300 characters").optional()
      : z.string().optional(),
    type: z.nativeEnum(TransactionType).optional(),
  });
 
export type TransactionFormValues = z.infer<ReturnType<typeof transactionSchema>>;
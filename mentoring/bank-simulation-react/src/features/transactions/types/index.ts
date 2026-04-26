import z from "zod";
import { TransactionType } from "../../../types/transaction";

export const transactionSchema = z.object({
  account_number: z.number()
    .min(1, "Account number is required"),
  amount: z.number()
    .min(1, "Amount must be at least 1$")
    .max(1000000, "Amount must be less than 1,000,000$"),
  
  
  destinationaccount_number: z.number().optional(),
  description: z.string().max(300,"Description must be less than 300 characters").optional(),
  
  type: z.nativeEnum(TransactionType).optional(), 
});

export type TransactionFormValues = z.infer<typeof transactionSchema>;
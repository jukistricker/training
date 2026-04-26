import { configureStore } from '@reduxjs/toolkit';
import accountReducer from './slices/accountSlice';
import transactionReducer from './slices/transactionSlice';

export const store = configureStore({
  reducer: {
    account: accountReducer,
    transactions: transactionReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false, 
    }),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
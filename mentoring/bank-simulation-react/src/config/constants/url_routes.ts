export const ACCOUNT_BASE = '/accounts';
export const TRANSACTION_BASE = '/transactions';

export const ROUTES = {
  HOME: '/',
  ACCOUNT: {
    LIST: `${ACCOUNT_BASE}`,
    LOGIN: `${ACCOUNT_BASE}/login`,
    REGISTER: `${ACCOUNT_BASE}/create`,
    DETAILS: `${ACCOUNT_BASE}/:id`,
  },
  TRANSACTIONS: {
    HISTORY: `${TRANSACTION_BASE}/history`,
    TRANSFER: `${TRANSACTION_BASE}/transfer`,
    DEPOSIT: `${TRANSACTION_BASE}/deposit`,
    WITHDRAW: `${TRANSACTION_BASE}/withdraw`,
  }
} as const;
import { USER_KEY } from "../config/constants/storage_key";

export const genericStorage = {
    get: (key: string) => {
        const data = localStorage.getItem(key);
        return data ? JSON.parse(data) : null;
    },
    save(key: string, data: any) {
        localStorage.setItem(key, JSON.stringify(data));
    },
    clear(key: string) {
        localStorage.removeItem(key);
    }
}

export const userStorage = {

  updateBalance: (newBalance: number) => {
    const user = genericStorage.get(USER_KEY);
    if (user) {
      user.balance = newBalance;
      genericStorage.save(USER_KEY, user);
    }
  },
};
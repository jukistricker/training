import axios from 'axios';

const axiosClient = axios.create({
  baseURL: 'http://localhost:5000', // Port JSON Server 
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosClient.interceptors.response.use(
  (response) => response.data, 
  (error) => {
    return Promise.reject(error.response?.data || 'Network Error');
  }
);

export default axiosClient;
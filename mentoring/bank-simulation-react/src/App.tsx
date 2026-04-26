import React from 'react'
import ReactDOM from 'react-dom/client'
import { RouterProvider } from 'react-router-dom'
import { Provider } from 'react-redux'
import { store } from './store/store'
import { router } from './router'
import './global.css'
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css'; //kệ nó, hoặc mở file vite-env.d.ts là tự hết
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import { Toaster } from 'react-hot-toast'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <Provider store={store}>
      <Toaster 
        position="top-right"
        toastOptions={{
          className: 'custom-toast',
        }}
      />
      <RouterProvider router={router} />
    </Provider>
  </React.StrictMode>,
)
import React from 'react';
import { Outlet } from 'react-router-dom';

const AuthLayout: React.FC = () => {
  return (
    <div className="auth-wrapper bg-light min-vh-100 d-flex align-items-center">
      <div className="container">
        <main role="main" className="pb-3">
          
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default AuthLayout;
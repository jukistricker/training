import React, { useState, useEffect } from 'react';
import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { logout } from '../../store/slices/accountSlice';
import { RootState } from '../../store/store';
import { ACCOUNT_BASE, ROUTES } from '../../config/constants/url_routes';
import { ErrorBoundary } from '../../ErrorBoundary';
import { AccountStatus } from '../../types/account';

const MainLayout: React.FC = () => {
  const [isSidebarActive, setSidebarActive] = useState(false);
  const [theme, setTheme] = useState(localStorage.getItem('theme') || 'light');
  
  const navigate = useNavigate();
  const dispatch = useDispatch();

  // Lấy dữ liệu từ Redux
  const { user } = useSelector((state: RootState) => state.account);
  const isAdmin = user?.role === 'Admin';
  
  const accountId = user?.id; 

  useEffect(() => {
    document.documentElement.setAttribute('data-bs-theme', theme);
    localStorage.setItem('theme', theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme(prev => (prev === 'light' ? 'dark' : 'light'));
  };

  const handleLogout = () => {
    document.documentElement.setAttribute('data-bs-theme', 'light');
    dispatch(logout());
    navigate(ROUTES.ACCOUNT.LOGIN);
  };

  return (
    <div id="wrapper">
      {/* Sidebar */}
      <nav id="sidebar" className={isSidebarActive ? 'active-mobile' : ''}>
        <div className="sidebar-header">
          <h5 className="mb-0 fw-bold text-success">
            <i className="bi bi-bank me-2"></i>BANK SIM
          </h5>
        </div>

        <ul className="list-unstyled mt-3">
          <li>
            <NavLink to={ROUTES.HOME} end className={({ isActive }) => (isActive ? 'active' : '')}>
              <i className="bi bi-speedometer2"></i> Dashboard
            </NavLink>
          </li>

          {/* User's Personal Account Link */}
          <li>
            <NavLink to={`${ACCOUNT_BASE}/${accountId}`} className={({ isActive }) => (isActive ? 'active' : '')}>
              <i className="bi bi-person-badge"></i> My Account
            </NavLink>
          </li>

          {/* --- ADMIN SECTION --- */}
          {isAdmin && (
            <>
              <li className="px-3 small text-uppercase text-muted fw-bold mt-4 mb-2">Administration</li>
              <li>
                <NavLink to={ROUTES.ACCOUNT.LIST} end>
                  <i className="bi bi-people"></i> All Accounts
                </NavLink>
              </li>
              
            </>
          )}

          {/* --- TRANSACTIONS SECTION --- */}
          {user?.status === AccountStatus.Active && (
            <>
              <li className="px-3 small text-uppercase text-muted fw-bold mt-4 mb-2">Personal Banking</li>
          <li>
            <NavLink to={ROUTES.TRANSACTIONS.DEPOSIT}>
              <i className="bi bi-cash-stack"></i> Deposit
            </NavLink>
          </li>
          <li>
            <NavLink to={ROUTES.TRANSACTIONS.WITHDRAW}>
              <i className="bi bi-box-arrow-right"></i> Withdraw
            </NavLink>
          </li>
          <li>
            <NavLink to={ROUTES.TRANSACTIONS.TRANSFER}>
              <i className="bi bi-arrow-left-right"></i> Transfer
            </NavLink>
          </li>
            </>
          )

          }
        </ul>
      </nav>

      {/* Content Area */}
      <div id="content">
        <nav className="top-navbar shadow-sm d-flex align-items-center px-3">
          <button 
            type="button" 
            className="btn btn-sm btn-outline-secondary d-md-none me-2"
            onClick={() => setSidebarActive(!isSidebarActive)}
          >
            <i className="bi bi-list"></i>
          </button>

          <div className="ms-auto d-flex align-items-center">
            {/* Theme Switcher */}
            <div 
              id="themeSwitcher" 
              className="me-3 btn btn-sm btn-outline-secondary border-0" 
              style={{ cursor: 'pointer' }}
              onClick={toggleTheme}
            >
              <i className={`bi ${theme === 'light' ? 'bi-sun-fill' : 'bi-moon-stars-fill'}`}></i>
            </div>

            {/* Profile Dropdown */}
            <div className="dropdown">
              <button 
                className="btn btn-link text-decoration-none dropdown-toggle text-body d-flex align-items-center shadow-none" 
                type="button" 
                data-bs-toggle="dropdown"
              >
                <div className="text-end me-2 d-none d-sm-block">
                  <div className="small fw-bold lh-1">{user?.owner_name}</div>
                  <small className="text-muted" style={{ fontSize: '10px' }}>{user?.role}</small>
                </div>
                <i className="bi bi-person-circle fs-4"></i>
              </button>
              <ul className="dropdown-menu dropdown-menu-end shadow border-0 mt-2">
                <li>
                  <div className="dropdown-item-text border-bottom pb-2 mb-1">
                    <p className="mb-0 small text-muted">Signed in as</p>
                    <strong>{user?.owner_name}</strong>
                  </div>
                </li>
                <li>
                  <button onClick={handleLogout} className="dropdown-item text-danger">
                    <i className="bi bi-box-arrow-left me-2"></i>Log out
                  </button>
                </li>
              </ul>
            </div>
          </div>
        </nav>

        <main className="main-container p-4">
          <ErrorBoundary>
            <Outlet />
          </ErrorBoundary>
        </main>

        <footer className="py-3 px-4 border-top bg-body-tertiary mt-auto">
          <div className="container-fluid text-center text-muted small">
            &copy; 2026 - BankAccountSimulation - <a href="https://github.com/jukistricker" className="text-decoration-none">Privacy</a>
          </div>
        </footer>
      </div>
    </div>
  );
};

export default MainLayout;
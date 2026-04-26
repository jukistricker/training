import { useSelector } from 'react-redux';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { ROUTES } from '../config/constants/url_routes';

export const ProtectedRoute = () => {
  const { user } = useSelector((state: any) => state.account);
  const location = useLocation();

 
  if (!user) {
    return <Navigate to={ROUTES.ACCOUNT.LOGIN} state={{ from: location }} replace />;
  }
  return <Outlet />;
};
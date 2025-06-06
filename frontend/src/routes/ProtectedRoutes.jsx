import { Navigate, Outlet } from "react-router-dom";
import { useSelector } from 'react-redux';


const ProtectedRoutes = ({ element }) => {
  const isAuthenticated = useSelector(state => state.auth.isAuthenticated);

  return isAuthenticated ? element : <Navigate to="/login" />;
};

export default ProtectedRoutes;

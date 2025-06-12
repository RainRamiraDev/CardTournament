import { Routes, Route } from "react-router-dom";
import LogInForm from '../components/LogInForm';
import Error404 from "../components/ui/Error404";
import ProtectedRoutes from './ProtectedRoutes';
import Menu from '../components/ui/menu/Menu';
import CrudUsuarioView from '../views/CrudUsuarioView';
import AssignCardsToUserView from '../views/AssignCardsToUserView';
import PlayerCardsView from '../views/PlayersCardsView';

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/login" element={<LogInForm />} />


      <Route
        path="/menu"
        element={<ProtectedRoutes element={<Menu />} />}
      />
        <Route
        path="/cartas"
        element={<ProtectedRoutes element={<AssignCardsToUserView />} />}
      />

         <Route
        path="/CardsUsuarios"
        element={<ProtectedRoutes element={<PlayerCardsView />} />}
      />
       <Route
        path="/usuarios"
        element={<ProtectedRoutes element={<CrudUsuarioView />} />}
      />
      <Route path="*" element={<Error404 />} />
    </Routes>
  );
};

export default AppRoutes;

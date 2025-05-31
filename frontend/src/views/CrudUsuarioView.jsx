import React from 'react';
import { Box, CssBaseline } from '@mui/material';
import Menu from '../components/ui/menu/Menu'; // Asegurate de que el path sea correcto
import UserCrudForm from '../components/CrudUsuarioForm'; // Componente del formulario

const CrudUsuarioView = () => {
  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <Menu />

      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <UserCrudForm />
      </Box>
    </Box>
  );
};

export default CrudUsuarioView;

import React from 'react';
import { Box, CssBaseline } from '@mui/material';
import Menu from '../components/ui/menu/Menu'; // Asegurate de que el path sea correcto
import UserCrudForm from '../components/usuario/CrudUsuarioForm'; // Componente del formulario

const CrudUsuarioView = () => {
  return (
    <Box sx={{ display: 'flex', height: '100vh' }}>
      <CssBaseline />
      <Menu />

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          display: 'flex',
          justifyContent: 'center', // centra horizontalmente el contenido dentro de main
          alignItems: 'center', // centra verticalmente el contenido dentro de main
          height: '100vh',
        }}
      >
        <UserCrudForm />
      </Box>
    </Box>
  );
};

export default CrudUsuarioView;


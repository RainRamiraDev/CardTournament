import React from 'react';
import { Box, CssBaseline, useTheme, useMediaQuery } from '@mui/material';
import Menu from '../components/ui/menu/Menu';
import UserCrudForm from '../components/usuario/CrudUsuarioForm';

const CrudUsuarioView = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <CssBaseline />
      <Menu />

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: isMobile ? 2 : 4,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          overflowY: 'auto',
          marginTop: '64px', // <-- Compensa el AppBar fijo
        }}
      >
        <UserCrudForm />
      </Box>
    </Box>
  );
};

export default CrudUsuarioView;

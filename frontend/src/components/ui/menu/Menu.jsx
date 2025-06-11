import React, { useEffect, useState } from 'react';
import { Box, CssBaseline, Snackbar, Alert } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import { useTheme } from '@mui/material/styles';

import AppBarMenu from './AppbarMenu';
import SideDrawer from './SideDrawer';

const Menu = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const theme = useTheme();

  const [openDrawer, setOpenDrawer] = useState(false);
  const [openSnackbar, setOpenSnackbar] = useState(false);

  useEffect(() => {
    if (location.state?.fromLogin) {
      setOpenSnackbar(true);
      window.history.replaceState({}, document.title);
    }
  }, [location.state]);

  const handleToggleDrawer = () => setOpenDrawer(prev => !prev);
  const handleDrawerClose = () => setOpenDrawer(false);
  const handleCloseSnackbar = () => setOpenSnackbar(false);

  return (
    <>
      <Box sx={{ height: 0 }}>
        <CssBaseline />
        <AppBarMenu onToggleDrawer={handleToggleDrawer} />
        <SideDrawer open={openDrawer} onClose={handleDrawerClose} navigate={navigate} theme={theme} />
      </Box>

      <Snackbar
        open={openSnackbar}
        autoHideDuration={3000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert onClose={handleCloseSnackbar} severity="success" sx={{ width: '100%' }}>
          ¡Ingreso exitoso!
        </Alert>
      </Snackbar>
    </>
  );
};

export default Menu;

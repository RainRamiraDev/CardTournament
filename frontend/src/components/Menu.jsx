import React, { useEffect, useState } from 'react';
import {
  Snackbar,
  Alert,
  Box,
  Drawer,
  List,
  Divider,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  CssBaseline,
  Typography,
  IconButton
} from '@mui/material';

import { styled, useTheme } from '@mui/material/styles';
import MenuIcon from '@mui/icons-material/Menu';
import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import FormatListBulletedIcon from '@mui/icons-material/FormatListBulleted';

import AddCardIcon from '@mui/icons-material/AddCard';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';


import { useNavigate } from 'react-router-dom';
import { useLocation } from 'react-router-dom';

const drawerWidth = 240;

// AppBar con estilo dinámico según si está abierto el drawer
const AppBar = styled('div')(({ theme }) => ({
  width: '100%',
  position: 'fixed',
  top: 0,
  left: 0,
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  display: 'flex',
  alignItems: 'center',
  height: '64px',
  paddingLeft: '1rem',
  zIndex: 1201,
}));

const DrawerHeader = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(0, 1),
  ...theme.mixins.toolbar,
  justifyContent: 'flex-end',
}));

const Main = styled('main', {
  shouldForwardProp: (prop) => prop !== 'open',
})(({ theme, open }) => ({
  flexGrow: 1,
  padding: theme.spacing(3),
  marginLeft: open ? drawerWidth : 0,
  transition: theme.transitions.create(['margin'], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.enteringScreen,
  }),
}));

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

  const handleDrawerOpen = () => {
    setOpenDrawer(true);
  };

  const handleToggleDrawer = () => {
  setOpenDrawer(prev => !prev);
};


  const handleDrawerClose = () => {
    setOpenDrawer(false);
  };



  const handleCloseSnackbar = () => {
    setOpenSnackbar(false);
  };



  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <CssBaseline />
        
        <AppBar>
            <IconButton
          color="inherit"
          aria-label="toggle drawer"
          onClick={handleToggleDrawer}
          edge="start"
          sx={{ mr: 2 }}
        >
          <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div">
            Mi App
          </Typography>
        </AppBar>

        {/* Drawer lateral */}
        <Drawer
          sx={{
            width: drawerWidth,
            flexShrink: 0,
            '& .MuiDrawer-paper': {
              width: drawerWidth,
              boxSizing: 'border-box',
            },
          }}
          variant="persistent"
          anchor="left"
          open={openDrawer}
        >
          <DrawerHeader>
            <IconButton onClick={handleDrawerClose}>
              {theme.direction === 'ltr' ? <ChevronLeftIcon /> : <ChevronRightIcon />}
            </IconButton>
          </DrawerHeader>
          <Divider />
         <List>
            {[
              { text: 'Manejo usuario', icon: <PeopleAltIcon />, route: '/usuarios' },
              { text: 'Asignar carta a usuario', icon: <AddCardIcon />, route: '/asignar-carta' },
              { text: 'Cartas de jugador', icon: <FormatListBulletedIcon />, route: '/cartas' },
            ].map(({ text, icon, route }) => (
              <ListItem key={text} disablePadding>
                <ListItemButton onClick={() => navigate(route)}>
                  <ListItemIcon>{icon}</ListItemIcon>
                  <ListItemText primary={text} />
                </ListItemButton>
              </ListItem>
            ))}
          </List>

        </Drawer>

       
      </Box>

      {/* Snackbar de éxito */}
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

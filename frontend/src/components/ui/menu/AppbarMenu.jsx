import React from 'react';
import { styled } from '@mui/material/styles';
import { Typography, IconButton } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';

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

const AppBarMenu = ({ onToggleDrawer }) => (
  <AppBar>
    <IconButton
      color="inherit"
      aria-label="toggle drawer"
      onClick={onToggleDrawer}
      edge="start"
      sx={{ mr: 2 }}
    >
      <MenuIcon />
    </IconButton>
    <Typography variant="h6" noWrap component="div">
      Mi App
    </Typography>
  </AppBar>
);

export default AppBarMenu;

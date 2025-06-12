import React from 'react';
import { styled } from '@mui/material/styles';
import { Typography, IconButton, Box } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';

const AppBar = styled('div')(({ theme }) => ({
  width: '100%',
  position: 'fixed',
  top: 0,
  left: 0,
  background: `linear-gradient(90deg, ${theme.palette.primary.main} 0%, ${theme.palette.secondary.main} 100%)`,
  color: theme.palette.primary.contrastText,
  display: 'flex',
  alignItems: 'center',
  height: '64px',
  paddingLeft: '1rem',
  zIndex: 1201,
  boxShadow: '0 8px 32px 0 rgba(0,191,255,0.25), 0 1.5px 6px 0 rgba(0,0,0,0.25)',
  backdropFilter: 'blur(8px)',
  borderBottom: `2px solid ${theme.palette.secondary.main}`,
  transition: 'background 0.4s, box-shadow 0.4s',
  '@media (max-width:600px)': {
    height: '56px',
    paddingLeft: '0.5rem',
  },
}));

const Title = styled(Typography)(({ theme }) => ({
  fontFamily: "'Cinzel', serif",
  fontWeight: 900,
  letterSpacing: '2px',
  textShadow: `0 0 8px ${theme.palette.primary.main}, 0 0 2px #000`,
  fontSize: '1.5rem',
  [theme.breakpoints.down('sm')]: {
    fontSize: '1.1rem',
    letterSpacing: '1px',
  },
}));

const AppBarMenu = ({ onToggleDrawer }) => (
  <AppBar>
    <IconButton
      color="inherit"
      aria-label="toggle drawer"
      onClick={onToggleDrawer}
      edge="start"
      sx={{
        mr: 2,
        background: 'rgba(255,255,255,0.08)',
        borderRadius: 2,
        transition: 'background 0.3s',
        '&:hover': {
          background: 'rgba(0,255,255,0.18)',
        },
      }}
      size="large"
    >
      <MenuIcon sx={{ fontSize: 32 }} />
    </IconButton>
    <Box sx={{ flexGrow: 1 }}>
      <Title variant="h6" noWrap component="div">
        CARD APP
      </Title>
    </Box>
  </AppBar>
);

export default AppBarMenu;

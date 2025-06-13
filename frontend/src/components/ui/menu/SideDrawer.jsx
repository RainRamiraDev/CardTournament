import React from 'react';
import {
  Drawer,
  Divider,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Box
} from '@mui/material';
import { styled } from '@mui/material/styles';

import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import FormatListBulletedIcon from '@mui/icons-material/FormatListBulleted';
import AddCardIcon from '@mui/icons-material/AddCard';

const drawerWidth = 240;

const StyledDrawer = styled(Drawer)(({ theme }) => ({
  '& .MuiDrawer-paper': {
    width: drawerWidth,
    height: '100vh',
    minHeight: '100vh',
    boxSizing: 'border-box',
    background: theme.palette.background.paper,
    borderRight: `2px solid ${theme.palette.primary.main}`,
    color: theme.palette.text.primary,
    boxShadow: `0 0 20px 2px ${theme.palette.primary.main}22`,
    backdropFilter: 'blur(6px)',
    borderBottomRightRadius: 24,
    borderTopRightRadius: 0,
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'flex-start',
    paddingBottom: theme.spacing(2),
    overflow: 'hidden',
    paddingTop: theme.spacing(2),
  },
}));

const DrawerHeader = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(0, 1),
  ...theme.mixins.toolbar,
  minHeight: 56, // Ajusta el alto para que no quede muy pequeño ni muy grande
  height: 56,
  justifyContent: 'flex-end',
  background: theme.palette.background.paper, // Fondo sólido
  boxShadow: 'none',
  borderTopRightRadius: 0,
  borderBottomRightRadius: 0,
}));

const StyledListItemButton = styled(ListItemButton)(({ theme }) => ({
  borderRadius: 8,
  margin: '4px 8px',
  transition: 'background 0.2s, transform 0.2s',
  '&:hover': {
    background: theme.palette.primary.main + '22',
    transform: 'scale(1.03)',
    boxShadow: `0 2px 8px 0 ${theme.palette.primary.main}33`,
  },
}));

const StyledListItemIcon = styled(ListItemIcon)(({ theme }) => ({
  color: theme.palette.primary.main,
  minWidth: 40,
  fontSize: 26,
}));

const SideDrawer = ({ open, onClose, navigate, theme }) => {
  const menuItems = [
    { text: 'Manejo usuario', icon: <PeopleAltIcon fontSize="medium" />, route: '/usuarios' },
    { text: 'Asignar carta a usuario', icon: <AddCardIcon fontSize="medium" />, route: '/cartas' },
    { text: 'Cartas de jugador', icon: <FormatListBulletedIcon fontSize="medium" />, route: '/CardsUsuarios' },
  ];

  return (
    <StyledDrawer
      variant="persistent"
      anchor="left"
      open={open}
    >
      <DrawerHeader />
      <Divider />
      <List>
        {menuItems.map(({ text, icon, route }) => (
          <ListItem key={text} disablePadding>
            <StyledListItemButton onClick={() => navigate(route)}>
              <StyledListItemIcon>{icon}</StyledListItemIcon>
              <ListItemText
                primary={text}
                primaryTypographyProps={{
                  fontWeight: 600,
                  letterSpacing: 1,
                  sx: { color: 'inherit' }
                }}
              />
            </StyledListItemButton>
          </ListItem>
        ))}
      </List>
      <Box sx={{ flexGrow: 1 }} />
    </StyledDrawer>
  );
};

export default SideDrawer;

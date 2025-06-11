import React from 'react';
import {
  Drawer,
  Divider,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  IconButton
} from '@mui/material';
import { styled } from '@mui/material/styles';

import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import FormatListBulletedIcon from '@mui/icons-material/FormatListBulleted';
import AddCardIcon from '@mui/icons-material/AddCard';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';

const drawerWidth = 240;

const DrawerHeader = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(0, 1),
  ...theme.mixins.toolbar,
  justifyContent: 'flex-end',
}));

const SideDrawer = ({ open, onClose, navigate, theme }) => {
  const menuItems = [
    { text: 'Manejo usuario', icon: <PeopleAltIcon />, route: '/usuarios' },
    { text: 'Asignar carta a usuario', icon: <AddCardIcon />, route: '/cartas' },
    { text: 'Cartas de jugador', icon: <FormatListBulletedIcon />, route: '/CardsUsuarios' },
  ];

  return (
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
      open={open}
    >
      <DrawerHeader>
        <IconButton onClick={onClose}>
          {theme.direction === 'ltr' ? <ChevronLeftIcon /> : <ChevronRightIcon />}
        </IconButton>
      </DrawerHeader>
      <Divider />
      <List>
        {menuItems.map(({ text, icon, route }) => (
          <ListItem key={text} disablePadding>
            <ListItemButton onClick={() => navigate(route)}>
              <ListItemIcon>{icon}</ListItemIcon>
              <ListItemText primary={text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Drawer>
  );
};

export default SideDrawer;

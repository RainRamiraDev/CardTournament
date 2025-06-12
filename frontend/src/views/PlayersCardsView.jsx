import React from 'react';
import { Box, CssBaseline, useTheme, useMediaQuery } from '@mui/material';
import Menu from '../components/ui/menu/Menu';
import PlayerCards from '../components/cartas/PlayersCards';

const PlayerCardsView = () => {
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
          justifyContent: isMobile ? 'flex-start' : 'center',
          overflowY: 'auto',
        }}
      >
        <PlayerCards />
      </Box>
    </Box>
  );
};

export default PlayerCardsView;

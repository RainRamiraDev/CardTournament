import React from 'react';
import { Box, Typography, useTheme } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';
import PerButton from './PerButton';

const Error404 = () => {
  const navigate = useNavigate();
  const theme = useTheme();

  const handleGoHome = () => {
    navigate('/menu');
  };

  return (
    <Box
      sx={{
        position: 'fixed',
        top: 0,
        left: 0,
        width: '100vw',
        height: '100vh',
        zIndex: 2000,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        textAlign: 'center',
        background: `radial-gradient(circle at 60% 40%, ${theme.palette.background.paper} 70%, ${theme.palette.background.default} 100%)`,
        p: 2,
        boxSizing: 'border-box',
      }}
    >
      {/* Icono de error animado */}
      <Box
        sx={{
          mb: { xs: 2, sm: 3 },
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          animation: 'error-bounce 1.2s infinite',
        }}
      >
        <ErrorOutlineIcon
          sx={{
            fontSize: { xs: 60, sm: 90, md: 120 },
            color: theme.palette.error.main,
            filter: `drop-shadow(0 0 16px ${theme.palette.error.main}88)`,
          }}
        />
        <style>
          {`
            @keyframes error-bounce {
              0% { transform: translateY(0);}
              30% { transform: translateY(-10px);}
              50% { transform: translateY(0);}
              70% { transform: translateY(-6px);}
              100% { transform: translateY(0);}
            }
          `}
        </style>
      </Box>

      <Typography
        variant="h1"
        sx={{
          color: theme.palette.error.main,
          fontWeight: 900,
          fontSize: { xs: '6rem', sm: '8rem', md: '11rem' },
          textShadow: `0 0 32px ${theme.palette.error.main}99, 0 0 12px #000`,
          mb: 2,
        }}
        gutterBottom
      >
        404
      </Typography>
      <Typography
        variant="h4"
        sx={{
          fontWeight: 700,
          color: theme.palette.text.primary,
          mb: 1,
          textShadow: `0 0 12px ${theme.palette.primary.main}33`,
          fontSize: { xs: '1.5rem', sm: '2.2rem', md: '2.7rem' },
        }}
        gutterBottom
      >
        Página no encontrada
      </Typography>
      <Typography
        variant="body1"
        sx={{
          color: theme.palette.text.secondary,
          mb: 3,
          maxWidth: 480,
          fontSize: { xs: '1.1rem', sm: '1.3rem', md: '1.4rem' },
        }}
        gutterBottom
      >
        Lo sentimos, la página que estás buscando no existe o fue movida.
      </Typography>
      <PerButton
        variant="contained"
        color="primary"
        onClick={handleGoHome}
        sx={{
          fontWeight: 700,
          fontSize: { xs: '0.85rem', sm: '0.95rem' },
          px: 2,
          py: 0.5,
          borderRadius: 2,
          boxShadow: `0 0 8px ${theme.palette.primary.main}`,
          textTransform: 'uppercase',
          minWidth: 90,
          maxWidth: 180,
          mx: 'auto',
          mt: 1,
        }}
      >
        Ir al inicio
      </PerButton>
    </Box>
  );
};

export default Error404;

// src/theme.js
import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#00BFFF', // azul brillante, tipo cristales mágicos
      contrastText: '#FFFFFF',
    },
    secondary: {
      main: '#FFB347', // naranja cálido, luz de fuego
      contrastText: '#1B1B1B',
    },
    background: {
      default: '#121212',  // fondo oscuro neutro
      paper: '#1E1E2F',    // cartas o paneles con un azul muy oscuro
    },
    text: {
      primary: '#E0E0E0',
      secondary: '#A0A0A0',
      disabled: '#555555',
    },
    error: {
      main: '#F44336',
    },
    warning: {
      main: '#FF9800',
    },
    info: {
      main: '#2196F3',
    },
    success: {
      main: '#4CAF50',
    },
  },

  typography: {
    fontFamily: `'Cinzel Decorative', serif`,
    h1: {
      fontWeight: 800,
      letterSpacing: '2px',
      color: '#00BFFF',
      textShadow: '0 0 8px #00BFFF',
    },
    h2: {
      fontWeight: 700,
      letterSpacing: '1.5px',
      color: '#FFB347',
      textShadow: '0 0 6px #FFB347',
    },
    button: {
      textTransform: 'uppercase',
      fontWeight: 700,
      letterSpacing: '1.2px',
      fontSize: '0.9rem',
      color: '#1B1B1B',
      textShadow: '1px 1px 2px rgba(0,0,0,0.8)',
    },
  },

  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 10,
          padding: '10px 25px',
          backgroundImage:
            'linear-gradient(45deg, #00BFFF 0%, #0077B6 100%)',
          boxShadow: '0 4px 15px rgba(0,191,255,0.6)',
          transition: 'all 0.3s ease',
          fontWeight: 700,
          '&:hover': {
            backgroundImage:
              'linear-gradient(45deg, #33CCFF 0%, #0099CC 100%)',
            boxShadow: '0 6px 20px rgba(51,204,255,0.9)',
            transform: 'scale(1.05)',
          },
          '&:disabled': {
            backgroundColor: '#555555',
            boxShadow: 'none',
            color: '#aaa',
          },
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          borderRadius: 15,
          backgroundImage:
            'radial-gradient(circle at center, #1E1E2F, #12121E 90%)',
          boxShadow:
            '0 8px 25px rgba(0,0,0,0.8), inset 0 0 10px #00BFFF',
          padding: '1.5rem',
          border: '1.5px solid #00BFFF',
          color: '#E0E0E0',
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 14,
          backgroundColor: '#27354B',
          border: '2px solid #00BFFF',
          boxShadow:
            '0 0 12px 3px rgba(0,191,255,0.7), 0 6px 20px rgba(0,0,0,0.9)',
          transition: 'transform 0.3s ease, box-shadow 0.3s ease',
          '&:hover': {
            transform: 'scale(1.04)',
            boxShadow:
              '0 0 20px 5px #33CCFF, 0 10px 30px rgba(0,0,0,1)',
          },
        },
      },
    },
     MuiCssBaseline: {
      styleOverrides: {
        body: {
          backgroundImage: `url('https://plus.unsplash.com/premium_photo-1699967711142-bc002e47990c?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D')`,
          backgroundRepeat: 'repeat',
          backgroundSize: 'auto',
          backgroundPosition: 'center',
          // opcional: efecto de opacidad o filtro
          
        },
      },
    },
    MuiSnackbar: {
      styleOverrides: {
        root: {
          '& .MuiAlert-root': {
            borderRadius: 10,
            backgroundColor: '#FFB347',
            color: '#FFB347',
            fontWeight: 'bold',
            boxShadow: '0 0 10px #FFB347',
            textShadow: '0 0 3px rgba(0,0,0,0.7)',
          },
        },
      },
    },
    
  },
});

export default theme;

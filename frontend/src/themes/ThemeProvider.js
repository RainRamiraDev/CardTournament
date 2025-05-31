// src/theme.js
import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    mode: 'dark', // 🌓 Estilo nocturno elegante
    primary: {
      main: '#D4AF37', // dorado para botones clave
    },
    secondary: {
      main: '#C62828', // rojo intenso
    },
    background: {
      default: '#1B1B1B', // fondo tipo mesa
      paper: '#2E2E2E',   // cartas o componentes
    },
    text: {
      primary: '#ffffff',
      secondary: '#BDBDBD',
    },
  },
  typography: {
    fontFamily: `'Cinzel', 'Georgia', serif`, // estilo clásico de cartas
    h1: {
      fontWeight: 700,
      letterSpacing: '2px',
    },
    button: {
      textTransform: 'uppercase',
      fontWeight: 'bold',
      letterSpacing: '1px',
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          padding: '10px 20px',
          boxShadow: '0 4px 12px rgba(0,0,0,0.3)',
          transition: 'all 0.3s ease',
          '&:hover': {
            transform: 'scale(1.05)',
            boxShadow: '0 6px 18px rgba(0,0,0,0.5)',
          },
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          borderRadius: 16,
          padding: '1rem',
          backgroundImage: 'linear-gradient(145deg, #2e2e2e, #252525)',
          boxShadow: '0 4px 12px rgba(0,0,0,0.4)',
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 16,
          backgroundColor: '#333',
          border: '2px solid #D4AF37',
        },
      },
    },
  },
});

export default theme;

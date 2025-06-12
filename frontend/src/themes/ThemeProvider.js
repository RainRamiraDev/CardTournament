// src/theme.js
import { createTheme } from '@mui/material/styles';

// === PALETA DE COLORES (usamos como variables base) ===
const colors = {
  blueCrystal: '#00BFFF',
  blueCrystalDark: '#0077B6',
  blueCrystalHover: '#33CCFF',
  blueCrystalHoverDark: '#0099CC',
  fireLight: '#FFB347',
  fireText: '#1B1B1B',
  darkBg: '#121212',
  darkPaper: '#1E1E2F',
  darkCard: '#27354B',
  textPrimary: '#E0E0E0',
  textSecondary: '#A0A0A0',
  textDisabled: '#555555',
  error: '#F44336',
  warning: '#FF9800',
  info: '#2196F3',
  success: '#4CAF50',
  cardPreviewBg: '#f9f9f9',
  cardPreviewBorder: '#ccc',
};

// === FUENTE BASE ===
const font = `'Cinzel Decorative', serif`;

// === THEME ===
const theme = createTheme({

  breakpoints: {
    values: {
      xs: 0,
      sm: 500,   // cambié sm de 600 a 500px para que se active antes
      md: 900,
      lg: 1200,
      xl: 1536,
    },
  },



  palette: {
    mode: 'dark',
    primary: {
      main: colors.blueCrystal,
      contrastText: '#FFFFFF',
    },
    secondary: {
      main: colors.fireLight,
      contrastText: colors.fireText,
    },
    background: {
      default: colors.darkBg,
      paper: colors.darkPaper,
    },
    text: {
      primary: colors.textPrimary,
      secondary: colors.textSecondary,
      disabled: colors.textDisabled,
    },
    error: { main: colors.error },
    warning: { main: colors.warning },
    info: { main: colors.info },
    success: { main: colors.success },
  },

  typography: {
    fontFamily: font,
    h1: {
      fontWeight: 800,
      letterSpacing: '2px',
      color: colors.blueCrystal,
      textShadow: `0 0 8px ${colors.blueCrystal}`,
    },
    h2: {
      fontWeight: 700,
      letterSpacing: '1.5px',
      color: colors.fireLight,
      textShadow: `0 0 6px ${colors.fireLight}`,
    },
    button: {
      textTransform: 'uppercase',
      fontWeight: 700,
      letterSpacing: '1.2px',
      fontSize: '0.9rem',
      color: colors.fireText,
      textShadow: '1px 1px 2px rgba(0,0,0,0.8)',
    },
  },

  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 10,
          padding: '10px 25px',
          backgroundImage: `linear-gradient(45deg, ${colors.blueCrystal} 0%, ${colors.blueCrystalDark} 100%)`,
          boxShadow: `0 4px 15px rgba(0,191,255,0.6)`,
          transition: 'all 0.3s ease',
          fontWeight: 700,
          '&:hover': {
            backgroundImage: `linear-gradient(45deg, ${colors.blueCrystalHover} 0%, ${colors.blueCrystalHoverDark} 100%)`,
            boxShadow: `0 6px 20px rgba(51,204,255,0.9)`,
            transform: 'scale(1.05)',
          },
          '&:disabled': {
            backgroundColor: colors.textDisabled,
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
          backgroundImage: `radial-gradient(circle at center, ${colors.darkPaper}, ${colors.darkBg} 90%)`,
          boxShadow: `0 8px 25px rgba(0,0,0,0.8), inset 0 0 10px ${colors.blueCrystal}`,
          padding: '1.5rem',
          border: `1.5px solid ${colors.blueCrystal}`,
          color: colors.textPrimary,
        },
      },
    },

    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 14,
          backgroundColor: colors.darkCard,
          border: `2px solid ${colors.blueCrystal}`,
          boxShadow: `0 0 12px 3px rgba(0,191,255,0.7), 0 6px 20px rgba(0,0,0,0.9)`,
          transition: 'transform 0.3s ease, box-shadow 0.3s ease',
          '&:hover': {
            transform: 'scale(1.04)',
            boxShadow: `0 0 20px 5px ${colors.blueCrystalHover}, 0 10px 30px rgba(0,0,0,1)`,
          },
        },
      },
    },

    MuiCardContent: {
      variants: [
        {
          props: { variant: 'cardPreview' },
          style: {
            background: colors.cardPreviewBg,
            padding: '16px',
            borderRadius: '10px',
            border: `1px solid ${colors.cardPreviewBorder}`,
            marginBottom: '16px',
            color: '#333',
          },
        },
      ],
    },

    MuiFormControl: {
      styleOverrides: {
        root: {
          marginBottom: '24px',
        },
      },
    },

    MuiBox: {
      variants: [
        {
          props: { variant: 'magic' },
         style: {
            backgroundColor: '#f0f0ff',
            border: '2px solid #7e57c2',
            borderRadius: 12,
            
            padding: 16,
            boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
            transition: 'transform 0.2s ease-in-out',
            '&:hover': {
              transform: 'scale(1.02)',
            },
          },
        },
      ],
    },

    MuiCssBaseline: {
      styleOverrides: {
        body: {
          backgroundImage: `url('https://plus.unsplash.com/premium_photo-1699967711142-bc002e47990c?q=80&w=1974&auto=format&fit=crop')`,
          backgroundRepeat: 'repeat',
          backgroundSize: 'auto',
          backgroundPosition: 'center',
          backgroundAttachment: 'fixed',
        },
      },
    },
    

    MuiSnackbar: {
      styleOverrides: {
        root: {
          '& .MuiAlert-root': {
            borderRadius: 10,
            backgroundColor: colors.info,
            color: colors.textPrimary,
            fontWeight: 'bold',
            boxShadow: `0 0 10px ${colors.fireLight}`,
     
          },
        },
      },
    },
  },
});

// === ESTILOS PERSONALIZADOS PARA ROLES (externos al theme) ===
export const customRoles = {
  1: { color: '#FF0000', fontWeight: 'bold' },       // Organizer rojo
  2: { color: '#0077FF', fontWeight: 600 },          // Administrator azul
  3: { color: '#00AA00', fontWeight: 600 },          // Judge verde
  4: { color: '#AAAAAA', fontWeight: 'normal' },     // Player gris
};

export default theme;

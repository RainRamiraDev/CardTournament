import React from 'react';
import { Box, Typography} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import PerButton from './PerButton';

const Error404 = () => {
  const navigate = useNavigate();

  const handleGoHome = () => {
    navigate('/menu'); // o '/' según tu ruta principal
  };

  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      height="100vh"
      textAlign="center"
      p={2}
    >
      <Typography variant="h1" color="error" gutterBottom>
        404
      </Typography>
      <Typography variant="h5" gutterBottom>
        Página no encontrada
      </Typography>
      <Typography variant="body1" gutterBottom>
        Lo sentimos, la página que estás buscando no existe.
      </Typography>
      <PerButton variant="contained" color="primary" onClick={handleGoHome}>
        Ir al inicio
      </PerButton>
    </Box>
  );
};

export default Error404;

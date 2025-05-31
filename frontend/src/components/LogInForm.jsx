import React, { useState } from 'react';
import { Box, Typography, Paper } from '@mui/material';
import TextField from './ui/TextField.jsx';
import Button from './ui/Button.jsx';
import LogInService from '../services/logInService.js'

const LogInForm = () => {
  const [form, setForm] = useState({ fullname: '', passcode: '' });
  const [errors, setErrors] = useState({});

  const handleChange = e => setForm({ ...form, [e.target.name]: e.target.value });

  const validate = () => {
    const newErrors = {};
    if (!form.fullname) newErrors.fullname = 'El fullname es requerido';
    if (!form.passcode) newErrors.passcode = 'La contraseña es requerida';
    return newErrors;
  };

const handleSubmit = async (e) => {
  e.preventDefault();
  const validationErrors = validate();
  if (Object.keys(validationErrors).length > 0) {
    setErrors(validationErrors);
  } else {
    try {
      // Llama al servicio pasando fullname y passcode del estado form
      const resultado = await LogInService(form.fullname, form.passcode);
      console.log('Login exitoso:', resultado);
      // Aquí puedes manejar lo que quieras después del login, 
      // como guardar un token, redireccionar, etc.
    } catch (error) {
      // Si hay error en el login, mostrarlo (puedes mejorar la UI)
      setErrors({ general: error.message });
    }
  }
};


  return (
    <Box sx={{ width: 350, margin: 'auto', marginTop: 10 }}>
      <Paper elevation={3} sx={{ padding: 4 }}>
        <Typography variant="h5" component="h1" gutterBottom>
          Iniciar sesión
        </Typography>
        <form onSubmit={handleSubmit} noValidate>
          <TextField
            label="fullname"
            name="fullname"
            type="fullname"
            value={form.fullname}
            onChange={handleChange}
            error={Boolean(errors.fullname)}
            helperText={errors.fullname}
          />
          <TextField
            label="passcode"
            name="passcode"
            type="passcode"
            value={form.passcode}
            onChange={handleChange}
            error={Boolean(errors.passcode)}
            helperText={errors.passcode}
          />
          <Button type="submit">Ingresar</Button>
        </form>
      </Paper>
    </Box>
  );
};

export default LogInForm;

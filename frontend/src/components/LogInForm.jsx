import React, { useState } from 'react';
import { Box, Typography, Paper, Snackbar, Alert } from '@mui/material';
import TextField from './ui/TextField.jsx';
import Button from './ui/Button.jsx';
import LogInService from '../services/logInService.js';
import { useNavigate } from 'react-router-dom';

import { useDispatch } from 'react-redux';
import { login } from '../store/authSlice';

const LogInForm = () => {
   const dispatch = useDispatch();
  const navigate = useNavigate();
  const [form, setForm] = useState({ fullname: '', passcode: '' });
  const [errors, setErrors] = useState({});
  const [openSnackbar, setOpenSnackbar] = useState(false);

  const handleChange = e => setForm({ ...form, [e.target.name]: e.target.value });

  const validate = () => {
    const newErrors = {};
    const fullnameRegex = /^[A-Za-zÁÉÍÓÚÑáéíóúñ\s]+$/;
    if (!form.fullname.trim()) {
      newErrors.fullname = 'El nombre completo es requerido';
    }
    const passcodeRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{3,}$/;
    if (!form.passcode) {
      newErrors.passcode = 'La contraseña es requerida';
    }
    return newErrors;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
    } else {
      try {
        const resultado = await LogInService(form.fullname, form.passcode);
        console.log('Login exitoso:', resultado);
        setErrors({});
        // Navega pasando estado para mostrar Snackbar en la siguiente página
        dispatch(login()); // marca como autenticado
        navigate('/menu', { state: { fromLogin: true } });
      } catch (error) {
        setErrors({ general: error.message });
      }
    }
  };

  const handleCloseSnackbar = (event, reason) => {
    if (reason === 'clickaway') return;
    setOpenSnackbar(false);
  };

  return (
    <Box sx={{ width: 350, margin: 'auto', marginTop: 10 }}>
      <Paper elevation={3} sx={{ padding: 4 }}>
        <Typography variant="h5" component="h1" gutterBottom>
          Iniciar sesión
        </Typography>
        <form onSubmit={handleSubmit} noValidate>
          <TextField
            label="Nombre completo"
            name="fullname"
            type="text"
            value={form.fullname}
            onChange={handleChange}
            error={Boolean(errors.fullname)}
            helperText={errors.fullname}
          />
          <TextField
            label="Contraseña"
            name="passcode"
            type="password"
            value={form.passcode}
            onChange={handleChange}
            error={Boolean(errors.passcode)}
            helperText={errors.passcode}
          />
          {errors.general && (
            <Typography color="error" variant="body2" sx={{ mt: 1 }}>
              {errors.general}
            </Typography>
          )}
          <Button type="submit">Ingresar</Button>
        </form>
      </Paper>

      {/* Este Snackbar no es necesario porque el mensaje se mostrará en /menu */}
    </Box>
  );
};

export default LogInForm;

// src/components/LogInForm.jsx
import React from 'react';
import { Box, Typography, Paper } from '@mui/material';
import PerTextField from './ui/PerTextField.jsx';
import PerButton from './ui/PerButton.jsx';

import {logInUser} from '../services/logInService.js';

import { useNavigate } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { login } from '../store/authSlice';
import { useForm } from '../hooks/useForm.js';
import { validateForm } from '../utils/validateForm.js';

const LogInForm = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const {
  form,
  errors,
  handleChange,
  validate,
  setErrors,
} = useForm({ fullname: '', passcode: '' }, validateForm);


  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) 
      return;

    try {
      const resultado = await logInUser(form.fullname, form.passcode);
      console.log('Login exitoso:', resultado);
      dispatch(login());
      navigate('/menu', { state: { fromLogin: true } });
    } catch (error) {
      setErrors({ general: error.message });
    }
  };

  return (
    <Box sx={{ width: 350, margin: 'auto', marginTop: 10 }}>
      <Paper elevation={3} sx={{ padding: 4 }}>
        <Typography variant="h5" component="h1" gutterBottom>
          Iniciar sesión
        </Typography>
        <form onSubmit={handleSubmit} noValidate>
          <PerTextField
            label="Nombre completo"
            name="fullname"
            type="text"
            value={form.fullname}
            onChange={handleChange}
            error={Boolean(errors.fullname)}
            helperText={errors.fullname}
          />
          <PerTextField
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
          <PerButton type="submit">Ingresar</PerButton>
        </form>
      </Paper>
    </Box>
  );
};

export default LogInForm;

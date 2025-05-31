import React, { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Snackbar,
  Alert,
  MenuItem,
  Select,
  InputLabel,
  FormControl,
} from '@mui/material';
import Button from './ui/Button';
import TextField from './ui/TextField';
import { useSnackbar } from '../hooks/useSnackbar';
import { useForm } from '../hooks/useForm';

const initialForm = {
  Id_User: '',
  Id_Country: '',
  Id_Rol: '',
  Fullname: '',
  Passcode: '',
  Alias: '',
  Email: '',
  Avatar_Url: '',
};

const CrudUsuarioForm = () => {
   const [action, setAction] = useState('CREATE');
  const { open, showSnackbar, closeSnackbar } = useSnackbar();

  // Pasamos action al hook useForm para que la validación sea acorde
  const { form, errors, handleChange, validate, setForm, setErrors, resetForm } = useForm(initialForm, action);

  const handleActionChange = (e) => {
    setAction(e.target.value);
    resetForm();
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!validate()) {
      console.log('Errores de validación:', errors);
      return;
    }

    console.log('Acción:', action);
    console.log('Datos:', form);
    showSnackbar();
  };


  const isCreate = action === 'CREATE';
  const isAlter = action === 'ALTER';
  const isDelete = action === 'DELETE';

  return (
    <Paper elevation={3} sx={{ p: 4, maxWidth: 500, mx: 'auto' }}>
      <Typography variant="h6" gutterBottom>
        Gestión de Usuario ({action})
      </Typography>

      <FormControl fullWidth sx={{ mb: 3 }}>
        <InputLabel>Acción</InputLabel>
        <Select value={action} label="Acción" onChange={handleActionChange}>
          <MenuItem value="CREATE">Crear</MenuItem>
          <MenuItem value="ALTER">Modificar</MenuItem>
          <MenuItem value="DELETE">Eliminar</MenuItem>
        </Select>
      </FormControl>

      <Box component="form" onSubmit={handleSubmit} noValidate>
        {(isAlter || isDelete) && (
          <TextField
            label="ID de Usuario"
            name="Id_User"
            value={form.Id_User}
            onChange={handleChange}
            error={!!errors.Id_User}
            helperText={errors.Id_User}
            required
          />
        )}

        {(isCreate || isAlter) && (
          <>
            <TextField
              label="ID País"
              name="Id_Country"
              value={form.Id_Country}
              onChange={handleChange}
              error={!!errors.Id_Country}
              helperText={errors.Id_Country}
              required
            />
            <TextField
              label="ID Rol"
              name="Id_Rol"
              value={form.Id_Rol}
              onChange={handleChange}
              error={!!errors.Id_Rol}
              helperText={errors.Id_Rol}
              required
            />
            <TextField
              label="Nombre completo"
              name="Fullname"
              value={form.Fullname}
              onChange={handleChange}
              error={!!errors.Fullname}
              helperText={errors.Fullname}
              required
            />
            {isCreate && (
              <TextField
                label="Contraseña"
                name="Passcode"
                type="password"
                value={form.Passcode}
                onChange={handleChange}
                error={!!errors.Passcode}
                helperText={errors.Passcode}
                required
              />
            )}
            <TextField
              label="Alias"
              name="Alias"
              value={form.Alias}
              onChange={handleChange}
              error={!!errors.Alias}
              helperText={errors.Alias}
              required
            />
            <TextField
              label="Email"
              name="Email"
              type="email"
              value={form.Email}
              onChange={handleChange}
              error={!!errors.Email}
              helperText={errors.Email}
              required
            />
            <TextField
              label="Avatar URL"
              name="Avatar_Url"
              value={form.Avatar_Url}
              onChange={handleChange}
              error={!!errors.Avatar_Url}
              helperText={errors.Avatar_Url}
              required
            />
          </>
        )}

        <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
          {action === 'CREATE'
            ? 'Crear'
            : action === 'ALTER'
            ? 'Modificar'
            : 'Eliminar'}
        </Button>
      </Box>

      <Snackbar
        open={open}
        autoHideDuration={3000}
        onClose={closeSnackbar}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert severity="success" onClose={closeSnackbar}>
          ¡Acción {action} ejecutada exitosamente!
        </Alert>
      </Snackbar>
    </Paper>
  );
};

export default CrudUsuarioForm;

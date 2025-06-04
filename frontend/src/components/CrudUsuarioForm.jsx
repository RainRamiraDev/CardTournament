import React, { useState, useEffect } from 'react';
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
import PerButton from './ui/PerButton';
import PerTextField from './ui/PerTextField';
import { useSnackbar } from '../hooks/useSnackbar';
import { useForm } from '../hooks/useForm';
import { createUser, alterUser, deactivateUser, getCountries, getRoles } from '../services/userService';

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

export const CrudUsuarioForm = () => {
  const [action, setAction] = useState('CREATE');
  const { open, showSnackbar, closeSnackbar } = useSnackbar();
  const [countries, setCountries] = useState([]);
  const [roles, setRoles] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const paises = await getCountries();
        const roles = await getRoles();
        setCountries(paises);
        setRoles(roles);
      } catch (error) {
        console.error('Error cargando datos:', error.message);
      }
    };

    fetchData();
  }, []);





  // Pasamos action al hook useForm para que la validación sea acorde
  const { form, errors, handleChange, validate, setForm, setErrors, resetForm } = useForm(initialForm, action);

  const isCreate = action === 'CREATE';
  const isAlter = action === 'ALTER';
  const isDelete = action === 'DELETE';


  const handleActionChange = (e) => {
    setAction(e.target.value)
    resetForm();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) {
      console.log('Errores de validación:', errors)
      return; 
    }

    try {
    if (action === 'CREATE') {
      await createUser(
        form.Id_Country,
        form.Id_Rol,
        form.Passcode,
        form.Fullname,
        form.Alias,
        form.Email,
        form.Avatar_Url
      );
    } else if (action === 'ALTER') {
      await alterUser(
        form.Id_User,
        form.Id_Country,
        form.Id_Rol,
        form.Fullname,
        form.Alias,
        form.Email,
        form.Avatar_Url
      );
    } else if (action === 'DELETE') {
      await deactivateUser(form.Id_User)
    }

    console.log('Acción:', action)
    console.log('Datos:', form)
    showSnackbar();
  }
  catch (error) {
    console.error('Error al ejecutar la acción:', error)
  }
}


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
          <PerTextField
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
            <Select
              label="País"
              name="Id_Country"
              value={form.Id_Country}
              onChange={handleChange}
              error={!!errors.Id_Country}
              helperText={errors.Id_Country}
              required
            />
            <Select
              label="Rol"
              name="Id_Rol"
              value={form.Id_Rol}
              onChange={handleChange}
              error={!!errors.Id_Rol}
              helperText={errors.Id_Rol}
              required
            />
            <PerTextField
              label="Nombre completo"
              name="Fullname"
              value={form.Fullname}
              onChange={handleChange}
              error={!!errors.Fullname}
              helperText={errors.Fullname}
              required
            />
            {isCreate && (
              <PerTextField
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
            <PerTextField
              label="Alias"
              name="Alias"
              value={form.Alias}
              onChange={handleChange}
              error={!!errors.Alias}
              helperText={errors.Alias}
              required
            />
            <PerTextField
              label="Email"
              name="Email"
              type="email"
              value={form.Email}
              onChange={handleChange}
              error={!!errors.Email}
              helperText={errors.Email}
              required
            />
            <PerTextField
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

        <PerButton type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
          {action === 'CREATE'
            ? 'Crear'
            : action === 'ALTER'
            ? 'Modificar'
            : 'Eliminar'}
        </PerButton>
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
}

export default CrudUsuarioForm;

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
import { createUser, alterUser, deactivateUser, getCountries, getRoles,getAllUsers } from '../services/userService';
import { useTheme } from '@mui/material/styles';
import { useNavigate } from 'react-router-dom';


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
  const { open, message, showSnackbar, closeSnackbar } = useSnackbar();
  const [countries, setCountries] = useState([]);
  const [roles, setRoles] = useState([]);
  const [users, setUsers] = useState([]);
  const theme = useTheme();  // obtienes el tema actual
  const navigate = useNavigate();


  useEffect(() => {
    const fetchData = async () => {
      try {
        const paises = await getCountries();
        const roles = await getRoles();
        const usuarios = await getAllUsers(); // 👈 tu función para traer usuarios
        setCountries(paises);
        setRoles(roles);
        setUsers(usuarios);
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
      showSnackbar('Usuario creado exitosamente');
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
      showSnackbar('Usuario modificado exitosamente');
    } else if (action === 'DELETE') {
      await deactivateUser(form.Id_User)
      showSnackbar('Usuario eliminado exitosamente');
    }

    console.log('Acción:', action)
    console.log('Datos:', form)
    showSnackbar();

// Espera 3 segundos (igual que el autoHideDuration del Snackbar), luego redirige
    setTimeout(() => {
      navigate('/menu');
    }, 3000);



  }
  catch (error) {
    console.error('Error al ejecutar la acción:', error)
  }
}

const roleStyles = {
    1: { color: theme.palette.primary.main, fontWeight: 'bold' },      // Organizer - azul
    2: { color: theme.palette.secondary.main, fontWeight: 'bold' },    // Administrator - naranja
    3: { color: theme.palette.info.main, fontWeight: 'bold' },         // Judge - azul info
    4: { color: theme.palette.success.main, fontWeight: 'normal' },    // Player - verde
  };


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
        <FormControl fullWidth sx={{ mb: 3 }} error={!!errors.Id_User}>
          <InputLabel id="label-usuario">Usuario</InputLabel>
          <Select
            labelId="label-usuario"
            name="Id_User"
            value={form.Id_User || ''}
            onChange={handleChange}
            label="Usuario"
            required
            renderValue={(selectedId) => {
          const user = users.find((u) => u.id_user === selectedId);
          if (!user) return '';
          const rolName = roles.find((r) => r.id_rol === user.id_rol)?.rol || 'Sin rol';
          return `${user.fullname} [${rolName}]`;
        }}

          >
              {users.map((user) => {
              const rolName = roles.find((r) => r.id_rol === user.id_rol)?.rol || 'Sin rol';
              const style = roleStyles[user.id_rol] || { color: 'inherit', fontWeight: 'normal' };

              return (
                <MenuItem key={user.id_user} value={user.id_user}>
                  <span style={style}>
                    {user.fullname} [{rolName.toUpperCase()}]
                  </span>
                </MenuItem>
              );
            })}

          </Select>
          {errors.Id_User && <FormHelperText>{errors.Id_User}</FormHelperText>}
        </FormControl>
      )}

      {(isCreate || isAlter) && (
        <>
          <FormControl fullWidth sx={{ mb: 3 }} error={!!errors.Id_Country}>
            <InputLabel id="label-pais">País</InputLabel>
            <Select
              labelId="label-pais"
              name="Id_Country"
              value={form.Id_Country || ''}
              onChange={handleChange}
              label="País"
              required
            >
              {countries?.map((pais) => (
                <MenuItem key={pais.id_country} value={pais.id_country}>
                  {pais.country_name}
                </MenuItem>
              ))}
            </Select>
            {errors.Id_Country && (
              <FormHelperText>{errors.Id_Country}</FormHelperText>
            )}
          </FormControl>

          <FormControl fullWidth sx={{ mb: 2 }} error={!!errors.Id_Rol}>
            <InputLabel id="rol-label">Rol</InputLabel>
            <Select
              labelId="rol-label"
              name="Id_Rol"
              value={form.Id_Rol || ''}
              onChange={handleChange}
              label="Rol"
              required
            >
              {roles?.map((rol) => {
              const style = roleStyles[rol.id_rol] || { color: 'inherit', fontWeight: 'normal' };
              return (
                <MenuItem key={rol.id_rol} value={rol.id_rol}>
                  <span style={style}>{rol.rol}</span>
                </MenuItem>
              );
            })}
            </Select>
            {errors.Id_Rol && (
              <FormHelperText>{errors.Id_Rol}</FormHelperText>
            )}
          </FormControl>

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
      {message}
    </Alert>
  </Snackbar>

  </Paper>
);
}

export default CrudUsuarioForm;

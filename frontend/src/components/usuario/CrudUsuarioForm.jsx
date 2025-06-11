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
  FormHelperText,
  List,
  ListItem,
  ListItemText,
  Grid
} from '@mui/material';
import PerButton from '../ui/PerButton';
import PerTextField from '../ui/PerTextField';
import { useSnackbar} from '../../hooks/useSnackbar';
import { useForm } from '../../hooks/useForm';
import {
  createUser,
  alterUser,
  deactivateUser,
  getCountries,
  getRoles,
  getAllUsers,
} from '../../services/userService';
import { useTheme } from '@mui/material/styles';
import { Form, useNavigate } from 'react-router-dom';

const initialForm = {
  Id_User: 0,
  id_Country: 0,
  Id_Rol: 0,
  Fullname: '',
  Passcode: '',
  Alias: '',
  Email: '',
  avatar_Url: '',
};

export const CrudUsuarioForm = () => {
  const [action, setAction] = useState('CREATE');
  const { open, message, showSnackbar, closeSnackbar } = useSnackbar();
  const [countries, setCountries] = useState([]);
  const [roles, setRoles] = useState([]);
  const [users, setUsers] = useState([]);
  const theme = useTheme();
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedUser, setSelectedUser] = useState(null);


  useEffect(() => {
    const fetchData = async () => {
      try {
        const paises = await getCountries();
        const roles = await getRoles();
        const usuarios = await getAllUsers();
        setCountries(paises);
        setRoles(roles);
        setUsers(usuarios);
      } catch (error) {
        console.error('Error cargando datos:', error.message);
      }
    };

    fetchData();
  }, []);

  const {
    form,
    errors,
    handleChange,
    validate,
    setForm,
    setErrors,
    resetForm,
  } = useForm(initialForm, action);

  const isCreate = action === 'CREATE';
  const isAlter = action === 'ALTER';
  const isDelete = action === 'DELETE';
  const isView = action === 'VIEW';

const handleActionChange = (event) => {
  const newAction = event.target.value;
  setAction(newAction);
  setForm(initialForm);
  setErrors({});

  if (newAction !== 'VIEW') {
    setSelectedUser(null); // 👈 Limpia el detalle al salir de VIEW
    setSearchTerm('');
  }
};


  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) {
      console.log('Errores de validación:', errors);
      return;
    }

    try {
      if (isCreate) {
        await createUser(
          form.id_Country,
          form.Id_Rol,
          form.Passcode,
          form.Fullname,
          form.Alias,
          form.Email,
          form.avatar_Url
        );
        showSnackbar('Usuario creado exitosamente');
      } else if (isAlter) {
        await alterUser(
          form.Id_User,
          form.id_Country,
          form.id_Rol,
          form.Fullname,
          form.Alias,
          form.Email,
          form.avatar_Url
        );
        showSnackbar('Usuario modificado exitosamente');
      } else if (isDelete) {
        await deactivateUser(form.Id_User);
        showSnackbar('Usuario eliminado exitosamente');
      }

      setTimeout(() => {
        navigate('/menu');
      }, 3000);
    } catch (error) {
      console.error('Error al ejecutar la acción:', error);
    }
  };

  const roleStyles = {
    1: { color: theme.palette.primary.main, fontWeight: 'bold' },
    2: { color: theme.palette.secondary.main, fontWeight: 'bold' },
    3: { color: theme.palette.info.main, fontWeight: 'bold' },
    4: { color: theme.palette.success.main, fontWeight: 'normal' },
  };


if (selectedUser) {
  console.log(selectedUser.avatar_Url)
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
          <MenuItem value="VIEW">Visualizar</MenuItem>
        </Select>
      </FormControl>

      <Box>
        <Grid container spacing={4}>
        {/* Columna izquierda: formulario principal */}
        <Grid item xs={12} md={6}>
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
                const rolName =
                  roles.find((r) => r.id_Rol === user?.id_Rol)?.rol || 'Sin rol';
                return `${user?.fullname} [${rolName}]`;
              }}
            >
              {users.map((user) => {
                const rolName =
                  roles.find((r) => r.id_Rol === user.id_Rol)?.rol || 'Sin rol';
                const style =
                  roleStyles[user.id_Rol] || { color: 'inherit', fontWeight: 'normal' };

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

       {isView && (
  <>
    <PerTextField
      label="Buscar por nombre"
      value={searchTerm}
      onClick={() => {
      handleChange({ target: { name: 'Id_User', value: user.id_user } });
      setSelectedUser(user);
    }}

      onChange={(e) => setSearchTerm(e.target.value)}
      fullWidth
      sx={{ mb: 2 }}
    />

    <Paper variant="outlined" sx={{ maxHeight: 250, overflowY: 'auto', mb: 3 }}>
      <List>
        {users
          .filter((user) =>
            user.fullname.toLowerCase().includes(searchTerm.toLowerCase())
          )
          .map((user) => {
            const rolName = roles.find((r) => r.id_Rol === user.id_Rol)?.rol || 'Sin rol';
            const style = roleStyles[user.id_Rol] || {};
            const isSelected = form.Id_User === user.id_user;
            return (
          <ListItem
            key={user.id_user}
            button
            selected={form.Id_User === user.id_user}
            onClick={() => {
              setSelectedUser(user);
              setForm((prev) => ({ ...prev, Id_User: user.id_user }));
            }}
            sx={{ ...style }}
          >


                <ListItemText
                  primary={`${user.fullname} [${rolName.toUpperCase()}]`}
                  sx={{
                    fontWeight: isSelected ? 'bold' : 'normal',
                    color: isSelected ? 'primary.main' : 'inherit',
                  }}
                />
              </ListItem>
            );
          })}
      </List>
    </Paper>

   


    {errors.Id_User && (
      <FormHelperText error>{errors.Id_User}</FormHelperText>
    )}
  </>
)}

        {(isCreate || isAlter) && (
          <>
            {/* País */}
             <FormControl fullWidth sx={{ mb: 3 }} error={!!errors.id_Country}>
              <InputLabel id="label-pais">País</InputLabel>
              <Select
                labelId="label-pais"
                name="id_Country"
                value={form.id_Country || ''}
                onChange={handleChange}
                label="País"
                required
              >
                {countries.map((pais) => (
                  <MenuItem key={pais.id_Country} value={pais.id_Country}>
                    {pais.country_name}
                  </MenuItem>
                ))}
              </Select>
              {errors.id_Country && (
                <FormHelperText>{errors.id_Country}</FormHelperText>
              )}
            </FormControl> 



            {/* Rol */}
            <FormControl fullWidth sx={{ mb: 2 }} error={!!errors.id_Rol}>
              <InputLabel id="rol-label">Rol</InputLabel>
              <Select
                labelId="rol-label"
                name="id_Rol"
                value={form.id_Rol || ''}
                onChange={handleChange}
                label="Rol"
                required
              >
                {roles.map((rol) => {
                  const style = roleStyles[rol.id_Rol] || {
                    color: 'inherit',
                    fontWeight: 'normal',
                  };
                  return (
               <MenuItem key={rol.id_Rol} value={rol.id_Rol} sx={style}>
                {rol.rol}
              </MenuItem>

                  );
                })}
              </Select>
              {errors.id_Rol && (
                <FormHelperText>{errors.id_Rol}</FormHelperText>
              )}
            </FormControl>

            {/* Campos de texto */}
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
              name="avatar_Url"
              value={form.avatar_Url}
              onChange={handleChange}
              error={!!errors.avatar_Url}
              helperText={errors.avatar_Url}
              required
            />
          </>
        )}

        {(isCreate || isAlter || isDelete) && (
          <PerButton type="submit" color="primary" variant="contained" fullWidth>
            {action === 'CREATE'
              ? 'Crear Usuario'
              : action === 'ALTER'
              ? 'Modificar Usuario'
              : 'Eliminar Usuario'}
          </PerButton>
        )}
      </Box>
      </Grid>

       <Grid item xs={12} md={6}>

         {selectedUser && (
        <Paper
          elevation={2}
          sx={{
            p: 2,
            mt: 2,
            borderLeft: `5px solid ${theme.palette.primary.main}`,
            backgroundColor: '#f9f9f9',
          }}
        >
          <Typography variant="h6" gutterBottom>
            Detalles del Usuario
          </Typography>
          <Typography><strong>Nombre:</strong> {selectedUser.fullname}</Typography>
          <Typography><strong>Alias:</strong> {selectedUser.alias}</Typography>
          <Typography><strong>Email:</strong> {selectedUser.email}</Typography>
          <Typography><strong>Rol:</strong> {
            roles.find((r) => r.id_Rol === selectedUser.id_Rol)?.rol || 'Sin rol'
          }</Typography>
          <Typography><strong>País:</strong> {
            countries.find((c) => c.id_Country === selectedUser.id_Country)?.country_name || 'Desconocido'
          }</Typography>
          {selectedUser.avatar_Url && (
            <Box mt={2}>
              <img
                src={selectedUser.avatar_Url}
                alt="Avatar"
                style={{ maxWidth: '100%', maxHeight: 150 }}
              />
            </Box>
          )}
        </Paper>
      )}
      </Grid>
    </Grid>
    </Box>

      <Snackbar open={open} autoHideDuration={3000} onClose={closeSnackbar}>
        <Alert severity="success" onClose={closeSnackbar} sx={{ width: '100%' }}>
          {message}
        </Alert>
      </Snackbar>
    </Paper>

      
  );
};

export default CrudUsuarioForm;
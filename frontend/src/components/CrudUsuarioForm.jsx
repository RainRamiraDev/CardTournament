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
  FormControl
} from '@mui/material';

import Button from './ui/Button';
import TextField from './ui/TextField';



const CrudUsuarioForm = () => {
  const [action, setAction] = useState('CREATE');
  const [formData, setFormData] = useState({
    Id_User: '',
    Id_Country: '',
    Id_Rol: '',
    Fullname: '',
    Passcode: '',
    Alias: '',
    Email: '',
    Avatar_Url: ''
  });

  const [snackbarOpen, setSnackbarOpen] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleActionChange = (e) => {
    setAction(e.target.value);
    setFormData({
      Id_User: '',
      Id_Country: '',
      Id_Rol: '',
      Fullname: '',
      Passcode: '',
      Alias: '',
      Email: '',
      Avatar_Url: ''
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log('Acción:', action);
    console.log('Datos:', formData);
    setSnackbarOpen(true);
    // Aquí se conectaría a tu API (con fetch o axios)
  };

  const handleCloseSnackbar = () => {
    setSnackbarOpen(false);
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
        {/* ID para ALTER y DELETE */}
        {(isAlter || isDelete) && (
          <TextField
            fullWidth
            label="ID de Usuario"
            name="Id_User"
            value={formData.Id_User}
            onChange={handleChange}
            margin="normal"
            required
          />
        )}

        {/* CREATE / ALTER campos */}
        {(isCreate || isAlter) && (
          <>
            <TextField
              fullWidth
              label="ID País"
              name={isCreate ? 'Id_Country' : 'Id_Country'}
              value={formData.Id_Country}
              onChange={handleChange}
              margin="normal"
              required={isCreate}
              disabled={isDelete}
            />
            <TextField
              fullWidth
              label="ID Rol"
              name={isCreate ? 'Id_Rol' : 'Id_Rol'}
              value={formData.Id_Rol}
              onChange={handleChange}
              margin="normal"
              required={isCreate}
              disabled={isDelete}
            />
            <TextField
              fullWidth
              label="Nombre completo"
              name="Fullname"
              value={formData.Fullname}
              onChange={handleChange}
              margin="normal"
              required={isCreate}
              disabled={isDelete}
            />
            {isCreate && (
              <TextField
                fullWidth
                label="Contraseña"
                name="Passcode"
                type="password"
                value={formData.Passcode}
                onChange={handleChange}
                margin="normal"
                required
              />
            )}
            <TextField
              fullWidth
              label="Alias"
              name="Alias"
              value={formData.Alias}
              onChange={handleChange}
              margin="normal"
              required={isCreate}
              disabled={isDelete}
            />
            <TextField
              fullWidth
              label="Email"
              name="Email"
              value={formData.Email}
              onChange={handleChange}
              margin="normal"
              type="email"
              required={isCreate}
              disabled={isDelete}
            />
            <TextField
              fullWidth
              label="Avatar URL"
              name="Avatar_Url"
              value={formData.Avatar_Url}
              onChange={handleChange}
              margin="normal"
              required={isCreate}
              disabled={isDelete}
            />
          </>
        )}

        <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
          {action === 'CREATE' ? 'Crear' : action === 'ALTER' ? 'Modificar' : 'Eliminar'}
        </Button>
      </Box>

      <Snackbar
        open={snackbarOpen}
        autoHideDuration={3000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert severity="success" onClose={handleCloseSnackbar}>
          ¡Acción {action} ejecutada exitosamente!
        </Alert>
      </Snackbar>
    </Paper>
  );
};

export default CrudUsuarioForm;

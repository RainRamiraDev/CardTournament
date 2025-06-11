import React, { useEffect, useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  List,
  ListItemText,
  ListItemIcon,
  ListItemButton,
  Checkbox,
  Button,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Snackbar,
  Alert,
  FormHelperText,
  Grid,
  Divider
} from '@mui/material';
import { Drawer, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

import { getAllUsers } from '../../services/userService';
import { getCardsByUserId } from '../../services/cardService';
import useDrawer from '../../hooks/useDrawer';

export default function PlayersCards() {
  const { open, openDrawer, closeDrawer } = useDrawer();
  const [users, setUsers] = useState([]);
  const [selectedUser, setSelectedUser] = useState('');
  const [userCards, setUserCards] = useState([]);
  const [loading, setLoading] = useState(true);
  const [loadingCards, setLoadingCards] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [errors, setErrors] = useState({});

  const fetchData = async () => {
    setLoading(true);
    try {
      const usersData = await getAllUsers();
      setUsers(usersData);
    } catch (error) {
      setSnackbar({ open: true, message: 'Error cargando datos', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const fetchUserCards = async (id_user) => {
    setLoadingCards(true);
    try {
      const response = await getCardsByUserId(id_user);
      setUserCards(response?.data || []);

    } catch (error) {
      setSnackbar({ open: true, message: 'Error cargando cartas del usuario', severity: 'error' });
    } finally {
      setLoadingCards(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleUserChange = async (e) => {
    const userId = Number(e.target.value);
    setSelectedUser(userId);
    setErrors((prev) => ({ ...prev, user: undefined }));
    await fetchUserCards(userId);
  };

  return (
    <>
      <Typography variant="h5" gutterBottom align="center" sx={{ mt: 4 }}>
        Seleccionar Jugador
      </Typography>

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <Grid container spacing={4} justifyContent="center" sx={{ mt: 4 }}>
          <Grid item xs={12} sm={6} md={4}>
            <Paper sx={{ p: 4, height: 500, overflowY: 'auto' }}>
              <Typography variant="h6" gutterBottom>
                Jugadores
              </Typography>
              <FormControl fullWidth sx={{ mb: 3 }} error={!!errors.user}>
                <InputLabel id="label-usuario">Usuario</InputLabel>
                <Select
                  labelId="label-usuario"
                  value={selectedUser}
                  onChange={handleUserChange}
                  label="Usuario"
                >
                  {users.map((user) => (
                    <MenuItem key={user.id_user} value={user.id_user}>
                      {user.fullname}
                    </MenuItem>
                  ))}
                </Select>
                {errors.user && <FormHelperText>{errors.user}</FormHelperText>}
              </FormControl>
            </Paper>
          </Grid>

          {selectedUser && (
            <Grid item xs={12} sm={6} md={6}>
              <Paper sx={{ p: 4, height: 500, overflowY: 'auto' }}>
                <Typography variant="h6" gutterBottom>
                  Cartas del Jugador
                </Typography>
                {loadingCards ? (
                  <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
                    <CircularProgress />
                  </Box>
                ) : userCards.length > 0 ? (
                  userCards.map((card, index) => (
                    <Box key={index} sx={{ mb: 2, p: 2, border: '1px solid #ccc', borderRadius: 2, background: '#f9f9f9' }}>
                      <Typography variant="subtitle1"><strong>{card.illustration}</strong></Typography>
                      <Typography variant="body2">Ataque: {card.attack}</Typography>
                      <Typography variant="body2">Defensa: {card.defense}</Typography>
                      <Typography variant="body2">Serie: {card.series_Name}</Typography>
                      <Typography variant="body2">Lanzamiento: {card.release_Date}</Typography>
                    </Box>
                  ))
                ) : (
                  <Typography variant="body2">Este jugador no tiene cartas asignadas.</Typography>
                )}
              </Paper>
            </Grid>
          )}
        </Grid>
      )}

      <Snackbar
        open={snackbar.open}
        autoHideDuration={3000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert
          severity={snackbar.severity}
          onClose={() => setSnackbar({ ...snackbar, open: false })}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>

      <Drawer anchor="right" open={open} onClose={closeDrawer}>
        <Box sx={{ width: 300, p: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
            <IconButton onClick={closeDrawer}>
              <CloseIcon />
            </IconButton>
          </Box>
          <Typography variant="h6" gutterBottom>
            Asignación exitosa
          </Typography>
          <Typography>
            Cartas mostradas exitosamente.
          </Typography>
        </Box>
      </Drawer>
    </>
  );
}

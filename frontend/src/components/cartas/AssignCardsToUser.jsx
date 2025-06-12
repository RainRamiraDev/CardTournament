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
  Grid
} from '@mui/material';

import { getAllCards, assignCardToPlayer } from '../../services/cardService';
import { getAllUsers } from '../../services/userService';
import { handleAxiosError } from '../../utils/handleAxiosError';
import useDrawer from '../../hooks/useDrawer';
import { Drawer, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

// Usa el hook de snackbar global
import { useSnackbar } from '../../hooks/useSnackbar';

export default function AssignCardsToUser() {
  const { open, openDrawer, closeDrawer } = useDrawer();
  const { message, showSnackbar, closeSnackbar, severity, setSeverity, open: snackbarOpen, setOpen: setSnackbarOpen } = useSnackbar();

  const [cards, setCards] = useState([]);
  const [users, setUsers] = useState([]);
  const [selectedCards, setSelectedCards] = useState([]);
  const [selectedUser, setSelectedUser] = useState('');
  const [loading, setLoading] = useState(true);
  const [assigning, setAssigning] = useState(false);
  const [errors, setErrors] = useState({});

  const fetchData = async () => {
    setLoading(true);
    try {
      const cardsData = await getAllCards();
      const usersData = await getAllUsers();
      setCards(cardsData);
      setUsers(usersData);
    } catch (error) {
      showSnackbar('Error cargando datos', 'error');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const validate = () => {
    const errors = {};
    if (!selectedUser) errors.user = 'Selecciona un jugador';
    if (selectedCards.length === 0) errors.cards = 'Selecciona al menos una carta';
    setErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleToggleCard = (cardId) => {
    setSelectedCards((prev) =>
      prev.includes(cardId) ? prev.filter((id) => id !== cardId) : [...prev, cardId]
    );
  };

  const handleUserChange = (e) => {
    setSelectedUser(Number(e.target.value)); // Forzar a número
    setErrors((prev) => ({ ...prev, user: undefined }));
  };

  // --- HANDLE SUBMIT usando el snackbar global ---
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    setAssigning(true);

    try {
      const response = await assignCardToPlayer(selectedUser, selectedCards);

      if (response && response.success) {
        showSnackbar(response.message || 'Cartas asignadas correctamente.', 'success');
        openDrawer();
        setSelectedCards([]);
        setSelectedUser('');
        setErrors({});
      } else {
        showSnackbar(response?.message || 'Error al asignar cartas', 'error');
      }
    } catch (error) {
      showSnackbar(error.message || 'Ocurrió un error al asignar las cartas.', 'error');
    } finally {
      setAssigning(false);
    }
  };

  return (
    <>
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <form onSubmit={handleSubmit}>
          <Grid container spacing={4} justifyContent="center" sx={{ mt: 4 }}>
         
            {/* Cartas */}
            <Grid item xs={12} sm={6} md={4}>
              <Paper
                sx={{
                  p: 4,
                  width: { xs: '100%', sm: 400 },
                  height: 500,
                  overflowY: 'auto',
                  display: 'flex',
                  flexDirection: 'column',
                  mb: { xs: 2, sm: 0 },
                }}
              >
                <Typography variant="h6" gutterBottom>
                  Cartas
                </Typography>
                <List>
                  {cards.map((card) => (
                    <ListItemButton
                      key={card.id_Card}
                      onClick={() => handleToggleCard(card.id_Card)}
                      selected={selectedCards.includes(card.id_Card)}
                      sx={{ borderRadius: 1 }}
                    >
                      <ListItemIcon>
                        <Checkbox
                          edge="start"
                          checked={selectedCards.includes(card.id_Card)}
                          tabIndex={-1}
                          disableRipple
                          onChange={() => handleToggleCard(card.id_Card)}
                        />
                      </ListItemIcon>
                      <ListItemText
                        primary={`#${card.id_Card} - ${card.illustration}`}
                        secondary={`Ataque: ${card.attack} / Defensa: ${card.defense}`}
                      />
                    </ListItemButton>
                  ))}
                </List>
                {errors.cards && (
                  <Typography color="error" variant="caption">
                    {errors.cards}
                  </Typography>
                )}
              </Paper>
            </Grid>

            {/* Jugadores */}
            <Grid item xs={12} sm={6} md={4}>
              <Paper
                sx={{
                  p: 4,
                  width: { xs: '100%', sm: 400 },
                  height: 500,
                  overflowY: 'auto',
                  display: 'flex',
                  flexDirection: 'column',
                  mb: { xs: 2, sm: 0 },
                }}
              >
                <Box>
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
                      renderValue={(selectedId) => {
                        const user = users.find((u) => u.id_user === selectedId);
                        return user ? user.fullname : 'Selecciona un usuario';
                      }}
                    >
                      {users.map((user) => (
                        <MenuItem key={user.id_user} value={user.id_user}>
                          {user.fullname}
                        </MenuItem>
                      ))}
                    </Select>
                    {errors.user && <FormHelperText>{errors.user}</FormHelperText>}
                  </FormControl>
                </Box>
                <Button
                  type="submit"
                  variant="contained"
                  color="primary"
                  fullWidth
                  disabled={assigning || !selectedUser || selectedCards.length === 0}
                >
                  {assigning ? 'Asignando...' : 'Asignar Cartas'}
                </Button>
              </Paper>
            </Grid>
          </Grid>
        </form>
      )}

      {/* Snackbar centralizado y usando el mensaje del backend */}
      <Snackbar
        open={snackbarOpen}
        autoHideDuration={3000}
        onClose={closeSnackbar}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert
          severity={severity || 'success'}
          onClose={closeSnackbar}
          sx={{ width: '100%' }}
        >
          {message}
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
            Las cartas fueron asignadas correctamente al jugador seleccionado.
          </Typography>
        </Box>
      </Drawer>
    </>
  );
}

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
import { useNavigate } from 'react-router-dom';

import { getAllCards, assignCardToPlayer } from '../../services/cardService';
import { getAllUsers } from '../../services/userService';
import { useSnackbar } from '../../hooks/useSnackbar';

export default function AssignCardsToUser() {
  const { message, showSnackbar, closeSnackbar, severity, open: snackbarOpen } = useSnackbar();
  const navigate = useNavigate();

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
    if (selectedCards.length < 8) errors.cards = 'Debes seleccionar al menos 8 cartas';
    setErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleToggleCard = (cardId) => {
    if (selectedCards.includes(cardId)) {
      setSelectedCards((prev) => prev.filter((id) => id !== cardId));
    } else {
      if (selectedCards.length >= 15) {
        showSnackbar('No puedes asignar más de 15 cartas a un jugador.', 'warning');
        return;
      }
      setSelectedCards((prev) => [...prev, cardId]);
    }
  };

  const handleUserChange = (e) => {
    setSelectedUser(Number(e.target.value));
    setErrors((prev) => ({ ...prev, user: undefined }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) {
      if (selectedCards.length < 8) {
        showSnackbar('Debes seleccionar al menos 8 cartas para asignar.', 'warning');
      }
      return;
    }

    setAssigning(true);

    try {
      const response = await assignCardToPlayer(selectedUser, selectedCards);

      if (response && response.success) {
        showSnackbar(response.message || 'Cartas asignadas correctamente.', 'success');
        setSelectedCards([]);
        setSelectedUser('');
        setErrors({});
        setTimeout(() => {
          navigate('/menu');
        }, 1200); // Espera breve para mostrar el snackbar antes de redirigir
      } else {
        showSnackbar(response?.message || 'Error al asignar cartas', 'error');
      }
    } catch (error) {
      showSnackbar(error.message || 'Ocurrió un error al asignar las cartas.', 'error');
    } finally {
      setAssigning(false);
    }
  };

  const handleClearSelection = () => {
    setSelectedCards([]);
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
                  p: { xs: 2, sm: 3, md: 4 },
                  width: { xs: '100%', sm: 350, md: 400 },
                  height: { xs: 350, sm: 420, md: 500 },
                  mb: { xs: 2, sm: 0 },
                  overflowY: 'auto',
                  display: 'flex',
                  flexDirection: 'column',
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
                  p: { xs: 2, sm: 3, md: 4 },
                  width: { xs: '100%', sm: 350, md: 400 },
                  height: { xs: 350, sm: 420, md: 500 },
                  mb: { xs: 2, sm: 0 },
                  overflowY: 'auto',
                  display: 'flex',
                  flexDirection: 'column',
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
                <Button
                  variant="outlined"
                  color="secondary"
                  fullWidth
                  sx={{ mt: 2 }}
                  onClick={handleClearSelection}
                  disabled={selectedCards.length === 0}
                >
                  Limpiar selección
                </Button>
              </Paper>
            </Grid>
          </Grid>
        </form>
      )}

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
    </>
  );
}

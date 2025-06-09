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
} from '@mui/material';

import { getAllCards, assignCardToPlayer } from '../../services/cardService';
import { getAllUsers } from '../../services/userService';
import { handleAxiosError } from '../../utils/handleAxiosError';

export default function AssignCardsToUser() {
  const [cards, setCards] = useState([]);
  const [users, setUsers] = useState([]);
  const [selectedCards, setSelectedCards] = useState([]);
  const [selectedUser, setSelectedUser] = useState('');
  const [loading, setLoading] = useState(true);
  const [assigning, setAssigning] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [errors, setErrors] = useState({});

  const fetchData = async () => {
    setLoading(true);
    try {
      const cardsData = await getAllCards();
      const usersData = await getAllUsers();
      setCards(cardsData);
      setUsers(usersData);
    } catch (error) {
      setSnackbar({ open: true, message: 'Error cargando datos', severity: 'error' });
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

const handleSubmit = async (e) => {
  e.preventDefault();
  if (!validate()) return;

  console.log('Tipo de selectedUser:', typeof selectedUser); // Para verificar que es number

  setAssigning(true);

  try {

    console.log('selectedUser:', selectedUser, typeof selectedUser);
    console.log('selectedCards:', selectedCards, selectedCards.map(id => typeof id));


    const response = await assignCardToPlayer(selectedUser, selectedCards);
    if (response.status === 200) {
      console.log('Cartas asignadas correctamente:', response.data);

      setSnackbar({
        open: true,
        severity: 'success',
        message: 'Cartas asignadas correctamente.',
      });

      setSelectedCards([]);
      setSelectedUser('');
      setErrors({});
    } else {
      throw new Error('Error al asignar cartas');
    }
  } catch (error) {
    handleAxiosError(error);
    setSnackbar({
      open: true,
      severity: 'error',
      message: 'Ocurrió un error al asignar las cartas.',
    });
  } finally {
    setAssigning(false);
  }
};


  return (
    <>
      <Typography variant="h5" gutterBottom align="center" sx={{ mt: 4 }}>
        Asignar Cartas a Jugador
      </Typography>

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <form onSubmit={handleSubmit}>
          <Box
            sx={{
              display: 'flex',
              gap: 4,
              justifyContent: 'center',
              mt: 4,
              flexWrap: 'wrap',
            }}
          >
            {/* Cartas */}
            <Paper
              sx={{
                p: 4,
                width: 400,
                height: 500,
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

            {/* Jugadores */}
            <Paper
              sx={{
                p: 4,
                width: 400,
                height: 500,
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'space-between',
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
          </Box>
        </form>
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
    </>
  );
}

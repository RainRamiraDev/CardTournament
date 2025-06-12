import React, { useEffect, useState } from 'react';
import {
  Box,
  Paper,
  Typography,
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
import { Drawer, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

import { getAllUsers } from '../../services/userService';
import { getCardsByUserId } from '../../services/cardService';
import useDrawer from '../../hooks/useDrawer';
import cardImage from '../../assets/card.jpg'; // Asegúrate de que la ruta sea correcta

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
      setUserCards(response.data);
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
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <Box sx={{ maxWidth: { xs: '100%', md: '1600px' }, mx: 'auto', px: { xs: 1, sm: 2 } }}>
          <Grid container spacing={4} sx={{ mt: 4 }} justifyContent="center">
            {/* Selector de usuario */}
            <Grid item xs={12} sm={6} md={4} lg={2}>
              <Paper
                sx={{
                  p: { xs: 2, sm: 3, md: 4 },
                  width: { xs: '100%', sm: 320, md: 350 },
                  minHeight: { xs: 220, sm: 260 },
                  m: { xs: 1, md: 2 },
                  borderRadius: 3,
                  backgroundColor: '#121212',
                  border: '1.5px solid cyan',
                  color: 'cyan',
                  fontFamily: "'Cinzel', serif",
                  boxShadow: '0 0 10px cyan',
                  transition: '0.3s',
                  '&:hover': {
                    boxShadow: '0 0 20px #00ffff',
                    backgroundColor: '#1b1b1b',
                  },
                }}
              >
                <Typography variant="h6" gutterBottom>
                  Jugadores
                </Typography>
                <Box sx={{ display: 'flex', justifyContent: 'center', width: '100%', mt: 5 }}>
                  <FormControl fullWidth error={!!errors.user} sx={{ maxWidth: 300 }}>
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
                </Box>
              </Paper>
            </Grid>

            {/* Cartas del jugador */}
            {selectedUser && (
              <Grid item xs={12}>
                <Paper sx={{ p: { xs: 2, sm: 3, md: 4 } }}>
                  <Typography variant="h6" gutterBottom>
                    Cartas del Jugador
                  </Typography>

                  {loadingCards ? (
                    <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
                      <CircularProgress />
                    </Box>
                  ) : userCards.length > 0 ? (
<Grid container spacing={4} sx={{ mt: 7 }} justifyContent="center">
                        {userCards.map((card) => (
                        <Grid item xs={12} sm={6} md={4} lg={2} xl={2} key={card.id_Card}>
                          <Paper
                            elevation={3}
                            sx={{
                              p: 2,
                              maxWidth: 180,
                              width: '100%',
                              height: '100%',
                              display: 'flex',
                              flexDirection: 'column',
                              justifyContent: 'space-between',
                              borderRadius: 3,
                              backgroundColor: '#121212',
                              border: '1.5px solid cyan',
                              color: 'cyan',
                              fontFamily: "'Cinzel', serif",
                              boxShadow: '0 0 10px cyan',
                              transition: '0.3s',
                              '&:hover': {
                                boxShadow: '0 0 20px #00ffff',
                                backgroundColor: '#1b1b1b',
                              },
                            }}
                          >
                            {/* Imagen arriba */}
                            <Box
                              component="img"
                              src={cardImage}
                              alt={`Ilustración de ${card.illustration}`}
                              sx={{
                                width: '100%',
                                mt: 2,
                                height: 100,
                                objectFit: 'contain',
                                mb: 2,
                                borderRadius: 2,
                                transform: 'scale(1.1)',
                                transformOrigin: 'center',
                              }}
                            />

                            <Typography
                              variant="subtitle1"
                              fontWeight="900"
                              gutterBottom
                              sx={{
                                letterSpacing: 2,
                                fontSize: { xs: '1rem', sm: '1.1rem', md: '1.2rem' },
                                textShadow: '0 0 5px cyan',
                                color: '#00ffff',
                                textTransform: 'uppercase',
                                mb: 2,
                              }}
                            >
                              {card.illustration}
                            </Typography>

                            <Box sx={{ flexGrow: 1, mb: 2 }}>
                              <Typography variant="body2" sx={{ mb: 0.7, fontWeight: '600', color: '#88ffff' }}>
                                ATAQUE: <span style={{ fontWeight: 'bold', color: 'white' }}>{card.attack}</span>
                              </Typography>
                              <Typography variant="body2" sx={{ mb: 0.7, fontWeight: '600', color: '#88ffff' }}>
                                DEFENSA: <span style={{ fontWeight: 'bold', color: 'white' }}>{card.defense}</span>
                              </Typography>
                              <Typography variant="body2" sx={{ mb: 0.7, fontWeight: '600', color: '#88ffff' }}>
                                SERIE: <span style={{ fontWeight: 'bold', color: 'white' }}>{card.series_Name}</span>
                              </Typography>
                              <Typography variant="body2" sx={{ fontWeight: '600', color: '#88ffff' }}>
                                LANZAMIENTO: <span style={{ fontWeight: 'bold', color: 'white' }}>{card.release_Date}</span>
                              </Typography>
                            </Box>
                          </Paper>
                        </Grid>
                      ))}
                    </Grid>
                  ) : (
                    <Typography variant="body2" sx={{ mt: 2 }}>
                      Este jugador no tiene cartas asignadas.
                    </Typography>
                  )}
                </Paper>
              </Grid>
            )}
          </Grid>
        </Box>
      )}

      {/* Snackbar de feedback */}
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

      {/* Drawer lateral */}
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
          <Typography>Cartas mostradas exitosamente.</Typography>
        </Box>
      </Drawer>
    </>
  );
}

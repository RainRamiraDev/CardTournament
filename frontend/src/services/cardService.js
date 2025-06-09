import axios from 'axios';
import { handleAxiosError } from '../utils/handleAxiosError';
import api from './Api';


// Obtener cartas
export async function getAllCards() {
  try {
    const response = await api.get('/Organizer/GetAllCards');
    return response.data.data; // 👈 devolvés solo el array
  } catch (error) {
    handleAxiosError(error, 'Error al obtener cartas');
  }
}

// Ejemplo de uso
export async function assignCardToPlayer( id_user, id_card) {
  try {
    const response = await api.post('/Organizer/AssignCardToPlayer', {
      id_user,
      id_card,
    });
    return response.data;
  } catch (error) {
    handleAxiosError(error, 'Error al asignar carta al jugador');
  }
}
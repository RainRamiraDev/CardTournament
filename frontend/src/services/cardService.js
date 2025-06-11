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
export async function assignCardToPlayer(id_user, id_card_array) {
  try {

        console.log('Payload:', { id_user, id_card: id_card_array }); // 👈 agregar esta línea
    const response = await api.post('/Organizer/AssignCardToPlayer', {
      id_user,
      id_card: id_card_array, // ⚠️ debe ser "id_card", no "id_cards"
    });
    return response.data;
  } catch (error) {
    handleAxiosError(error, 'Error al asignar carta al jugador');
  }
}


export async function getCardsByUserId(id_User) {
  try {
    const response = await api.get(`/Organizer/GetCardsByUser?id_user=${id_User}`);
    return response.data;
  } catch (error) {
    handleAxiosError(error, 'Error al obtener cartas del jugador');
  }
}








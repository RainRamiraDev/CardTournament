import axios from 'axios';
import { handleAxiosError } from '../utils/handleAxiosError';
import api from './Api';




// Ejemplo de uso
export async function logInUser(Fullname, Passcode) {
  try {
    const response = await api.post('/LogIn/LogIn', {
      Fullname,
      Passcode,
    });
    return response.data;
  } catch (error) {
    handleAxiosError(error, 'Error al iniciar sesión');
  }
}




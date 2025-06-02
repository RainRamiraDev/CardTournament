import axios from 'axios';
import { handleAxiosError } from '../utils/handleAxiosError';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;




// Ejemplo de uso
export async function logInUser(Fullname, Passcode) {
  try {
    const response = await axios.post(`${API_BASE_URL}/LogIn/LogIn`, {
      Fullname,
      Passcode,
    });
    return response.data;
  } catch (error) {
    handleAxiosError()(error, 'Error al iniciar sesión');
  }
}




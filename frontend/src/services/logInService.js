// src/services/LoginService.js
import axios from 'axios';

export async function logInService(Fullname, Passcode) {
  try {
    const response = await axios.post('http://localhost:7276/Api/LogIn/Login', {
      Fullname,
      Passcode,
    });
    return response.data; // datos devueltos por el backend
  } catch (error) {
    if (error.response) {
      throw new Error(error.response.data.message || 'Error en el login');
    } else {
      throw new Error('No se pudo conectar con el servidor');
    }
  }
}

export default logInService;
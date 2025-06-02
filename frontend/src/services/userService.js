import axios from 'axios';
import { handleAxiosError } from '../utils/handleAxiosError';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;




// Registro
export async function createUser(Id_Country, Id_Rol, Passcode, Fullname, Alias, Email, Avatar_Url) {
  try {
    const response = await axios.post(`${API_BASE_URL}/Admin/CreateUser`,{
        Id_Country,
        Id_Rol,
        Passcode, 
        Fullname, 
        Alias, 
        Email, 
        Avatar_Url
    } );
    return response.data;
  } catch (error) {
       handleAxiosError(error, 'registrar al usuario');
  }
}

// Modificacion
export async function alterUser(Id_User, New_IdCountry, New_Id_Rol, New_Fullname, New_Alias, New_Email, New_Avatar_Url) {
  try {
    const response = await axios.put(`${API_BASE_URL}/Admin/AlterUser`,{
      Id_User, 
      New_IdCountry, 
      New_Id_Rol, 
      New_Fullname, 
      New_Alias, 
      New_Email, 
      New_Avatar_Url
    } );
    return response.data;
  } catch (error) {
     handleAxiosError(error, 'modificar al usuario');
  }
}

// Eliminacion
// Eliminacion
export async function deactivateUser(Id_User) {
  try {
    const response = await axios.delete(
      `${API_BASE_URL}/Admin/DeactivateUser`,
      { data: { Id_User } } // <-- así se envía el body en DELETE
    );
    return response.data;
  } catch (error) {
     handleAxiosError(error, 'desactivar al usuario');
  }
}

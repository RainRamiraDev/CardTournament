import axios from 'axios';
import { handleAxiosError } from '../utils/handleAxiosError';

import api from './Api';


// Registro
export async function createUser(Id_Country, Id_Rol, Passcode, Fullname, Alias, Email, Avatar_Url) {
  try {
    const response = await api.post('/Admin/CreateUser',{
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
    const response = await api.put('/Admin/AlterUser',{
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
export async function deactivateUser(Id_User) {
  try {
    const response = await api.delete(
      '/Admin/DeactivateUser',
      { data: { Id_User } } // <-- así se envía el body en DELETE
    );
    return response.data;
  } catch (error) {
     handleAxiosError(error, 'desactivar al usuario');
  }
}

// Obtener países
export async function getCountries() {
  try {
    const response = await api.get('/Organizer/GetCountries');
    return response.data.data; // 👈 devolvés solo el array
  } catch (error) {
    handleAxiosError(error, 'obtener los países');
  }
}

// Obtener roles
export async function getRoles() {
  try {
    const response = await api.get('/Organizer/GetRoles');
    return response.data.data; // 👈 devolvés solo el array
  } catch (error) {
    handleAxiosError(error, 'Error al obtener roles');
  }
}



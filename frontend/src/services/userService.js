import axios from 'axios';
import { handleAxiosError } from '../utils/handleAxiosError';

import api from './Api';


// Registro
export async function createUser(Id_Country, Id_Rol, Passcode, Fullname, Alias, Email, Avatar_Url) {
  try {
    const response = await api.post('/Admin/CreateUser', {
      Id_Country,
      Id_Rol,
      Passcode,
      Fullname,
      Alias,
      Email,
      Avatar_Url
    });
    if (response.data && response.data.success) {
      return response.data;
    } else {
      throw new Error(response.data?.message || 'Error al registrar usuario');
    }
  } catch (error) {
    handleAxiosError(error, 'registrar al usuario');
    throw error;
  }
}

// Haz lo mismo para alterUser y deactivateUser

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
     if (response.data && response.data.success) {
      return response.data;
    } else {
      throw new Error(response.data?.message || 'Error al modificar usuario');
    }
  } catch (error) {
    handleAxiosError(error, 'modificar al usuario');
    throw error;
  }
}

// Eliminacion
export async function deactivateUser(Id_User) {
  try {
    const response = await api.delete(
      '/Admin/DeactivateUser',
      { data: { Id_User } } // <-- así se envía el body en DELETE
    );
     if (response.data && response.data.success) {
      return response.data;
    } else {
      throw new Error(response.data?.message || 'Error al ELIMINAR usuario');
    }
  } catch (error) {
    handleAxiosError(error, 'eliminar al usuario');
    throw error;
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

// Obtener usuarios
export async function getAllUsers() {
  try {
    const response = await api.get('/Admin/GetAllUsers');
    return response.data.data; // 👈 devolvés solo el array
  } catch (error) {
    handleAxiosError(error, 'Error al obtener usuarios');
  }
}



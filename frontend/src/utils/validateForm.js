const fullnameRegex = /^[A-Za-zÁÉÍÓÚÑáéíóúñ\s]+$/;
const passcodeRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{3,}$/;
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const urlRegex = /^(https?:\/\/)[^\s/$.?#].[^\s]*$/i;


// Validación para login
export function validateLoginForm(form) {
  const errors = {};
  if (!form.Fullname?.trim()) {
    errors.Fullname = 'El nombre de usuario es requerido';
  }
  if (!form.Passcode?.trim()) {
    errors.Passcode = 'La contraseña es requerida';
  } else if (!passcodeRegex.test(form.Passcode)) {
    errors.Passcode = 'Debe tener al menos 3 caracteres, incluyendo letras y números';
  }
  return errors;
}

// Validación para acciones de usuario (crear, modificar, eliminar)
function validateUserId(form, action) {
  const errors = {};
  const idUserStr = form.Id_User !== undefined && form.Id_User !== null ? String(form.Id_User).trim() : '';

  if ((action === 'ALTER' || action === 'DELETE') && idUserStr === '') {
    errors.Id_User = 'El ID de usuario es requerido';
  }

  return errors;
}


function validateUserFields(form, action) {
  const errors = {};
 

    if (!Number.isInteger(Number(form.id_Country)) || Number(form.id_Country) <= 0) {
    errors.id_Country = 'El país es requerido';
  }

  if (!Number.isInteger(Number(form.id_Rol)) || Number(form.id_Rol) <= 0) {
    errors.id_Rol = 'El rol es requerido';
  }

  if (!form.Fullname?.trim()) {
    errors.Fullname = 'El nombre completo es requerido';
  } else if (!fullnameRegex.test(form.Fullname)) {
    errors.Fullname = 'El nombre contiene caracteres inválidos';
  }

 if (action === 'CREATE') {
  const passcode = form.Passcode ?? ''; // si es null o undefined, lo convierte en string vacío
  if (!passcode.trim()) {
    errors.Passcode = 'La contraseña es requerida';
  } else if (!passcodeRegex.test(passcode)) {
    errors.Passcode = 'Debe tener al menos 3 caracteres, incluyendo letras y números';
  }
}


  if (!form.Alias?.trim()) {
    errors.Alias = 'El alias es requerido';
  }

  if (!form.Email?.trim()) {
    errors.Email = 'El email es requerido';
  } else if (!emailRegex.test(form.Email)) {
    errors.Email = 'El email no es válido';
  }

  if (!form.avatar_Url?.trim()) {
    errors.avatar_Url = 'La URL del avatar es requerida';
  } else if (!urlRegex.test(form.avatar_Url)) {
    errors.avatar_Url = 'La URL no es válida';
  }

  return errors;
}

// Función principal que decide qué validación usar
export function validateForm(form, action) {
  let errors = {};

  if (action === 'LOGIN') {
    errors = { ...validateLoginForm(form) };
  } else {
    errors = { 
      ...validateUserId(form, action), 
      ...(action === 'CREATE' || action === 'ALTER' ? validateUserFields(form, action) : {}) 
    };
  }

  return errors;
}
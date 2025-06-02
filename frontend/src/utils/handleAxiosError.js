export function handleAxiosError(error, action = 'la operación') {
  if (error.response) {
    throw new Error(error.response.data.message || `Error al intentar ${action}`);
  } else {
    throw new Error('No se pudo conectar con el servidor');
  }
}
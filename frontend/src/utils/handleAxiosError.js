export function handleAxiosError(error, action = 'la operación') {
  if (error.response) {
    const data = error.response.data;
    const backendMessage = data.message || '';
    const backendErrors = Array.isArray(data.errors) && data.errors.length
      ? data.errors.join(', ')
      : '';
    const message =
      backendMessage && backendErrors
        ? `${backendMessage}: ${backendErrors}`
        : backendMessage || backendErrors || `Error al intentar ${action}`;
    throw new Error(message);
  } else {
    throw new Error('No se pudo conectar con el servidor');
  }
}
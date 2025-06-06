import { useState } from 'react';

export const useSnackbar = () => {
  const [open, setOpen] = useState(false);
  const [message, setMessage] = useState('');  // Estado para el mensaje personalizado

  const showSnackbar = (msg) => {
    setMessage(msg);
    setOpen(true);
  };

  const closeSnackbar = () => setOpen(false);

  return { open, message, showSnackbar, closeSnackbar };
};

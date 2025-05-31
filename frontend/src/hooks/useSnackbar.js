import { useState } from 'react';

export const useSnackbar = () => {
  const [open, setOpen] = useState(false);
  const showSnackbar = () => setOpen(true);
  const closeSnackbar = () => setOpen(false);

  return { open, showSnackbar, closeSnackbar };
};

import { useState } from 'react';

export const useDrawer = (initial = false) => {
  const [open, setOpen] = useState(initial);
  const openDrawer = () => setOpen(true);
  const closeDrawer = () => setOpen(false);
  const toggleDrawer = () => setOpen(prev => !prev);

  return { open, openDrawer, closeDrawer, toggleDrawer };
};

export default useDrawer;
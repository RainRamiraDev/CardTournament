import React from 'react';
import { Button as MuiButton } from '@mui/material';

const PerButton = ({ children, ...props }) => {
  return (
    <MuiButton
      variant="contained"
      color="primary"
      fullWidth
      sx={{ mt: 2 }}
      {...props}
    >
      {children}
    </MuiButton>
  );
};

export default PerButton;
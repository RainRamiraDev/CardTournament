import React from 'react';
import { TextField as MuiTextField } from '@mui/material';

const PerTextField = ({ label, name, type = 'text', value, onChange, error, helperText }) => {
  return (
    <MuiTextField
      label={label}
      name={name}
      type={type}
      fullWidth
      margin="normal"
      value={value}
      onChange={onChange}
      error={error}
      helperText={helperText}
    />
  );
};

export default PerTextField;

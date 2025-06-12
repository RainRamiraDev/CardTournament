import { useState } from 'react';
import { validateForm } from '../utils/validateForm';

export const useForm = (initialForm, action) => {
  const [form, setForm] = useState(initialForm);
  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prevForm) => ({
      ...prevForm,
      [name]: value,
    }));
  };

  const validate = () => {
    const validationErrors = validateForm(form, action);
    setErrors(validationErrors);
    return Object.keys(validationErrors).length === 0;
  };

  const resetForm = () => {
    setForm(initialForm);
    setErrors({});
  };

  return {
    form,
    errors,
    setForm,
    setErrors,
    handleChange,
    validate,
    resetForm,
  };
};

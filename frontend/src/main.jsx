import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { ThemeProvider } from '@mui/material/styles';
import { BrowserRouter } from 'react-router-dom';
import { store } from './store/store.js';
import { Provider } from 'react-redux';

import './index.css'
import App from './App.jsx'
import theme from './themes/ThemeProvider.js';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <Provider store={store}>
    <ThemeProvider theme={theme}>
        <BrowserRouter>
             <App />
        </BrowserRouter>
    </ThemeProvider>
  </Provider>
  </StrictMode>,
)

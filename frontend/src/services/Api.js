import axios from 'axios';
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;


const api = axios.create({
  baseURL: API_BASE_URL, // Replace with your API base URL
    timeout: 10000, // Set a timeout for requests
    headers: {
        'Content-Type': 'application/json',
    },
});

export default api;
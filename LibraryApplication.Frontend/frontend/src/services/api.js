import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7282/',
});

export default api;
import axios from 'axios';
import { LoginRequest, AuthResponse } from '../types/User';

const API_URL = 'https://localhost:7106/api';

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export const authApi = {
    login: (data: LoginRequest) => api.post<AuthResponse>('/Auth/login', data),
    register: (data: any) => api.post<AuthResponse>('/Auth/register', data),
};

export const userApi = {
    getById: (id: string) => api.get(`/Users/${id}`),
    getByLogin: (login: string) => api.get(`/Users/by-login/${login}`),
};

export const accountApi = {
    getByUserId: (userId: string) => api.get(`/Accounts/user/${userId}`),
    getById: (id: string) => api.get(`/Accounts/${id}`),
    create: (data: any) => api.post('/Accounts', data),
    deposit: (id: string, amount: number) => api.post(`/Accounts/${id}/deposit`, amount),
    withdraw: (id: string, amount: number) => api.post(`/Accounts/${id}/withdraw`, amount),
};
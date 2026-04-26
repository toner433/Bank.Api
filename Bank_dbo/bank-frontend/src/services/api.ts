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

api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('userId');
            if (window.location.pathname !== '/login') {
                window.location.href = '/login';
            }
        }
        return Promise.reject(error);
    }
);

export const authApi = {
    login: (data: LoginRequest) => api.post<AuthResponse>('/Auth/login', data),
    register: (data: any) => api.post<AuthResponse>('/Auth/register', data),
    requestPasswordReset: (data: { email: string }) => api.post('/Auth/password-reset/request', data),
    confirmPasswordReset: (data: { email: string; code: string; newPassword: string; confirmPassword: string }) =>
        api.post('/Auth/password-reset/confirm', data),
};

export const userApi = {
    getById: (id: string) => api.get(`/Users/${id}`),
    getByLogin: (login: string) => api.get(`/Users/by-login/${login}`),
    updateProfile: (id: string, data: { fullName?: string; email?: string; phone?: string }) =>
        api.put(`/Users/${id}/profile`, data),
    changePassword: (id: string, data: { oldPassword: string; newPassword: string; confirmPassword: string }) =>
        api.post(`/Users/${id}/change-password`, data),
};

export const accountApi = {
    getAccessible: () => api.get('/Accounts/accessible'),
    getTransferRecipient: (accountNumber: string) =>
        api.get('/Accounts/transfer-recipient', { params: { accountNumber } }),
    getByUserId: (userId: string) => api.get(`/Accounts/user/${userId}`),
    getById: (id: string) => api.get(`/Accounts/${id}`),
    transfer: (data: {
        fromAccountId: string;
        toAccountNumber?: string;
        toAccountId?: string;
        amount: number;
        description?: string;
        recipientInn?: string;
    }) => api.post('/Accounts/transfer', data),
    getHistory: (id: string, params?: Record<string, unknown>) => api.get(`/Accounts/${id}/history`, { params }),
    create: (data: { currency: string; accountType: string; organizationId?: string }) => api.post('/Accounts', data),
    deposit: (id: string, amount: number) => api.post(`/Accounts/${id}/deposit`, amount),
    withdraw: (id: string, amount: number) => api.post(`/Accounts/${id}/withdraw`, amount),
};

export const cardApi = {
    getByUserId: (userId: string) => api.get(`/Cards/user/${userId}`),
    getById: (id: string) => api.get(`/Cards/${id}`),
    create: (data: any) => api.post('/Cards', data),
    block: (id: string) => api.post(`/Cards/${id}/block`),
    unblock: (id: string) => api.post(`/Cards/${id}/unblock`),
};

export const operationApi = {
    getById: (id: string) => api.get(`/Operations/${id}`),
    getUserOperations: (userId: string, params?: any) => api.get(`/Operations/user/${userId}`, { params }),
    getOrganizationOperations: (organizationId: string, params?: any) =>
        api.get(`/Operations/organization/${organizationId}`, { params }),
    downloadReceiptPdf: (id: string) => api.get(`/Operations/${id}/receipt.pdf`, { responseType: 'blob' }),
};

export const organizationApi = {
    register: (data: { name: string; inn: string; kpp?: string; legalAddress: string; publicKeyPem?: string }) =>
        api.post('/Organizations/register', data),
    my: () => api.get('/Organizations/my'),
    get: (id: string) => api.get(`/Organizations/${id}`),
    members: (id: string) => api.get(`/Organizations/${id}/members`),
    addMember: (id: string, data: { userLogin: string; role: string }) => api.post(`/Organizations/${id}/members`, data),
    removeMember: (orgId: string, userId: string) => api.delete(`/Organizations/${orgId}/members/${userId}`),
};

export const paymentOrderApi = {
    create: (data: {
        organizationId: string;
        fromAccountId: string;
        documentNumber?: string;
        documentDate?: string;
        amount: number;
        recipientName: string;
        recipientInn?: string;
        recipientKpp?: string;
        recipientAccountNumber?: string;
        recipientBankName?: string;
        recipientBankBik?: string;
        paymentPriority?: number;
        paymentType?: string;
        vatType?: string;
        vatAmount?: number;
        purpose: string;
    }) => api.post('/PaymentOrders', data),
    listByOrganization: (organizationId: string) => api.get(`/PaymentOrders/organization/${organizationId}`),
    sign: (id: string, data: { deviceDetected: boolean; signatureValue: string; certificateThumbprint?: string }) =>
        api.post(`/PaymentOrders/${id}/sign`, data),
    execute: (id: string, data: { deviceDetected: boolean }) => api.post(`/PaymentOrders/${id}/execute`, data),
    downloadDocumentPdf: (id: string) => api.get(`/PaymentOrders/${id}/document.pdf`, { responseType: 'blob' }),
};

export const adminApi = {
    stats: () => api.get('/Admin/stats'),
    users: () => api.get('/Admin/users'),
    organizations: () => api.get('/Admin/organizations'),
    accounts: () => api.get('/Admin/accounts'),
    blockUser: (id: string) => api.post(`/Admin/users/${id}/block`),
    unblockUser: (id: string) => api.post(`/Admin/users/${id}/unblock`),
    blockAccount: (id: string) => api.post(`/Admin/accounts/${id}/block`),
    unblockAccount: (id: string) => api.post(`/Admin/accounts/${id}/unblock`),
    updateAccount: (id: string, data: { adminComment?: string; balance?: number; currency?: string; accountType?: string; isBlocked?: boolean }) => api.put(`/Admin/accounts/${id}`, data),
};

export const depositApi = {
    open: (data: { organizationId?: string; fromAccountId: string; amount: number; termMonths: number }) =>
        api.post('/Deposits/open', data),
    my: () => api.get('/Deposits/my'),
    organization: (organizationId: string) => api.get(`/Deposits/organization/${organizationId}`),
    close: (data: { timeDepositId: string; targetAccountId: string }) => api.post('/Deposits/close', data),
};

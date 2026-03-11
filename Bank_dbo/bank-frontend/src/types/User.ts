export interface User {
    id: string;
    login: string;
    email: string;
    fullName: string;
    phone: string;
    isBlocked: boolean;
    createdAt: string;
}

export interface LoginRequest {
    login: string;
    password: string;
}

export interface AuthResponse {
    token: string;
    user: User;
    expiresAt: string;
}
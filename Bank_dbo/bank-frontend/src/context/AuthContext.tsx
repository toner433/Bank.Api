import React, { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { organizationApi } from '../services/api';

export type AuthContextValue = {
    token: string | null;
    userId: string | null;
    hasCorporateAccess: boolean;
    isAdmin: boolean;
    setSession: (token: string, userId: string, isAdmin: boolean) => Promise<void>;
    clearSession: () => void;
    refreshCorporate: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

const LS_ADMIN = 'isAdmin';

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [token, setToken] = useState<string | null>(() => localStorage.getItem('token'));
    const [userId, setUserId] = useState<string | null>(() => localStorage.getItem('userId'));
    const [hasCorporateAccess, setHasCorporateAccess] = useState(false);
    const [isAdmin, setIsAdmin] = useState(() => localStorage.getItem(LS_ADMIN) === 'true');

    const refreshCorporate = useCallback(async () => {
        const t = localStorage.getItem('token');
        if (!t) {
            setHasCorporateAccess(false);
            return;
        }
        try {
            const r = await organizationApi.my();
            const list = r.data as unknown[];
            setHasCorporateAccess(Array.isArray(list) && list.length > 0);
        } catch {
            setHasCorporateAccess(false);
        }
    }, []);

    useEffect(() => {
        if (token) void refreshCorporate();
        else setHasCorporateAccess(false);
    }, [token, refreshCorporate]);

    const setSession = useCallback(
        async (newToken: string, newUserId: string, adminFlag: boolean) => {
            localStorage.setItem('token', newToken);
            localStorage.setItem('userId', newUserId);
            localStorage.setItem(LS_ADMIN, adminFlag ? 'true' : 'false');
            setToken(newToken);
            setUserId(newUserId);
            setIsAdmin(adminFlag);
            await refreshCorporate();
        },
        [refreshCorporate]
    );

    const clearSession = useCallback(() => {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem(LS_ADMIN);
        setToken(null);
        setUserId(null);
        setHasCorporateAccess(false);
        setIsAdmin(false);
    }, []);

    const value = useMemo(
        () => ({
            token,
            userId,
            hasCorporateAccess,
            isAdmin,
            setSession,
            clearSession,
            refreshCorporate,
        }),
        [token, userId, hasCorporateAccess, isAdmin, setSession, clearSession, refreshCorporate]
    );

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth должен вызываться внутри AuthProvider');
    return ctx;
}

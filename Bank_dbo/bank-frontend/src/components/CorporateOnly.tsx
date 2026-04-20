import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

/** Доступ только если пользователь уже в организации (не только физлицо). */
const CorporateOnly: React.FC<{ children: React.ReactElement }> = ({ children }) => {
    const { hasCorporateAccess } = useAuth();
    if (!hasCorporateAccess) {
        return <Navigate to="/dashboard" replace />;
    }
    return children;
};

export default CorporateOnly;

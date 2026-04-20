import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import MainNavigation from './MainNavigation';
import AppFooter from './AppFooter';

const AppLayout: React.FC = () => {
    const { token } = useAuth();
    if (!token) return <Navigate to="/login" replace />;
    return (
        <div className="app-shell">
            <MainNavigation />
            <main className="app-main">
                <Outlet />
            </main>
            <AppFooter />
        </div>
    );
};

export default AppLayout;

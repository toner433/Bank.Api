import React from 'react';
import { NavLink, Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const subNav = ({ isActive }: { isActive: boolean }) =>
    'admin-subnav__link' + (isActive ? ' admin-subnav__link--active' : '');

const AdminLayout: React.FC = () => {
    const { isAdmin } = useAuth();
    if (!isAdmin) return <Navigate to="/dashboard" replace />;

    return (
    <div className="admin-layout">
        <div className="container">
            <div className="admin-layout__head">
                <h1 className="page-title">Панель администратора</h1>
                <nav className="admin-subnav" aria-label="Админ-меню">
                    <NavLink to="/admin" end className={subNav}>
                        Обзор
                    </NavLink>
                    <NavLink to="/admin/users" className={subNav}>
                        Пользователи
                    </NavLink>
                    <NavLink to="/admin/organizations" className={subNav}>
                        Организации
                    </NavLink>
                </nav>
            </div>
            <Outlet />
        </div>
    </div>
    );
};

export default AdminLayout;

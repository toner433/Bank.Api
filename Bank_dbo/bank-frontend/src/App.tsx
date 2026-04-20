import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Accounts from './pages/Accounts';
import Profile from './pages/Profile';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';
import AccountDetail from './pages/AccountDetail';
import Cards from './pages/Cards';
import CardDetail from './pages/CardDetail';
import CreateCard from './pages/CreateCard';
import Transfer from './pages/Transfer';
import CreateAccount from './pages/CreateAccount';
import CorporateList from './pages/CorporateList';
import CorporateRegister from './pages/CorporateRegister';
import CorporateOrganization from './pages/CorporateOrganization';
import Deposits from './pages/Deposits';
import CorporateOnly from './components/CorporateOnly';
import AppLayout from './components/AppLayout';
import AdminLayout from './components/AdminLayout';
import AdminDashboard from './pages/admin/AdminDashboard';
import AdminUsersPage from './pages/admin/AdminUsersPage';
import AdminOrganizationsPage from './pages/admin/AdminOrganizationsPage';
import { useAuth } from './context/AuthContext';

function AppRoutes() {
    const { token } = useAuth();
    const isAuthenticated = !!token;

    return (
        <Routes>
            <Route path="/login" element={isAuthenticated ? <Navigate to="/dashboard" replace /> : <Login />} />
            <Route path="/register" element={isAuthenticated ? <Navigate to="/dashboard" replace /> : <Register />} />

            <Route element={<AppLayout />}>
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="/accounts" element={<Accounts />} />
                <Route path="/accounts/:id" element={<AccountDetail />} />
                <Route path="/accounts/create" element={<CreateAccount />} />
                <Route path="/transfer" element={<Transfer />} />
                <Route path="/deposits" element={<Deposits />} />
                <Route path="/cards" element={<Cards />} />
                <Route path="/cards/create" element={<CreateCard />} />
                <Route path="/cards/:id" element={<CardDetail />} />
                <Route path="/profile" element={<Profile />} />

                <Route path="/corporate/register" element={<CorporateRegister />} />
                <Route
                    path="/corporate"
                    element={
                        <CorporateOnly>
                            <CorporateList />
                        </CorporateOnly>
                    }
                />
                <Route
                    path="/corporate/:orgId"
                    element={
                        <CorporateOnly>
                            <CorporateOrganization />
                        </CorporateOnly>
                    }
                />

                <Route path="/admin" element={<AdminLayout />}>
                    <Route index element={<AdminDashboard />} />
                    <Route path="users" element={<AdminUsersPage />} />
                    <Route path="organizations" element={<AdminOrganizationsPage />} />
                </Route>
            </Route>

            <Route path="/" element={<Navigate to={isAuthenticated ? '/dashboard' : '/login'} replace />} />
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    );
}

function App() {
    return (
        <BrowserRouter>
            <AppRoutes />
        </BrowserRouter>
    );
}

export default App;

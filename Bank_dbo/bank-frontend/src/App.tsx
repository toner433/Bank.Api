import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Accounts from './pages/Accounts';
import Profile from './pages/Profile';
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';

function App() {
    const isAuthenticated = !!localStorage.getItem('token');

    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route
                    path="/dashboard"
                    element={isAuthenticated ? <Dashboard /> : <Navigate to="/login" />}
                />
                <Route
                    path="/accounts"
                    element={isAuthenticated ? <Accounts /> : <Navigate to="/login" />}
                />
                <Route
                    path="/profile"
                    element={isAuthenticated ? <Profile /> : <Navigate to="/login" />}
                />
                <Route path="/" element={<Navigate to={isAuthenticated ? "/dashboard" : "/login"} />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
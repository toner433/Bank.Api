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


function App() {
    const isAuthenticated = !!localStorage.getItem('token');

    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
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
                <Route
                    path="/accounts/:id"
                    element={isAuthenticated ? <AccountDetail /> : <Navigate to="/login" />}
                />
              
                <Route
                    path="/transfer"
                    element={isAuthenticated ? <Transfer /> : <Navigate to="/login" />}
                />
                <Route
                    path="/cards/:id"
                    element={isAuthenticated ? <CardDetail /> : <Navigate to="/login" />}
                />
           
                <Route
                    path="/cards/create"
                    element={isAuthenticated ? <CreateCard /> : <Navigate to="/login" />}
                />
              
                <Route
                    path="/cards"
                    element={isAuthenticated ? <Cards /> : <Navigate to="/login" />}
                />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
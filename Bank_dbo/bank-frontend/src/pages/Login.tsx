import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../services/api';

const Login: React.FC = () => {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const response = await authApi.login({ login, password });
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('userId', response.data.user.id);
            navigate('/dashboard');
        } catch (err) {
            setError('Неверный логин или пароль');
        }
    };

    return (
        <>
            <header className="app-header">
                <div className="container">
                    <h1>D-bank<span>.</span></h1>
                </div>
            </header>

            <main>
                <div className="container">
                    <div className="row justify-content-center">
                        <div className="col-md-5">
                            <div className="card">
                                <div className="card-header">
                                    <h2>Вход</h2>
                                </div>

                                {error && (
                                    <div className="text-muted" style={{ marginBottom: '1rem' }}>
                                        {error}
                                    </div>
                                )}

                                <form onSubmit={handleSubmit}>
                                    <div className="form-group">
                                        <label className="form-label">Логин</label>
                                        <input
                                            type="text"
                                            className="form-input"
                                            value={login}
                                            onChange={(e) => setLogin(e.target.value)}
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label className="form-label">Пароль</label>
                                        <input
                                            type="password"
                                            className="form-input"
                                            value={password}
                                            onChange={(e) => setPassword(e.target.value)}
                                            required
                                        />
                                    </div>

                                    <button type="submit" className="btn btn-block">
                                        Войти
                                    </button>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
                <div style={{ marginTop: '1rem', textAlign: 'center' }}>
                    <Link to="/register" className="nav-link">
                        Нет аккаунта? Зарегистрироваться
                    </Link>
                </div>
            </main>


            <footer className="app-footer">
                <div className="container">
                    <p>D-bank © 2026</p>
                </div>
            </footer>
        </>
    );
};

export default Login;
import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../services/api';
import { useAuth } from '../context/AuthContext';
import { parseAuthResponse } from '../utils/parseAuthResponse';

const Login: React.FC = () => {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { setSession } = useAuth();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);
        try {
            const response = await authApi.login({ login, password });
            const payload = parseAuthResponse(response.data);
            if (!payload) {
                console.error('Ответ входа без token/user:', response.data);
                setError('Сервер вернул неожиданный ответ. Откройте консоль (F12).');
                return;
            }
            await setSession(payload.token, payload.userId, payload.isAdmin);
            navigate('/dashboard', { replace: true });
        } catch (err: unknown) {
            console.error('Ошибка входа:', err);
            const ax = err as {
                code?: string;
                message?: string;
                response?: { data?: { error?: string }; status?: number };
            };
            if (ax.code === 'ERR_NETWORK' || ax.message === 'Network Error') {
                setError(
                    'Нет связи с банком. Запустите API (dotnet run) и откройте https://localhost:7106 в браузере один раз, чтобы принять сертификат.'
                );
            } else if (ax.response?.status === 401) {
                setError(ax.response?.data?.error || 'Неверный логин или пароль');
            } else {
                setError(ax.response?.data?.error || 'Не удалось войти. Проверьте консоль (F12).');
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <header className="app-header">
                <div className="container">
                    <h1>
                        D-bank<span>.</span>
                    </h1>
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
                                    <div className="alert alert-danger m-3" role="alert">
                                        {error}
                                    </div>
                                )}

                                <form onSubmit={handleSubmit} className="p-3">
                                    <div className="form-group">
                                        <label className="form-label">Логин</label>
                                        <input
                                            type="text"
                                            className="form-input"
                                            value={login}
                                            onChange={(e) => setLogin(e.target.value)}
                                            required
                                            autoComplete="username"
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
                                            autoComplete="current-password"
                                        />
                                    </div>

                                    <button type="submit" className="btn btn-block" disabled={loading}>
                                        {loading ? 'Вход…' : 'Войти'}
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
                <p className="text-muted small" style={{ textAlign: 'center', marginTop: '1.5rem', maxWidth: '28rem', marginLeft: 'auto', marginRight: 'auto' }}>
                    Тестовый администратор: логин <strong>bankadmin</strong>, пароль <strong>Admin123!</strong> (только для разработки).
                </p>
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

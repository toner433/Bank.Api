import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { userApi } from '../services/api';
import { User } from '../types/User';

const Profile: React.FC = () => {
    const [user, setUser] = useState<User | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUser = async () => {
            const userId = localStorage.getItem('userId');
            if (userId) {
                const response = await userApi.getById(userId);
                setUser(response.data);
            }
        };
        fetchUser();
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        navigate('/login');
    };

    if (!user) return <div className="loading">Загрузка</div>;

    return (
        <>
            <header className="app-header">
                <div className="container">
                    <div className="d-flex justify-content-between align-items-center">
                        <h1>D-bank<span>.</span></h1>
                        <Link to="/dashboard" className="nav-link">
                            На главную
                        </Link>
                    </div>
                </div>
            </header>

            <main>
                <div className="container">
                    <div className="row justify-content-center">
                        <div className="col-md-8">
                            <div className="card">
                                <div className="card-header">
                                    <h2>Профиль</h2>
                                </div>

                                <div className="row">
                                    <div className="col-md-6">
                                        <div className="form-group">
                                            <div className="form-label">ФИО</div>
                                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>{user.fullName}</div>
                                        </div>

                                        <div className="form-group">
                                            <div className="form-label">Логин</div>
                                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>{user.login}</div>
                                        </div>

                                        <div className="form-group">
                                            <div className="form-label">Email</div>
                                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>{user.email}</div>
                                        </div>
                                    </div>

                                    <div className="col-md-6">
                                        <div className="form-group">
                                            <div className="form-label">Телефон</div>
                                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>{user.phone}</div>
                                        </div>

                                        <div className="form-group">
                                            <div className="form-label">Статус</div>
                                            <div>
                                                {user.isBlocked ?
                                                    <span className="badge badge-danger">Заблокирован</span> :
                                                    <span className="badge badge-success">Активен</span>
                                                }
                                            </div>
                                        </div>

                                        <div className="form-group">
                                            <div className="form-label">Дата регистрации</div>
                                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                                {new Date(user.createdAt).toLocaleDateString('ru-RU')}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <hr />

                                <button onClick={handleLogout} className="btn btn-danger">
                                    Выйти
                                </button>
                            </div>
                        </div>
                    </div>
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

export default Profile;
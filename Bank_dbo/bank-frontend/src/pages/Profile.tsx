import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { userApi } from '../services/api';
import { User } from '../types/User';
import { useAuth } from '../context/AuthContext';

const Profile: React.FC = () => {
    const [user, setUser] = useState<User | null>(null);
    const [edit, setEdit] = useState({ fullName: '', email: '', phone: '' });
    const [pwd, setPwd] = useState({ oldPassword: '', newPassword: '', confirmPassword: '' });
    const [msg, setMsg] = useState('');
    const [err, setErr] = useState('');
    const navigate = useNavigate();
    const { clearSession } = useAuth();

    useEffect(() => {
        const fetchUser = async () => {
            const userId = localStorage.getItem('userId');
            if (userId) {
                const response = await userApi.getById(userId);
                const u = response.data as User;
                setUser(u);
                setEdit({ fullName: u.fullName, email: u.email, phone: u.phone });
            }
        };
        fetchUser();
    }, []);

    const handleLogout = () => {
        clearSession();
        navigate('/login', { replace: true });
    };

    const saveProfile = async (e: React.FormEvent) => {
        e.preventDefault();
        setErr('');
        setMsg('');
        const userId = localStorage.getItem('userId');
        if (!userId) return;
        try {
            await userApi.updateProfile(userId, edit);
            setMsg('Профиль сохранён');
            const r = await userApi.getById(userId);
            setUser(r.data as User);
        } catch (e: any) {
            setErr(e.response?.data?.error || 'Ошибка сохранения');
        }
    };

    const savePassword = async (e: React.FormEvent) => {
        e.preventDefault();
        setErr('');
        setMsg('');
        const userId = localStorage.getItem('userId');
        if (!userId) return;
        try {
            await userApi.changePassword(userId, pwd);
            setMsg('Пароль изменён');
            setPwd({ oldPassword: '', newPassword: '', confirmPassword: '' });
        } catch (e: any) {
            setErr(e.response?.data?.error || 'Ошибка смены пароля');
        }
    };

    if (!user) return <div className="loading">Загрузка</div>;

    return (
        <div className="container page-stack">
            <h1 className="page-title">Профиль</h1>

            {err && <div className="alert alert-danger">{err}</div>}
            {msg && (
                <div className="alert" style={{ background: '#d4edda', color: '#155724' }}>
                    {msg}
                </div>
            )}

            <div className="row">
                <div className="col-md-6 mb-4">
                    <div className="card h-100">
                        <div className="card-header">
                            <h3>Данные</h3>
                        </div>
                        <div className="form-group">
                            <div className="form-label">ФИО</div>
                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                {user.fullName}
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="form-label">Логин</div>
                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                {user.login}
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="form-label">Статус</div>
                            <div>
                                {user.isBlocked ? (
                                    <span className="badge badge-danger">Заблокирован</span>
                                ) : (
                                    <span className="badge badge-success">Активен</span>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
                <div className="col-md-6 mb-4">
                    <div className="card h-100">
                        <div className="card-header">
                            <h3>Редактирование</h3>
                        </div>
                        <form onSubmit={saveProfile}>
                            <div className="form-group">
                                <label className="form-label">ФИО</label>
                                <input className="form-input" value={edit.fullName} onChange={(e) => setEdit({ ...edit, fullName: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label className="form-label">Email</label>
                                <input className="form-input" type="email" value={edit.email} onChange={(e) => setEdit({ ...edit, email: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label className="form-label">Телефон</label>
                                <input className="form-input" value={edit.phone} onChange={(e) => setEdit({ ...edit, phone: e.target.value })} />
                            </div>
                            <button type="submit" className="btn">
                                Сохранить
                            </button>
                        </form>
                    </div>
                </div>
            </div>

            <div className="card mb-4">
                <div className="card-header">
                    <h3>Смена пароля</h3>
                </div>
                <form onSubmit={savePassword}>
                    <div className="form-group">
                        <label className="form-label">Текущий пароль</label>
                        <input className="form-input" type="password" value={pwd.oldPassword} onChange={(e) => setPwd({ ...pwd, oldPassword: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Новый пароль</label>
                        <input className="form-input" type="password" value={pwd.newPassword} onChange={(e) => setPwd({ ...pwd, newPassword: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label className="form-label">Подтверждение</label>
                        <input className="form-input" type="password" value={pwd.confirmPassword} onChange={(e) => setPwd({ ...pwd, confirmPassword: e.target.value })} required />
                    </div>
                    <button type="submit" className="btn">
                        Сменить пароль
                    </button>
                </form>
            </div>

            <button type="button" onClick={handleLogout} className="btn btn-danger">
                Выйти из аккаунта
            </button>
        </div>
    );
};

export default Profile;

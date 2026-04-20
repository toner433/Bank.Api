import React, { useEffect, useState } from 'react';
import { adminApi } from '../../services/api';

const AdminDashboard: React.FC = () => {
    const [stats, setStats] = useState<{ usersCount: number; blockedUsersCount: number; organizationsCount: number; accountsCount: number } | null>(null);
    const [err, setErr] = useState('');

    useEffect(() => {
        adminApi
            .stats()
            .then((r) => setStats(r.data as any))
            .catch((e) => setErr(e.response?.data?.error || 'Нет доступа'));
    }, []);

    if (err) return <div className="alert alert-danger">{err}</div>;
    if (!stats) return <div className="loading">Загрузка…</div>;

    return (
        <div className="stat-grid">
            <div className="stat-card card">
                <div className="stat-card__label">Пользователей</div>
                <div className="stat-card__value">{stats.usersCount}</div>
            </div>
            <div className="stat-card card">
                <div className="stat-card__label">Заблокировано</div>
                <div className="stat-card__value">{stats.blockedUsersCount}</div>
            </div>
            <div className="stat-card card">
                <div className="stat-card__label">Организаций</div>
                <div className="stat-card__value">{stats.organizationsCount}</div>
            </div>
            <div className="stat-card card">
                <div className="stat-card__label">Счетов</div>
                <div className="stat-card__value">{stats.accountsCount}</div>
            </div>
        </div>
    );
};

export default AdminDashboard;

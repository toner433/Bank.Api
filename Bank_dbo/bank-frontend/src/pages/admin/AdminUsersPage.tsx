import React, { useEffect, useState } from 'react';
import { adminApi } from '../../services/api';
import { useAuth } from '../../context/AuthContext';

const AdminUsersPage: React.FC = () => {
    const { userId } = useAuth();
    const [rows, setRows] = useState<any[]>([]);
    const [err, setErr] = useState('');
    
    const [status, setStatus] = useState('all');
    const [role, setRole] = useState('all');
    const [search, setSearch] = useState('');
    
    const [sortBy, setSortBy] = useState('createdAt');
    const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');

    const load = () => {
        setErr('');
        adminApi
            .users({ status, role, search, sortBy, sortOrder })
            .then((r) => setRows(r.data as any[]))
            .catch((e) => setErr(e.response?.data?.error || 'Ошибка'));
    };

    useEffect(() => {
        load();
    }, [status, role, sortBy, sortOrder]);

    const handleSearch = () => {
        load();
    };

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter') handleSearch();
    };

    const toggleSort = (field: string) => {
        if (sortBy === field) {
            setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
        } else {
            setSortBy(field);
            setSortOrder('desc');
        }
    };

    const toggle = async (id: string, block: boolean) => {
        setErr('');
        try {
            if (block) await adminApi.blockUser(id);
            else await adminApi.unblockUser(id);
            await load();
        } catch (e: any) {
            setErr(e.response?.data?.error || 'Ошибка');
        }
    };

    const getSortIcon = (field: string) => {
        if (sortBy !== field) return null;
        return sortOrder === 'asc' ? ' ↑' : ' ↓';
    };

    if (err && rows.length === 0) return <div className="alert alert-danger">{err}</div>;

    return (
        <div>
            {err && <div className="alert alert-danger">{err}</div>}
            
            <div className="card mb-3" style={{ padding: '1rem' }}>
                <div className="row" style={{ gap: '0.5rem 0', alignItems: 'flex-end' }}>
                    <div className="col-md-3">
                        <div className="form-group" style={{ marginBottom: 0 }}>
                            <label className="form-label">Статус</label>
                            <select 
                                className="form-input" 
                                value={status} 
                                onChange={(e) => setStatus(e.target.value)}
                            >
                                <option value="all">Все</option>
                                <option value="active">Активные</option>
                                <option value="blocked">Заблокированные</option>
                            </select>
                        </div>
                    </div>
                    <div className="col-md-3">
                        <div className="form-group" style={{ marginBottom: 0 }}>
                            <label className="form-label">Роль</label>
                            <select 
                                className="form-input" 
                                value={role} 
                                onChange={(e) => setRole(e.target.value)}
                            >
                                <option value="all">Все</option>
                                <option value="admin">Администраторы</option>
                                <option value="client">Клиенты</option>
                            </select>
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="form-group" style={{ marginBottom: 0 }}>
                            <label className="form-label">Поиск</label>
                            <input
                                className="form-input"
                                type="text"
                                placeholder="Логин, ФИО или email"
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                                onKeyPress={handleKeyPress}
                            />
                        </div>
                    </div>
                    <div className="col-md-2 d-flex gap-2" style={{ marginBottom: 0, paddingTop: '1.5rem' }}>
                        <button type="button" className="btn btn--sm" onClick={handleSearch}>
                            Найти
                        </button>
                        <button 
                            type="button" 
                            className="btn btn--sm btn-outline-secondary"
                            onClick={() => { setStatus('all'); setRole('all'); setSearch(''); setSortBy('createdAt'); setSortOrder('desc'); }}
                        >
                            Сбросить
                        </button>
                    </div>
                </div>
            </div>

            <div className="table-wrap card" style={{ padding: 0, overflow: 'hidden' }}>
                <table className="data-table">
                    <thead>
                        <tr>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('login')}>
                                Логин{getSortIcon('login')}
                            </th>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('fullName')}>
                                ФИО{getSortIcon('fullName')}
                            </th>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('email')}>
                                Email{getSortIcon('email')}
                            </th>
                            <th>Статус</th>
                            <th>Роль</th>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('createdAt')}>
                                Дата регистрации{getSortIcon('createdAt')}
                            </th>
                            <th />
                        </tr>
                    </thead>
                    <tbody>
                        {rows.map((u) => (
                            <tr key={u.id}>
                                <td>{u.login}</td>
                                <td>{u.fullName}</td>
                                <td>{u.email}</td>
                                <td>{u.isBlocked ? <span className="badge badge-danger">Блок</span> : <span className="badge badge-success">ОК</span>}</td>
                                <td>{u.isAdmin ? 'Админ' : 'Клиент'}</td>
                                <td>{new Date(u.createdAt).toLocaleDateString('ru-RU')}</td>
                                <td>
                                    {u.id !== userId && !u.isAdmin && (
                                        u.isBlocked ? (
                                            <button type="button" className="btn btn--sm btn-success" onClick={() => toggle(u.id, false)}>
                                                Разблокировать
                                            </button>
                                        ) : (
                                            <button type="button" className="btn btn--sm btn-danger" onClick={() => toggle(u.id, true)}>
                                                Заблокировать
                                            </button>
                                        )
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                {rows.length === 0 && <p className="text-muted p-3 mb-0">Пользователи не найдены</p>}
            </div>
        </div>
    );
};

export default AdminUsersPage;

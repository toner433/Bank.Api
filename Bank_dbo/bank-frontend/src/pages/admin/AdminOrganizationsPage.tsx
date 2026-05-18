import React, { useEffect, useState } from 'react';
import { adminApi } from '../../services/api';

const AdminOrganizationsPage: React.FC = () => {
    const [rows, setRows] = useState<any[]>([]);
    const [err, setErr] = useState('');

    const [search, setSearch] = useState('');

    const [sortBy, setSortBy] = useState('createdAt');
    const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');

    const load = () => {
        adminApi
            .organizations({ search, sortBy, sortOrder })
            .then((r) => setRows(r.data as any[]))
            .catch((e) => setErr(e.response?.data?.error || 'Ошибка'));
    };

    useEffect(() => {
        load();
    }, [sortBy, sortOrder]);

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

    const getSortIcon = (field: string) => {
        if (sortBy !== field) return null;
        return sortOrder === 'asc' ? ' ↑' : ' ↓';
    };

    if (err) return <div className="alert alert-danger">{err}</div>;

    return (
        <div>
            <div className="card mb-3" style={{ padding: '1rem' }}>
                <div className="row" style={{ gap: '0.5rem 0', alignItems: 'flex-end' }}>
                    <div className="col-md-6">
                        <div className="form-group" style={{ marginBottom: 0 }}>
                            <label className="form-label">Поиск</label>
                            <input
                                className="form-input"
                                type="text"
                                placeholder="Название или ИНН"
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
                            onClick={() => { setSearch(''); setSortBy('createdAt'); setSortOrder('desc'); }}
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
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('name')}>
                                Название{getSortIcon('name')}
                            </th>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('inn')}>
                                ИНН{getSortIcon('inn')}
                            </th>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('createdAt')}>
                                Создана{getSortIcon('createdAt')}
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {rows.map((o) => (
                            <tr key={o.id}>
                                <td>{o.name}</td>
                                <td>{o.inn}</td>
                                <td>{new Date(o.createdAt).toLocaleString('ru-RU')}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                {rows.length === 0 && <p className="text-muted p-3 mb-0">Организации не найдены</p>}
            </div>
        </div>
    );
};

export default AdminOrganizationsPage;

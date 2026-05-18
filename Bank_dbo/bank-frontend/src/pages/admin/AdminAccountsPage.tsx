import React, { useEffect, useState } from 'react';
import { adminApi } from '../../services/api';

interface EditForm {
    adminComment: string;
    balance: string;
    currency: string;
    accountType: string;
    isBlocked: boolean;
}

const AdminAccountsPage: React.FC = () => {
    const [rows, setRows] = useState<any[]>([]);
    const [err, setErr] = useState('');
    const [editingId, setEditingId] = useState<string>('');
    const [editForm, setEditForm] = useState<EditForm>({
        adminComment: '',
        balance: '',
        currency: '',
        accountType: '',
        isBlocked: false,
    });

    const [currency, setCurrency] = useState('all');
    const [accountType, setAccountType] = useState('all');
    const [status, setStatus] = useState('all');
    const [search, setSearch] = useState('');

    const [sortBy, setSortBy] = useState('createdAt');
    const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');

    const load = () => {
        setErr('');
        adminApi
            .accounts({ currency, accountType, status, search, sortBy, sortOrder })
            .then((r) => setRows(r.data as any[]))
            .catch((e) => setErr(e.response?.data?.error || 'Ошибка'));
    };

    useEffect(() => {
        load();
    }, [currency, accountType, status, sortBy, sortOrder]);

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

    const startEdit = (a: any) => {
        setEditingId(a.id);
        setEditForm({
            adminComment: a.adminComment || '',
            balance: String(a.balance),
            currency: a.currency,
            accountType: a.accountType,
            isBlocked: a.isBlocked,
        });
    };

    const cancelEdit = () => {
        setEditingId('');
    };

    const saveEdit = async (id: string) => {
        setErr('');
        try {
            await adminApi.updateAccount(id, {
                adminComment: editForm.adminComment || undefined,
                balance: editForm.balance !== '' ? Number(editForm.balance) : undefined,
                currency: editForm.currency || undefined,
                accountType: editForm.accountType || undefined,
                isBlocked: editForm.isBlocked,
            });
            setEditingId('');
            await load();
        } catch (e: any) {
            setErr(e.response?.data?.error || 'Ошибка сохранения');
        }
    };

    if (err && rows.length === 0) return <div className="alert alert-danger">{err}</div>;

    return (
        <div>
            {err && <div className="alert alert-danger">{err}</div>}

            <div className="card mb-3" style={{ padding: '1rem' }}>
                <div className="row" style={{ gap: '0.5rem 0', alignItems: 'flex-end' }}>
                    <div className="col-md-2">
                        <div className="form-group" style={{ marginBottom: 0 }}>
                            <label className="form-label">Валюта</label>
                            <select 
                                className="form-input" 
                                value={currency} 
                                onChange={(e) => setCurrency(e.target.value)}
                            >
                                <option value="all">Все</option>
                                <option value="BYN">BYN</option>
                                <option value="USD">USD</option>
                                <option value="EUR">EUR</option>
                                <option value="RUB">RUB</option>
                            </select>
                        </div>
                    </div>
                    <div className="col-md-3">
                        <div className="form-group" style={{ marginBottom: 0 }}>
                            <label className="form-label">Тип счёта</label>
                            <select 
                                className="form-input" 
                                value={accountType} 
                                onChange={(e) => setAccountType(e.target.value)}
                            >
                                <option value="all">Все</option>
                                <option value="Debit">Дебетовый</option>
                                <option value="corporate_current">Корпоративный текущий</option>
                                <option value="time_deposit">Срочный вклад</option>
                            </select>
                        </div>
                    </div>
                    <div className="col-md-2">
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
                            <label className="form-label">Поиск</label>
                            <input
                                className="form-input"
                                type="text"
                                placeholder="Номер счёта или владелец"
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
                            onClick={() => { setCurrency('all'); setAccountType('all'); setStatus('all'); setSearch(''); setSortBy('createdAt'); setSortOrder('desc'); }}
                        >
                            Сбросить
                        </button>
                    </div>
                </div>
            </div>

            {editingId && (
                <div className="card mb-4" style={{ padding: '1.5rem' }}>
                    <h4 style={{ marginBottom: '1rem' }}>Редактирование счёта</h4>
                    <div className="row">
                        <div className="col-md-4">
                            <div className="form-group">
                                <label className="form-label">Баланс</label>
                                <input
                                    className="form-input"
                                    type="number"
                                    step="0.01"
                                    value={editForm.balance}
                                    onChange={(e) => setEditForm({ ...editForm, balance: e.target.value })}
                                />
                            </div>
                        </div>
                        <div className="col-md-3">
                            <div className="form-group">
                                <label className="form-label">Валюта</label>
                                <select
                                    className="form-input"
                                    value={editForm.currency}
                                    onChange={(e) => setEditForm({ ...editForm, currency: e.target.value })}
                                >
                                    <option value="BYN">BYN</option>
                                    <option value="USD">USD</option>
                                    <option value="EUR">EUR</option>
                                    <option value="RUB">RUB</option>
                                </select>
                            </div>
                        </div>
                        <div className="col-md-5">
                            <div className="form-group">
                                <label className="form-label">Тип счёта</label>
                                <select
                                    className="form-input"
                                    value={editForm.accountType}
                                    onChange={(e) => setEditForm({ ...editForm, accountType: e.target.value })}
                                >
                                    <option value="Debit">Дебетовый (Debit)</option>
                                    <option value="corporate_current">Корпоративный текущий</option>
                                    <option value="time_deposit">Срочный вклад</option>
                                </select>
                            </div>
                        </div>
                        <div className="col-md-8">
                            <div className="form-group">
                                <label className="form-label">Комментарий администратора</label>
                                <input
                                    className="form-input"
                                    value={editForm.adminComment}
                                    onChange={(e) => setEditForm({ ...editForm, adminComment: e.target.value })}
                                    placeholder="Необязательно"
                                />
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="form-group">
                                <label className="form-label">Статус</label>
                                <div className="d-flex align-items-center gap-3" style={{ paddingTop: '0.5rem' }}>
                                    <label className="d-flex align-items-center gap-2" style={{ cursor: 'pointer' }}>
                                        <input
                                            type="checkbox"
                                            checked={editForm.isBlocked}
                                            onChange={(e) => setEditForm({ ...editForm, isBlocked: e.target.checked })}
                                        />
                                        Заблокирован
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="d-flex gap-2 mt-2">
                        <button type="button" className="btn btn--sm" onClick={() => saveEdit(editingId)}>
                            Сохранить
                        </button>
                        <button type="button" className="btn btn--sm btn-outline-secondary" onClick={cancelEdit}>
                            Отмена
                        </button>
                    </div>
                </div>
            )}

            <div className="table-wrap card" style={{ padding: 0, overflow: 'hidden' }}>
                <table className="data-table">
                    <thead>
                        <tr>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('accountNumber')}>
                                Номер{getSortIcon('accountNumber')}
                            </th>
                            <th>Владелец</th>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('balance')}>
                                Баланс{getSortIcon('balance')}
                            </th>
                            <th>Тип</th>
                            <th>Статус</th>
                            <th>Комментарий</th>
                            <th style={{ cursor: 'pointer' }} onClick={() => toggleSort('createdAt')}>
                                Создан{getSortIcon('createdAt')}
                            </th>
                            <th />
                        </tr>
                    </thead>
                    <tbody>
                        {rows.map((a) => (
                            <tr key={a.id} style={editingId === a.id ? { background: 'rgba(10,37,64,0.04)' } : undefined}>
                                <td>{a.accountNumber}</td>
                                <td>{a.owner}</td>
                                <td>{a.balance} {a.currency}</td>
                                <td>{a.accountType}</td>
                                <td>
                                    {a.isBlocked
                                        ? <span className="badge badge-danger">Блок</span>
                                        : <span className="badge badge-success">ОК</span>}
                                </td>
                                <td>{a.adminComment || '—'}</td>
                                <td>{new Date(a.createdAt).toLocaleDateString('ru-RU')}</td>
                                <td>
                                    <button
                                        type="button"
                                        className="btn btn--sm btn-outline-secondary"
                                        onClick={() => editingId === a.id ? cancelEdit() : startEdit(a)}
                                    >
                                        {editingId === a.id ? 'Закрыть' : 'Изменить'}
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                {rows.length === 0 && <p className="text-muted p-3 mb-0">Счета не найдены</p>}
            </div>
        </div>
    );
};

export default AdminAccountsPage;

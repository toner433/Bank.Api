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

    const load = () =>
        adminApi
            .accounts()
            .then((r) => setRows(r.data as any[]))
            .catch((e) => setErr(e.response?.data?.error || 'Ошибка'));

    useEffect(() => {
        load();
    }, []);

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
                            <th>Номер</th>
                            <th>Владелец</th>
                            <th>Баланс</th>
                            <th>Тип</th>
                            <th>Статус</th>
                            <th>Комментарий</th>
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
            </div>
        </div>
    );
};

export default AdminAccountsPage;

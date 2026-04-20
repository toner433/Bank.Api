import React, { useEffect, useState } from 'react';
import { adminApi } from '../../services/api';
import { useAuth } from '../../context/AuthContext';

const AdminUsersPage: React.FC = () => {
    const { userId } = useAuth();
    const [rows, setRows] = useState<any[]>([]);
    const [err, setErr] = useState('');

    const load = () =>
        adminApi
            .users()
            .then((r) => setRows(r.data as any[]))
            .catch((e) => setErr(e.response?.data?.error || 'Ошибка'));

    useEffect(() => {
        load();
    }, []);

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

    if (err && rows.length === 0) return <div className="alert alert-danger">{err}</div>;

    return (
        <div>
            {err && <div className="alert alert-danger">{err}</div>}
            <div className="table-wrap card" style={{ padding: 0, overflow: 'hidden' }}>
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Логин</th>
                            <th>ФИО</th>
                            <th>Email</th>
                            <th>Статус</th>
                            <th>Роль</th>
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
            </div>
        </div>
    );
};

export default AdminUsersPage;

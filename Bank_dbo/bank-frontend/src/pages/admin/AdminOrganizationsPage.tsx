import React, { useEffect, useState } from 'react';
import { adminApi } from '../../services/api';

const AdminOrganizationsPage: React.FC = () => {
    const [rows, setRows] = useState<any[]>([]);
    const [err, setErr] = useState('');

    useEffect(() => {
        adminApi
            .organizations()
            .then((r) => setRows(r.data as any[]))
            .catch((e) => setErr(e.response?.data?.error || 'Ошибка'));
    }, []);

    if (err) return <div className="alert alert-danger">{err}</div>;

    return (
        <div className="table-wrap card" style={{ padding: 0, overflow: 'hidden' }}>
            <table className="data-table">
                <thead>
                    <tr>
                        <th>Название</th>
                        <th>ИНН</th>
                        <th>Создана</th>
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
            {rows.length === 0 && <p className="text-muted p-3 mb-0">Нет организаций</p>}
        </div>
    );
};

export default AdminOrganizationsPage;

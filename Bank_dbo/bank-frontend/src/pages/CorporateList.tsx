import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { organizationApi } from '../services/api';

const CorporateList: React.FC = () => {
    const [orgs, setOrgs] = useState<any[]>([]);

    useEffect(() => {
        organizationApi
            .my()
            .then((r) => setOrgs(r.data as any[]))
            .catch(() => setOrgs([]));
    }, []);

    return (
        <div className="container page-stack">
            <div className="page-toolbar">
                <h1 className="page-title mb-0">Мои организации</h1>
                <Link to="/corporate/register" className="btn">
                    Новая организация
                </Link>
            </div>
            <div className="row">
                {orgs.map((o) => (
                    <div key={o.id} className="col-md-6 mb-3">
                        <div className="card h-100">
                            <div className="card-header">
                                <h3>{o.name}</h3>
                            </div>
                            <div className="p-3">
                                <p className="text-muted mb-1">ИНН {o.inn}</p>
                                <p className="text-muted small">{o.legalAddress}</p>
                                <Link to={`/corporate/${o.id}`} className="btn">
                                    Управление
                                </Link>
                            </div>
                        </div>
                    </div>
                ))}
                {orgs.length === 0 && (
                    <div className="col-12">
                        <div className="card p-4 text-center text-muted">
                            Нет организаций. <Link to="/corporate/register">Зарегистрировать</Link>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default CorporateList;

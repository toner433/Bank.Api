import React, { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { accountApi } from '../services/api';

const CreateAccount: React.FC = () => {
    const [search] = useSearchParams();
    const orgId = search.get('organizationId');
    const [currency, setCurrency] = useState('BYN');
    const [accountType, setAccountType] = useState(orgId ? 'corporate_current' : 'Debit');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (!localStorage.getItem('userId')) throw new Error('Пользователь не авторизован');
            const body: { currency: string; accountType: string; organizationId?: string } = { currency, accountType };
            if (orgId) body.organizationId = orgId;
            await accountApi.create(body);
            navigate(orgId ? `/corporate/${orgId}` : '/accounts');
        } catch (err: any) {
            setError(err.response?.data?.error || 'Ошибка при создании счёта');
        }
    };

    return (
        <div className="container page-stack">
            <h1 className="page-title">{orgId ? 'Корпоративный счёт' : 'Открытие счёта'}</h1>
            <div className="row justify-content-center">
                <div className="col-md-6">
                    <div className="card">
                        <div className="card-header">
                            <h2>Реквизиты</h2>
                        </div>
                        {orgId && <p className="text-muted small px-3 pt-2 mb-0">Счёт будет привязан к организации (только директор).</p>}
                        {error && <div className="alert alert-danger m-3">{error}</div>}
                        <form onSubmit={handleSubmit} className="p-3 pt-0">
                            <div className="form-group">
                                <label className="form-label">Валюта</label>
                                <select className="form-input" value={currency} onChange={(e) => setCurrency(e.target.value)}>
                                    <option value="BYN">BYN</option>
                                    <option value="USD">USD</option>
                                    <option value="EUR">EUR</option>
                                    <option value="RUB">RUB</option>
                                </select>
                            </div>
                            <div className="form-group">
                                <label className="form-label">Тип счёта</label>
                                <select className="form-input" value={accountType} onChange={(e) => setAccountType(e.target.value)}>
                                    <option value="Debit">Дебетовый</option>
                                    <option value="Credit">Кредитный</option>
                                </select>
                            </div>
                            <button type="submit" className="btn btn-block">
                                Открыть
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CreateAccount;

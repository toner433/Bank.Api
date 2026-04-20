import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { cardApi, accountApi } from '../services/api';

const CreateCard: React.FC = () => {
    const [accounts, setAccounts] = useState<any[]>([]);
    const [formData, setFormData] = useState({
        accountId: '',
        cardType: 'Debit',
        cardHolderName: '',
    });
    const [error, setError] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const fetchAccounts = async () => {
            try {
                const userId = localStorage.getItem('userId');
                if (userId) {
                    const response = await accountApi.getAccessible();
                    const acc = response.data as any[];
                    setAccounts(acc.filter((a) => !a.organizationId && a.accountType !== 'time_deposit'));
                }
            } catch (e) {
                console.error('Ошибка загрузки счетов', e);
            }
        };
        fetchAccounts();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await cardApi.create(formData);
            navigate('/cards');
        } catch (err: any) {
            setError(err.response?.data?.error || 'Ошибка при создании карты');
        }
    };

    return (
        <div className="container page-stack">
            <h1 className="page-title">Заказать карту</h1>
            <div className="row justify-content-center">
                <div className="col-md-6">
                    <div className="card">
                        <div className="card-header">
                            <h2>Данные</h2>
                        </div>

                        {error && <div className="alert alert-danger">{error}</div>}

                        <form onSubmit={handleSubmit}>
                            <div className="form-group">
                                <label className="form-label">Счет для привязки</label>
                                <select
                                    className="form-input"
                                    value={formData.accountId}
                                    onChange={(e) => setFormData({ ...formData, accountId: e.target.value })}
                                    required
                                >
                                    <option value="">Выберите счет</option>
                                    {accounts.map((acc: any) => (
                                        <option key={acc.id} value={acc.id}>
                                            {acc.accountNumber} ({acc.balance} {acc.currency})
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="form-group">
                                <label className="form-label">Тип карты</label>
                                <select
                                    className="form-input"
                                    value={formData.cardType}
                                    onChange={(e) => setFormData({ ...formData, cardType: e.target.value })}
                                    required
                                >
                                    <option value="Debit">Дебетовая</option>
                                    <option value="Credit">Кредитная</option>
                                </select>
                            </div>

                            <div className="form-group">
                                <label className="form-label">Имя на карте</label>
                                <input
                                    type="text"
                                    className="form-input"
                                    value={formData.cardHolderName}
                                    onChange={(e) => setFormData({ ...formData, cardHolderName: e.target.value })}
                                    placeholder="IVAN IVANOV"
                                    required
                                />
                            </div>

                            <button type="submit" className="btn btn-block">
                                Заказать карту
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CreateCard;

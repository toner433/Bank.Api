import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { accountApi, operationApi } from '../services/api';

const Transfer: React.FC = () => {
    const [accounts, setAccounts] = useState([]);
    const [formData, setFormData] = useState({
        fromAccountId: '',
        toAccountNumber: '',
        amount: '',
        description: ''
    });
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const fetchAccounts = async () => {
            try {
                const userId = localStorage.getItem('userId');
                if (userId) {
                    const response = await accountApi.getByUserId(userId);
                    setAccounts(response.data);
                }
            } catch (error) {
                console.error('Ошибка загрузки счетов', error);
            }
        };
        fetchAccounts();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        try {
            const requestData = {
                fromAccountId: formData.fromAccountId,
                toAccountNumber: formData.toAccountNumber,
                amount: Number(formData.amount),
                description: formData.description || 'Перевод'
            };

            await operationApi.transfer(requestData);
            setSuccess('Перевод выполнен успешно!');
            setTimeout(() => navigate('/accounts'), 2000);
        } catch (err: any) {
            setError(err.response?.data?.error || 'Ошибка при переводе');
        }
    };

    return (
        <>
            <header className="app-header">
                <div className="container">
                    <div className="d-flex justify-content-between align-items-center">
                        <h1>D-bank<span>.</span></h1>
                        <Link to="/accounts" className="nav-link">
                            Назад к счетам
                        </Link>
                    </div>
                </div>
            </header>

            <main>
                <div className="container">
                    <div className="row justify-content-center">
                        <div className="col-md-6">
                            <div className="card">
                                <div className="card-header">
                                    <h2>Перевод средств</h2>
                                </div>

                                {error && (
                                    <div className="alert alert-danger">
                                        {error}
                                    </div>
                                )}

                                {success && (
                                    <div className="alert" style={{
                                        background: '#d4edda',
                                        borderColor: '#c3e6cb',
                                        color: '#155724'
                                    }}>
                                        {success}
                                    </div>
                                )}

                                <form onSubmit={handleSubmit}>
                                    <div className="form-group">
                                        <label className="form-label">Счет списания</label>
                                        <select
                                            className="form-input"
                                            value={formData.fromAccountId}
                                            onChange={(e) => setFormData({ ...formData, fromAccountId: e.target.value })}
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
                                        <label className="form-label">Номер счета получателя</label>
                                        <input
                                            type="text"
                                            className="form-input"
                                            value={formData.toAccountNumber}
                                            onChange={(e) => setFormData({ ...formData, toAccountNumber: e.target.value })}
                                            placeholder="40817810000000000001"
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label className="form-label">Сумма</label>
                                        <input
                                            type="number"
                                            className="form-input"
                                            value={formData.amount}
                                            onChange={(e) => setFormData({ ...formData, amount: e.target.value })}
                                            min="0.01"
                                            step="0.01"
                                            required
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label className="form-label">Назначение платежа</label>
                                        <input
                                            type="text"
                                            className="form-input"
                                            value={formData.description}
                                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                            placeholder="Перевод"
                                        />
                                    </div>

                                    <button type="submit" className="btn btn-block">
                                        Выполнить перевод
                                    </button>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            </main>

            <footer className="app-footer">
                <div className="container">
                    <p>D-bank © 2026</p>
                </div>
            </footer>
        </>
    );
};

export default Transfer;
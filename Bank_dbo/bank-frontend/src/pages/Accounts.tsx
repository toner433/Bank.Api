import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { accountApi } from '../services/api';

const Accounts: React.FC = () => {
    const [accounts, setAccounts] = useState([]);

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

    return (
        <>
            <header className="app-header">
                <div className="container">
                    <div className="d-flex justify-content-between align-items-center">
                        <h1>D-bank<span>.</span></h1>
                        <Link to="/dashboard" className="nav-link">
                            На главную
                        </Link>
                    </div>
                </div>
            </header>

            <main>
                <div className="container">
                    <div className="d-flex justify-content-between align-items-center mb-4">
                        <h2>Мои счета</h2>
                        <Link to="/transfer" className="btn">
                            Новый перевод
                        </Link>
                    </div>
                    <div className="row">
                        <div className="col-12">
                            <div className="card">
                                <div className="card-header">
                                    <h2>Счета</h2>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="row">
                        {accounts.map((account: any) => (
                            <div key={account.id} className="col-md-6">
                                <div className="card">
                                    <div className="card-header">
                                        <h3>{account.accountType === 'Debit' ? 'Текущий счет' : 'Кредитный счет'}</h3>
                                    </div>

                                    <div className="form-group">
                                        <div className="form-label">Номер счета</div>
                                        <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                            {account.accountNumber}
                                        </div>
                                    </div>

                                    <div className="form-group">
                                        <div className="form-label">Баланс</div>
                                        <div className="balance">
                                            {account.balance.toLocaleString('ru-RU')} {account.currency}
                                        </div>
                                    </div>

                                    <Link to={`/accounts/${account.id}`} className="btn">
                                        Подробнее
                                    </Link>
                                </div>
                            </div>
                        ))}

                        {accounts.length === 0 && (
                            <div className="col-12">
                                <div className="card">
                                    <p className="text-muted" style={{ textAlign: 'center', margin: 0 }}>
                                        У вас пока нет открытых счетов
                                    </p>
                                </div>
                            </div>
                        )}
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

export default Accounts;
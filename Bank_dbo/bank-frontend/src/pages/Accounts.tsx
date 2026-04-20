import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { accountApi } from '../services/api';

const Accounts: React.FC = () => {
    const [accounts, setAccounts] = useState<any[]>([]);

    useEffect(() => {
        const fetchAccounts = async () => {
            try {
                const userId = localStorage.getItem('userId');
                if (userId) {
                    const response = await accountApi.getAccessible();
                    setAccounts(response.data as any[]);
                }
            } catch (error) {
                console.error('Ошибка загрузки счетов', error);
            }
        };
        fetchAccounts();
    }, []);

    return (
        <div className="container page-stack">
            <div className="page-toolbar">
                <h1 className="page-title mb-0">Мои счета</h1>
                <div className="page-toolbar__actions">
                    <Link to="/accounts/create" className="btn">
                        Открыть счёт
                    </Link>
                    <Link to="/transfer" className="btn">
                        Перевод
                    </Link>
                </div>
            </div>

            <div className="row">
                {accounts.map((account: any) => (
                    <div key={account.id} className="col-md-6 mb-3">
                        <div className="card h-100">
                            <div className="card-header">
                                <h3>
                                    {account.organizationName ? `Корп.: ${account.organizationName}` : 'Личный счёт'}
                                    {' · '}
                                    {account.accountType === 'Debit'
                                        ? 'Текущий'
                                        : account.accountType === 'time_deposit'
                                          ? 'Вклад'
                                          : account.accountType}
                                </h3>
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
                                    {Number(account.balance).toLocaleString('ru-RU')} {account.currency}
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
                            <p className="text-muted mb-0" style={{ textAlign: 'center' }}>
                                У вас пока нет открытых счетов
                            </p>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Accounts;

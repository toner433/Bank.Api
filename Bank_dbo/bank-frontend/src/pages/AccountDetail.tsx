import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { accountApi } from '../services/api';

const AccountDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [account, setAccount] = useState<any>(null);
    const [operations, setOperations] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchAccountDetails = async () => {
            try {
                const accountRes = await accountApi.getById(id!);
                setAccount(accountRes.data);

                const historyRes = await accountApi.getHistory(id!);
                setOperations(historyRes.data as any[]);
            } catch (error) {
                console.error('Ошибка загрузки', error);
            } finally {
                setLoading(false);
            }
        };
        fetchAccountDetails();
    }, [id]);

    if (loading) return <div className="loading">Загрузка...</div>;
    if (!account) return <div className="container p-4">Счёт не найден или нет доступа</div>;

    return (
        <div className="container page-stack">
            <h1 className="page-title">Счёт {account.accountNumber}</h1>
            <div className="card">
                <div className="card-header">
                    <h2>Реквизиты</h2>
                </div>

                <div className="row">
                    <div className="col-md-6">
                        <div className="form-group">
                            <div className="form-label">Номер счета</div>
                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                {account.accountNumber}
                            </div>
                        </div>

                        <div className="form-group">
                            <div className="form-label">Тип счета</div>
                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                {account.accountType === 'Debit' ? 'Текущий' : account.accountType === 'time_deposit' ? 'Срочный вклад' : account.accountType}
                            </div>
                        </div>
                        {account.organizationName && (
                            <div className="form-group">
                                <div className="form-label">Организация</div>
                                <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                    {account.organizationName}
                                </div>
                            </div>
                        )}
                    </div>

                    <div className="col-md-6">
                        <div className="form-group">
                            <div className="form-label">Валюта</div>
                            <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                {account.currency}
                            </div>
                        </div>

                        <div className="form-group">
                            <div className="form-label">Баланс</div>
                            <div className="balance">
                                {Number(account.balance).toLocaleString('ru-RU')} {account.currency}
                            </div>
                        </div>
                    </div>
                </div>

                <div className="row" style={{ marginTop: '2rem' }}>
                    <div className="col-12">
                        <h3 style={{ marginBottom: '1rem' }}>История операций</h3>

                        {operations.length === 0 ? (
                            <p className="text-muted">Нет операций</p>
                        ) : (
                            operations.map((op: any) => {
                                const isOutgoing = op.fromAccountNumber === account.accountNumber;
                                const isIncoming = op.toAccountNumber === account.accountNumber;

                                let sign = '';
                                let color = '';

                                if (isOutgoing) {
                                    sign = '-';
                                    color = '#e74c3c';
                                } else if (isIncoming) {
                                    sign = '+';
                                    color = '#27ae60';
                                } else {
                                    sign = '';
                                    color = '#7f8c8d';
                                }

                                return (
                                    <div
                                        key={op.id}
                                        style={{
                                            padding: '1rem',
                                            borderBottom: '1px solid #ecf0f1',
                                            display: 'flex',
                                            justifyContent: 'space-between',
                                        }}
                                    >
                                        <div>
                                            <div style={{ fontWeight: 500 }}>{op.description}</div>
                                            <div style={{ color: '#7f8c8d', fontSize: '0.875rem' }}>{new Date(op.createdAt).toLocaleDateString('ru-RU')}</div>
                                        </div>
                                        <div style={{ color, fontWeight: 500 }}>
                                            {sign}
                                            {op.amount} {account.currency}
                                        </div>
                                    </div>
                                );
                            })
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AccountDetail;

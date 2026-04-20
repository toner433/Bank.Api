import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { cardApi, accountApi } from '../services/api';

const CardDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [card, setCard] = useState<any>(null);
    const [account, setAccount] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCard = async () => {
            try {
                const response = await cardApi.getById(id!);
                setCard(response.data);

                if (response.data.accountId) {
                    const accRes = await accountApi.getById(response.data.accountId);
                    setAccount(accRes.data);
                }
            } catch (error) {
                console.error('Ошибка загрузки карты', error);
            } finally {
                setLoading(false);
            }
        };
        fetchCard();
    }, [id]);

    const handleBlock = async () => {
        if (window.confirm('Заблокировать карту?')) {
            try {
                await cardApi.block(id!);
                alert('Карта заблокирована');
                navigate('/cards');
            } catch {
                alert('Ошибка при блокировке');
            }
        }
    };

    const handleUnblock = async () => {
        if (window.confirm('Разблокировать карту?')) {
            try {
                await cardApi.unblock(id!);
                alert('Карта разблокирована');
                navigate('/cards');
            } catch {
                alert('Ошибка при разблокировке');
            }
        }
    };

    if (loading) return <div className="loading">Загрузка...</div>;
    if (!card) return <div className="container p-4">Карта не найдена</div>;

    return (
        <div className="container page-stack">
            <h1 className="page-title">Карта</h1>
            <div className="row justify-content-center">
                <div className="col-md-8 col-lg-7">
                    <div className="card">
                        <div className="card-header">
                            <h2>Детали карты</h2>
                        </div>

                        <div className="bank-card-visual bank-card-visual--large">
                            <div className="bank-card-visual__number">{card.cardNumber}</div>
                            {account && (
                                <div className="bank-card-visual__balance">
                                    <div className="bank-card-visual__label">Баланс счёта</div>
                                    <div className="bank-card-visual__balance-val">
                                        {Number(account.balance).toLocaleString('ru-RU')} {account.currency}
                                    </div>
                                </div>
                            )}
                            <div className="bank-card-visual__row">
                                <div>
                                    <div className="bank-card-visual__label">Держатель</div>
                                    <div className="bank-card-visual__value">{card.cardHolderName}</div>
                                </div>
                                <div>
                                    <div className="bank-card-visual__label">Срок</div>
                                    <div className="bank-card-visual__value">
                                        {card.expiryDate?.length === 5 ? card.expiryDate : card.expiryDate?.substring(0, 5)}
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="row">
                            <div className="col-md-6">
                                <div className="form-group">
                                    <div className="form-label">Тип карты</div>
                                    <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                        {card.cardType === 'Debit' ? 'Дебетовая' : 'Кредитная'}
                                    </div>
                                </div>
                                <div className="form-group">
                                    <div className="form-label">Статус</div>
                                    <div>
                                        {card.isBlocked ? <span style={{ color: '#e74c3c' }}>Заблокирована</span> : <span style={{ color: '#27ae60' }}>Активна</span>}
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-6">
                                <div className="form-group">
                                    <div className="form-label">Счёт</div>
                                    <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                        {card.accountNumber}
                                    </div>
                                </div>
                                <div className="form-group">
                                    <div className="form-label">Дневной лимит</div>
                                    <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                        {card.dailyLimit ?? '—'} {card.currency || 'BYN'}
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="d-flex gap-3 flex-wrap mt-3">
                            {card.isBlocked ? (
                                <button type="button" onClick={handleUnblock} className="btn btn-success">
                                    Разблокировать
                                </button>
                            ) : (
                                <button type="button" onClick={handleBlock} className="btn btn-danger">
                                    Заблокировать
                                </button>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CardDetail;

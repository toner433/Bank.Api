import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { cardApi } from '../services/api';

const Cards: React.FC = () => {
    const [cards, setCards] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCards = async () => {
            try {
                const userId = localStorage.getItem('userId');
                if (userId) {
                    const response = await cardApi.getByUserId(userId);
                    setCards(response.data as any[]);
                }
            } catch (error) {
                console.error('Ошибка загрузки карт', error);
            } finally {
                setLoading(false);
            }
        };
        fetchCards();
    }, []);

    if (loading) return <div className="loading">Загрузка...</div>;

    return (
        <div className="container page-stack">
            <div className="page-toolbar">
                <h1 className="page-title mb-0">Мои карты</h1>
                <Link to="/cards/create" className="btn">
                    Заказать карту
                </Link>
            </div>

            <div className="row g-4">
                {cards.length === 0 ? (
                    <div className="col-12">
                        <div className="card">
                            <p className="text-muted mb-0" style={{ textAlign: 'center' }}>
                                У вас пока нет карт
                            </p>
                        </div>
                    </div>
                ) : (
                    cards.map((card: any) => (
                        <div key={card.id} className="col-md-6 col-lg-4">
                            <div className="card h-100">
                                <div className="card-header">
                                    <h3>{card.cardType === 'Debit' ? 'Дебетовая' : 'Кредитная'}</h3>
                                </div>

                                <div className="bank-card-visual-wrap">
                                    <div className="bank-card-visual">
                                        <div className="bank-card-visual__number">{card.cardNumber}</div>
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
                                </div>

                                <div className="row">
                                    <div className="col-6">
                                        <div className="form-group">
                                            <div className="form-label">Статус</div>
                                            <div>{card.isBlocked ? <span style={{ color: '#e74c3c' }}>Заблокирована</span> : <span style={{ color: '#27ae60' }}>Активна</span>}</div>
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <div className="form-label">Лимит</div>
                                            <div>
                                                {card.dailyLimit} {card.currency || 'BYN'}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <Link to={`/cards/${card.id}`} className="btn mt-2">
                                    Подробнее
                                </Link>
                            </div>
                        </div>
                    ))
                )}
            </div>
        </div>
    );
};

export default Cards;

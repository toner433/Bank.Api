import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { cardApi } from '../services/api';

const Cards: React.FC = () => {
    const [cards, setCards] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCards = async () => {
            try {
                const userId = localStorage.getItem('userId');
                if (userId) {
                    const response = await cardApi.getByUserId(userId);
                    setCards(response.data);
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
                        <h2>Мои карты</h2>
                        <Link to="/cards/create" className="btn">
                            Заказать карту
                        </Link>
                    </div>

                    <div className="row">
                        {cards.length === 0 ? (
                            <div className="col-12">
                                <div className="card">
                                    <p className="text-muted" style={{ textAlign: 'center', margin: 0 }}>
                                        У вас пока нет карт
                                    </p>
                                </div>
                            </div>
                        ) : (
                            cards.map((card: any) => (
                                <div key={card.id} className="col-md-6">
                                    <div className="card">
                                        <div className="card-header">
                                            <h3>{card.cardType === 'Debit' ? 'Дебетовая карта' : 'Кредитная карта'}</h3>
                                        </div>

                                        <div style={{
                                            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                            color: 'white',
                                            padding: '1.5rem',
                                            borderRadius: '8px',
                                            marginBottom: '1rem'
                                        }}>
                                            <div style={{ fontSize: '1.25rem', letterSpacing: '2px', marginBottom: '1rem' }}>
                                                {card.cardNumber}
                                            </div>
                                            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                                <div>
                                                    <div style={{ fontSize: '0.75rem', opacity: 0.8 }}>Держатель</div>
                                                    <div>{card.cardHolderName}</div>
                                                </div>
                                                <div>
                                                    <div style={{ fontSize: '0.75rem', opacity: 0.8 }}>Срок</div>
                                                    <div>{card.expiryDate}</div>
                                                </div>
                                            </div>
                                        </div>

                                        <div className="row">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <div className="form-label">Статус</div>
                                                    <div>
                                                        {card.isBlocked ?
                                                            <span style={{ color: '#e74c3c' }}>Заблокирована</span> :
                                                            <span style={{ color: '#27ae60' }}>Активна</span>
                                                        }
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <div className="form-label">Лимит</div>
                                                    <div>{card.dailyLimit} {card.currency}</div>
                                                </div>
                                            </div>
                                        </div>

                                        <Link to={`/cards/${card.id}`} className="btn">
                                            Подробнее
                                        </Link>
                                    </div>
                                </div>
                            ))
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

export default Cards;
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

                    
                    <div className="row g-4">
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
                                <div key={card.id} className="col-md-6 col-lg-4 col-xl-3">
                                    <div className="card h-100">
                                        <div className="card-header">
                                            <h3>{card.cardType === 'Debit' ? 'Дебетовая' : 'Кредитная'}</h3>
                                        </div>

                                       
                                        <div className="d-flex  mb-3">
                                            <div style={{
                                                background: 'linear-gradient(145deg, #0a2540 0%, #1e3a5f 100%)',
                                                color: '#ffffff',
                                                padding: '1.25rem',
                                                borderRadius: '12px',
                                                width: '300px',               
                                                height: '160px',               
                                                display: 'flex',
                                                flexDirection: 'column',
                                                justifyContent: 'space-between',
                                                boxShadow: '0 8px 20px rgba(10, 37, 64, 0.15)',
                                                border: '1px solid rgba(255,255,255,0.1)'
                                            }}>
                                               
                                                <div style={{
                                                    fontSize: '1.1rem',
                                                    letterSpacing: '2px',
                                                    fontFamily: 'monospace',
                                                    wordBreak: 'break-all'
                                                }}>
                                                    {card.cardNumber}
                                                </div>

                                               
                                                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                                    <div>
                                                        <div style={{ fontSize: '0.65rem', opacity: 0.7, textTransform: 'uppercase' }}>Держатель</div>
                                                        <div style={{ fontSize: '0.9rem', fontWeight: 500 }}>{card.cardHolderName}</div>
                                                    </div>
                                                    <div>
                                                        <div style={{ fontSize: '0.65rem', opacity: 0.7, textTransform: 'uppercase' }}>Срок</div>
                                                        <div style={{ fontSize: '0.9rem', fontWeight: 500 }}>
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
                                                    <div>
                                                        {card.isBlocked ? (
                                                            <span style={{ color: '#e74c3c' }}>Заблокирована</span>
                                                        ) : (
                                                            <span style={{ color: '#27ae60' }}>Активна</span>
                                                        )}
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <div className="form-label">Лимит</div>
                                                    <div>{card.dailyLimit} {card.currency || 'BYN'}</div>
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
import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { cardApi } from '../services/api';

const CardDetail: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [card, setCard] = useState<any>(null);
    const [loading, setLoading] = useState(true);
    const [showFullNumber, setShowFullNumber] = useState(false);

    useEffect(() => {
        const fetchCard = async () => {
            try {
                const response = await cardApi.getById(id!);
                setCard(response.data);
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
            } catch (error) {
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
            } catch (error) {
                alert('Ошибка при разблокировке');
            }
        }
    };

    if (loading) return <div className="loading">Загрузка...</div>;

    return (
        <>
            <header className="app-header">
                <div className="container">
                    <div className="d-flex justify-content-between align-items-center">
                        <h1>D-bank<span>.</span></h1>
                        <Link to="/cards" className="nav-link">
                            Назад к картам
                        </Link>
                    </div>
                </div>
            </header>

            <main>
                <div className="container">
                    <div className="card">
                        <div className="card-header">
                            <h2>Детали карты</h2>
                        </div>

                        <div style={{
                            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                            color: 'white',
                            padding: '2rem',
                            borderRadius: '8px',
                            marginBottom: '2rem'
                        }}>
                            <div style={{ fontSize: '1.5rem', letterSpacing: '3px', marginBottom: '1.5rem' }}>
                                {showFullNumber ? card.cardNumber : '**** **** **** ' + card.cardNumber.slice(-4)}
                            </div>
                            <button
                                onClick={() => setShowFullNumber(!showFullNumber)}
                                style={{
                                    background: 'transparent',
                                    border: '1px solid white',
                                    color: 'white',
                                    padding: '0.25rem 1rem',
                                    borderRadius: '4px',
                                    cursor: 'pointer',
                                    marginBottom: '1rem'
                                }}
                            >
                                {showFullNumber ? 'Скрыть номер' : 'Показать номер'}
                            </button>
                            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                <div>
                                    <div style={{ fontSize: '0.875rem', opacity: 0.8 }}>Держатель</div>
                                    <div style={{ fontSize: '1.25rem' }}>{card.cardHolderName}</div>
                                </div>
                                <div>
                                    <div style={{ fontSize: '0.875rem', opacity: 0.8 }}>Срок</div>
                                    <div style={{ fontSize: '1.25rem' }}>{card.expiryDate}</div>
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
                                        {card.isBlocked ?
                                            <span style={{ color: '#e74c3c' }}>Заблокирована</span> :
                                            <span style={{ color: '#27ae60' }}>Активна</span>
                                        }
                                    </div>
                                </div>
                            </div>

                            <div className="col-md-6">
                                <div className="form-group">
                                    <div className="form-label">Счет</div>
                                    <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                        {card.accountNumber}
                                    </div>
                                </div>

                                <div className="form-group">
                                    <div className="form-label">Дневной лимит</div>
                                    <div className="form-input" style={{ border: 'none', paddingLeft: 0 }}>
                                        {card.dailyLimit} BYN
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div style={{ display: 'flex', gap: '1rem', marginTop: '1rem' }}>
                            {card.isBlocked ? (
                                <button onClick={handleUnblock} className="btn btn-success">
                                    Разблокировать
                                </button>
                            ) : (
                                <button onClick={handleBlock} className="btn btn-danger">
                                    Заблокировать
                                </button>
                            )}
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

export default CardDetail;
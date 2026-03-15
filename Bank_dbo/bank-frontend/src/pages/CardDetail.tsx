import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
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
                    <div className="row justify-content-center">
                        <div className="col-md-7 col-lg-6">
                            <div className="card">
                                <div className="card-header">
                                    <h2>Детали карты</h2>
                                </div>

                               
                                <div style={{
                                    background: 'linear-gradient(145deg, #0a2540 0%, #1e3a5f 100%)',
                                    color: '#ffffff',
                                    padding: '2rem',
                                    borderRadius: '16px',
                                    marginBottom: '2rem',
                                    boxShadow: '0 10px 25px rgba(10, 37, 64, 0.2)',
                                    border: '1px solid rgba(255,255,255,0.1)'
                                }}>
                                    <div style={{
                                        fontSize: '1.5rem',
                                        letterSpacing: '3px',
                                        marginBottom: '1rem',
                                        fontFamily: 'monospace'
                                    }}>
                                        {card.cardNumber}
                                    </div>
                                    {account && (
                                        <div style={{ marginTop: '1.5rem', borderTop: '1px solid rgba(255,255,255,0.2)', paddingTop: '1rem' }}>
                                            <div style={{ fontSize: '0.75rem', opacity: 0.7, textTransform: 'uppercase' }}>Баланс счёта</div>
                                            <div style={{ fontSize: '1.5rem', fontWeight: 300 }}>
                                                {account.balance.toLocaleString('ru-RU')} {account.currency}
                                            </div>
                                        </div>
                                    )}
                                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>

                                        <div>
                                            <div style={{ fontSize: '0.75rem', opacity: 0.7, textTransform: 'uppercase' }}>Держатель</div>
                                            <div style={{ fontSize: '1.2rem', fontWeight: 500 }}>{card.cardHolderName}</div>
                                        </div>
                                        <div>
                                            <div style={{ fontSize: '0.75rem', opacity: 0.7, textTransform: 'uppercase' }}>Срок</div>
                                            <div style={{ fontSize: '1.2rem', fontWeight: 500 }}>
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
                                                {card.isBlocked ? (
                                                    <span style={{ color: '#e74c3c' }}>Заблокирована</span>
                                                ) : (
                                                    <span style={{ color: '#27ae60' }}>Активна</span>
                                                )}
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
                                                {card.dailyLimit} 1000 BYN
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div style={{ display: 'flex', gap: '1rem', marginTop: '1.5rem' }}>
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
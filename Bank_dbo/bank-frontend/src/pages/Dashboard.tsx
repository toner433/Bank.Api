import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Dashboard: React.FC = () => {
    const { hasCorporateAccess } = useAuth();

    return (
        <div className="container page-stack">
            <h1 className="page-title">Добро пожаловать</h1>
            <p className="text-muted page-lead">Выберите раздел в меню сверху или откройте быстрый доступ ниже.</p>

            <div className="row g-3">
                <div className="col-md-6 col-lg-4">
                    <div className="card h-100">
                        <div className="card-header">
                            <h3>Счета</h3>
                        </div>
                        <p className="text-muted mb-4">Личные и корпоративные счета</p>
                        <Link to="/accounts" className="btn">
                            Перейти
                        </Link>
                    </div>
                </div>
                <div className="col-md-6 col-lg-4">
                    <div className="card h-100">
                        <div className="card-header">
                            <h3>Переводы</h3>
                        </div>
                        <p className="text-muted mb-4">По номеру счёта в банке</p>
                        <Link to="/transfer" className="btn">
                            Перейти
                        </Link>
                    </div>
                </div>
                <div className="col-md-6 col-lg-4">
                    <div className="card h-100">
                        <div className="card-header">
                            <h3>Вклады</h3>
                        </div>
                        <p className="text-muted mb-4">Срочные вклады</p>
                        <Link to="/deposits" className="btn">
                            Перейти
                        </Link>
                    </div>
                </div>
                {hasCorporateAccess ? (
                    <div className="col-md-6 col-lg-4">
                        <div className="card h-100">
                            <div className="card-header">
                                <h3>Организации</h3>
                            </div>
                            <p className="text-muted mb-4">Кабинет юрлица</p>
                            <Link to="/corporate" className="btn">
                                Перейти
                            </Link>
                        </div>
                    </div>
                ) : (
                    <div className="col-md-6 col-lg-4">
                        <div className="card h-100">
                            <div className="card-header">
                                <h3>Для бизнеса</h3>
                            </div>
                            <p className="text-muted mb-4">Регистрация организации в банке</p>
                            <Link to="/corporate/register" className="btn">
                                Зарегистрировать
                            </Link>
                        </div>
                    </div>
                )}
                <div className="col-md-6 col-lg-4">
                    <div className="card h-100">
                        <div className="card-header">
                            <h3>Карты</h3>
                        </div>
                        <p className="text-muted mb-4">К личным счетам</p>
                        <Link to="/cards" className="btn">
                            Перейти
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Dashboard;

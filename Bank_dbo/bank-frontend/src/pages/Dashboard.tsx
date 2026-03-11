import React from 'react';
import { Link } from 'react-router-dom';

const Dashboard: React.FC = () => {
    return (
        <>
            <header className="app-header">
                <div className="container">
                    <div className="d-flex justify-content-between align-items-center">
                        <h1>D-bank<span>.</span></h1>
                        <Link to="/profile" className="nav-link">
                            Профиль
                        </Link>
                    </div>
                </div>
            </header>

            <main>
                <div className="container">
                    <div className="row">
                        <div className="col-12">
                            <div className="card">
                                <div className="card-header">
                                    <h2>Добро пожаловать</h2>
                                </div>
                                <p className="text-muted">Управляйте своими финансами</p>
                            </div>
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md-6">
                            <div className="card">
                                <div className="card-header">
                                    <h3>Счета</h3>
                                </div>
                                <p className="text-muted mb-4">Просмотр баланса и истории</p>
                                <Link to="/accounts" className="btn">
                                    Перейти
                                </Link>
                            </div>
                        </div>

                        <div className="col-md-6">
                            <div className="card">
                                <div className="card-header">
                                    <h3>Карты</h3>
                                </div>
                                <p className="text-muted mb-4">Управление картами</p>
                                <Link to="/cards" className="btn">
                                    Перейти
                                </Link>
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

export default Dashboard;
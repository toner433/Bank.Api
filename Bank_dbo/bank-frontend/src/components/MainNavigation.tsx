import React from 'react';
import { NavLink, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const navClass = ({ isActive }: { isActive: boolean }) =>
    'main-nav__link' + (isActive ? ' main-nav__link--active' : '');

const MainNavigation: React.FC = () => {
    const { hasCorporateAccess, isAdmin, clearSession } = useAuth();

    return (
        <header className="app-header app-header--nav">
            <div className="container app-header__inner">
                <Link to={isAdmin ? '/admin' : '/dashboard'} className="app-brand">
                    D-bank<span>.</span>
                </Link>
                <nav className="main-nav" aria-label="Основное меню">
                    {isAdmin ? (
                        <>
                            <NavLink to="/admin" className={navClass} end>
                                Админ-панель
                            </NavLink>
                            <NavLink to="/admin/users" className={navClass}>
                                Пользователи
                            </NavLink>
                            <NavLink to="/admin/organizations" className={navClass}>
                                Организации
                            </NavLink>
                        </>
                    ) : (
                        <>
                            <NavLink to="/dashboard" className={navClass} end>
                                Главная
                            </NavLink>
                            <NavLink to="/accounts" className={navClass}>
                                Счета
                            </NavLink>
                            <NavLink to="/transfer" className={navClass}>
                                Переводы
                            </NavLink>
                            <NavLink to="/deposits" className={navClass}>
                                Вклады
                            </NavLink>
                            <NavLink to="/cards" className={navClass}>
                                Карты
                            </NavLink>
                            {hasCorporateAccess && (
                                <NavLink to="/corporate" className={navClass}>
                                    Организации
                                </NavLink>
                            )}
                            {!hasCorporateAccess && (
                                <NavLink to="/corporate/register" className={navClass}>
                                    Для бизнеса
                                </NavLink>
                            )}
                        </>
                    )}
                </nav>
                <div className="main-nav__user">
                    {!isAdmin && (
                        <NavLink to="/profile" className={navClass}>
                            Профиль
                        </NavLink>
                    )}
                    <button
                        type="button"
                        className="btn btn--ghost btn--sm"
                        onClick={() => {
                            clearSession();
                            window.location.assign('/login');
                        }}
                    >
                        Выйти
                    </button>
                </div>
            </div>
        </header>
    );
};

export default MainNavigation;

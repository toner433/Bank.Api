import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../services/api';

const Register: React.FC = () => {
    const [formData, setFormData] = useState({
        login: '',
        password: '',
        email: '',
        phone: '',
        fullName: '',
        passportNumber: '',
        birthDate: ''
    });
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [success, setSuccess] = useState('');
    const navigate = useNavigate();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });

       
        if (errors[name]) {
            const newErrors = { ...errors };
            delete newErrors[name];
            setErrors(newErrors);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setErrors({});

        try {
            const requestData = {
                login: formData.login,
                password: formData.password,
                email: formData.email,
                phone: formData.phone,
                fullName: formData.fullName,
                passportNumber: formData.passportNumber,
                birthDate: formData.birthDate
            };

            await authApi.register(requestData);
            setSuccess('Регистрация успешна!');
            setTimeout(() => navigate('/login'), 2000);

        } catch (err: any) {
            console.log('Ошибка:', err.response?.data);

            
            if (err.response?.data?.error) {
                setErrors({ form: err.response.data.error });
            }
           
            else if (err.response?.data?.message) {
                setErrors({ form: err.response.data.message });
            }
           
            else if (err.response?.data?.error) {
                setErrors({ form: err.response.data.error });
            }
            
            
        }
    };

    return (
        <>
            <header className="app-header">
                <div className="container">
                    <div className="d-flex justify-content-between align-items-center">
                        <h1>D-bank<span>.</span></h1>
                        <Link to="/login" className="nav-link">
                            Вход
                        </Link>
                    </div>
                </div>
            </header>

            <main>
                <div className="container">
                    <div className="row justify-content-center">
                        <div className="col-md-8">
                            <div className="card">
                                <div className="card-header">
                                    <h2>Регистрация</h2>
                                </div>

                                {errors.form && (
                                    <div style={{
                                        background: '#f8d7da',
                                        border: '1px solid #f5c6cb',
                                        padding: '1rem',
                                        marginBottom: '1rem',
                                        color: '#721c24'
                                    }}>
                                        {errors.form}
                                    </div>
                                )}

                                {success && (
                                    <div style={{
                                        background: '#d4edda',
                                        border: '1px solid #c3e6cb',
                                        padding: '1rem',
                                        marginBottom: '1rem',
                                        color: '#155724'
                                    }}>
                                        {success}
                                    </div>
                                )}

                                <form onSubmit={handleSubmit}>
                                    <div className="row">
                                        <div className="col-md-6">
                                            <div style={{ marginBottom: '1.5rem' }}>
                                                <label style={{ display: 'block', marginBottom: '0.5rem', color: '#666' }}>
                                                    Логин
                                                </label>
                                                <input
                                                    type="text"
                                                    name="login"
                                                    style={{
                                                        width: '100%',
                                                        padding: '0.5rem 0',
                                                        border: 'none',
                                                        borderBottom: errors.login ? '1px solid #e74c3c' : '1px solid #ddd',
                                                        fontSize: '1rem'
                                                    }}
                                                    value={formData.login}
                                                    onChange={handleChange}
                                                    required
                                                />
                                                {errors.login && (
                                                    <div style={{ color: '#e74c3c', fontSize: '0.875rem', marginTop: '0.25rem' }}>
                                                        {errors.login}
                                                    </div>
                                                )}
                                            </div>
                                        </div>

                                        <div className="col-md-6">
                                            <div style={{ marginBottom: '1.5rem' }}>
                                                <label style={{ display: 'block', marginBottom: '0.5rem', color: '#666' }}>
                                                    Пароль
                                                </label>
                                                <input
                                                    type="password"
                                                    name="password"
                                                    style={{
                                                        width: '100%',
                                                        padding: '0.5rem 0',
                                                        border: 'none',
                                                        borderBottom: errors.password ? '1px solid #e74c3c' : '1px solid #ddd',
                                                        fontSize: '1rem'
                                                    }}
                                                    value={formData.password}
                                                    onChange={handleChange}
                                                    required
                                                />
                                                {errors.password && (
                                                    <div style={{ color: '#e74c3c', fontSize: '0.875rem', marginTop: '0.25rem' }}>
                                                        {errors.password}
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    </div>

                                    <div className="row">
                                        <div className="col-md-6">
                                            <div style={{ marginBottom: '1.5rem' }}>
                                                <label style={{ display: 'block', marginBottom: '0.5rem', color: '#666' }}>
                                                    Email
                                                </label>
                                                <input
                                                    type="email"
                                                    name="email"
                                                    style={{
                                                        width: '100%',
                                                        padding: '0.5rem 0',
                                                        border: 'none',
                                                        borderBottom: errors.email ? '1px solid #e74c3c' : '1px solid #ddd',
                                                        fontSize: '1rem'
                                                    }}
                                                    value={formData.email}
                                                    onChange={handleChange}
                                                    required
                                                />
                                                {errors.email && (
                                                    <div style={{ color: '#e74c3c', fontSize: '0.875rem', marginTop: '0.25rem' }}>
                                                        {errors.email}
                                                    </div>
                                                )}
                                            </div>
                                        </div>

                                        <div className="col-md-6">
                                            <div style={{ marginBottom: '1.5rem' }}>
                                                <label style={{ display: 'block', marginBottom: '0.5rem', color: '#666' }}>
                                                    Телефон
                                                </label>
                                                <input
                                                    type="tel"
                                                    name="phone"
                                                    style={{
                                                        width: '100%',
                                                        padding: '0.5rem 0',
                                                        border: 'none',
                                                        borderBottom: errors.phone ? '1px solid #e74c3c' : '1px solid #ddd',
                                                        fontSize: '1rem'
                                                    }}
                                                    value={formData.phone}
                                                    onChange={handleChange}
                                                    required
                                                />
                                                {errors.phone && (
                                                    <div style={{ color: '#e74c3c', fontSize: '0.875rem', marginTop: '0.25rem' }}>
                                                        {errors.phone}
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    </div>

                                    <div style={{ marginBottom: '1.5rem' }}>
                                        <label style={{ display: 'block', marginBottom: '0.5rem', color: '#666' }}>
                                            ФИО
                                        </label>
                                        <input
                                            type="text"
                                            name="fullName"
                                            style={{
                                                width: '100%',
                                                padding: '0.5rem 0',
                                                border: 'none',
                                                borderBottom: errors.fullName ? '1px solid #e74c3c' : '1px solid #ddd',
                                                fontSize: '1rem'
                                            }}
                                            value={formData.fullName}
                                            onChange={handleChange}
                                            required
                                        />
                                        {errors.fullName && (
                                            <div style={{ color: '#e74c3c', fontSize: '0.875rem', marginTop: '0.25rem' }}>
                                                {errors.fullName}
                                            </div>
                                        )}
                                    </div>

                                    <div className="row">
                                        <div className="col-md-6">
                                            <div style={{ marginBottom: '1.5rem' }}>
                                                <label style={{ display: 'block', marginBottom: '0.5rem', color: '#666' }}>
                                                    Номер паспорта
                                                </label>
                                                <input
                                                    type="text"
                                                    name="passportNumber"
                                                    style={{
                                                        width: '100%',
                                                        padding: '0.5rem 0',
                                                        border: 'none',
                                                        borderBottom: errors.passportNumber ? '1px solid #e74c3c' : '1px solid #ddd',
                                                        fontSize: '1rem'
                                                    }}
                                                    value={formData.passportNumber}
                                                    onChange={handleChange}
                                                    required
                                                />
                                                {errors.passportNumber && (
                                                    <div style={{ color: '#e74c3c', fontSize: '0.875rem', marginTop: '0.25rem' }}>
                                                        {errors.passportNumber}
                                                    </div>
                                                )}
                                            </div>
                                        </div>

                                        <div className="col-md-6">
                                            <div style={{ marginBottom: '1.5rem' }}>
                                                <label style={{ display: 'block', marginBottom: '0.5rem', color: '#666' }}>
                                                    Дата рождения
                                                </label>
                                                <input
                                                    type="date"
                                                    name="birthDate"
                                                    style={{
                                                        width: '100%',
                                                        padding: '0.5rem 0',
                                                        border: 'none',
                                                        borderBottom: errors.birthDate ? '1px solid #e74c3c' : '1px solid #ddd',
                                                        fontSize: '1rem'
                                                    }}
                                                    value={formData.birthDate}
                                                    onChange={handleChange}
                                                    required
                                                />
                                                {errors.birthDate && (
                                                    <div style={{ color: '#e74c3c', fontSize: '0.875rem', marginTop: '0.25rem' }}>
                                                        {errors.birthDate}
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    </div>

                                    <button type="submit" style={{
                                        width: '100%',
                                        padding: '0.75rem',
                                        background: 'transparent',
                                        border: '1px solid #2c3e50',
                                        color: '#2c3e50',
                                        fontSize: '1rem',
                                        cursor: 'pointer',
                                        marginTop: '1rem'
                                    }}>
                                        Зарегистрироваться
                                    </button>
                                </form>
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

export default Register;
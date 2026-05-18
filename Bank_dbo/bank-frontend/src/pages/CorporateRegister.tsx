import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { organizationApi } from '../services/api';
import { useAuth } from '../context/AuthContext';
import { generateOrgKeyPair } from '../utils/ecdsaSign';

const CorporateRegister: React.FC = () => {
    const [form, setForm] = useState({ name: '', inn: '', kpp: '', legalAddress: '' });
    const [error, setError] = useState('');
    const [generating, setGenerating] = useState(false);
    const navigate = useNavigate();
    const { refreshCorporate } = useAuth();

    const submit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setGenerating(true);
        try {
            
            const tempId = crypto.randomUUID();
            const publicKeyPem = await generateOrgKeyPair(tempId);

            const res = await organizationApi.register({
                name: form.name,
                inn: form.inn.replace(/\s/g, ''),
                kpp: form.kpp.replace(/\s/g, '') || undefined,
                legalAddress: form.legalAddress,
                publicKeyPem,
            });
            const data = res.data as { id?: string; Id?: string };
            const newId = data.id ?? data.Id;

            
            if (newId) {
                const privateJwk = localStorage.getItem(`org_private_key_${tempId}`);
                if (privateJwk) {
                    localStorage.setItem(`org_private_key_${newId}`, privateJwk);
                    localStorage.removeItem(`org_private_key_${tempId}`);
                }
                localStorage.setItem(`org_public_key_pem_${newId}`, publicKeyPem);
                localStorage.removeItem(`org_public_key_pem_${tempId}`);
            }

            await refreshCorporate();
            if (newId) navigate(`/corporate/${newId}`, { replace: true });
            else navigate('/corporate', { replace: true });
        } catch (err: any) {
            setError(err.response?.data?.error || 'Ошибка регистрации');
        } finally {
            setGenerating(false);
        }
    };

    return (
        <div className="container page-stack">
            <div className="page-toolbar">
                <h1 className="page-title mb-0">Регистрация организации</h1>
                <Link to="/corporate" className="nav-link">
                    К списку организаций
                </Link>
            </div>
            <div className="row justify-content-center">
                <div className="col-md-8">
                    <div className="card">
                        <div className="card-header">
                            <h2>Реквизиты</h2>
                            <p className="text-muted mb-0">
                                Это не замена регистрации человека: вы уже вошли как пользователь (физлицо). Здесь в систему добавляется
                                юрлицо (ИНН, адрес); вы назначаетесь директором и можете пригласить бухгалтера и открыть корпоративные счета.
                            </p>
                        </div>
                        {error && <div className="alert alert-danger m-3">{error}</div>}
                        <form onSubmit={submit} className="p-3">
                            <div className="form-group">
                                <label className="form-label">Наименование</label>
                                <input className="form-input" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
                            </div>
                            <div className="form-group">
                                <label className="form-label">ИНН</label>
                                <input
                                    className="form-input"
                                    value={form.inn}
                                    onChange={(e) => setForm({ ...form, inn: e.target.value })}
                                    maxLength={12}
                                    required
                                />
                            </div>
                            <div className="form-group">
                                <label className="form-label">КПП (необязательно)</label>
                                <input className="form-input" value={form.kpp} onChange={(e) => setForm({ ...form, kpp: e.target.value })} maxLength={9} />
                            </div>
                            <div className="form-group">
                                <label className="form-label">Юридический адрес</label>
                                <input className="form-input" value={form.legalAddress} onChange={(e) => setForm({ ...form, legalAddress: e.target.value })} required />
                            </div>
                            <button type="submit" className="btn btn-block" disabled={generating}>
                                {generating ? 'Генерация ключей ЭЦП…' : 'Зарегистрировать'}
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CorporateRegister;

import React, { useEffect, useState, useCallback } from 'react';
import { Link, useParams } from 'react-router-dom';
import {
    organizationApi,
    accountApi,
    paymentOrderApi,
    depositApi,
    operationApi,
} from '../services/api';

const CorporateOrganization: React.FC = () => {
    const { orgId } = useParams<{ orgId: string }>();
    const [org, setOrg] = useState<any>(null);
    const [members, setMembers] = useState<any[]>([]);
    const [accounts, setAccounts] = useState<any[]>([]);
    const [orders, setOrders] = useState<any[]>([]);
    const [deposits, setDeposits] = useState<any[]>([]);
    const [operations, setOperations] = useState<any[]>([]);
    const [error, setError] = useState('');
    const [memberLogin, setMemberLogin] = useState('');
    const [memberRole, setMemberRole] = useState('Accountant');
    const [newAccCurrency, setNewAccCurrency] = useState('BYN');
    const [poForm, setPoForm] = useState({
        fromAccountId: '',
        amount: '',
        recipientName: '',
        recipientInn: '',
        recipientAccountNumber: '',
        purpose: '',
    });
    const [depForm, setDepForm] = useState({ fromAccountId: '', amount: '', termMonths: 6 });

    const load = useCallback(async () => {
        if (!orgId) return;
        setError('');
        try {
            const [o, m, acc, ord, dep, op] = await Promise.all([
                organizationApi.get(orgId),
                organizationApi.members(orgId),
                accountApi.getAccessible(),
                paymentOrderApi.listByOrganization(orgId),
                depositApi.organization(orgId),
                operationApi.getOrganizationOperations(orgId, { limit: 100 }),
            ]);
            setOrg(o.data);
            setMembers(m.data);
            setAccounts((acc.data as any[]).filter(a => a.organizationId === orgId));
            setOrders(ord.data);
            setDeposits(dep.data);
            setOperations(op.data);
        } catch (e: any) {
            setError(e.response?.data?.error || 'Ошибка загрузки');
        }
    }, [orgId]);

    useEffect(() => {
        load();
    }, [load]);

    const addMember = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!orgId) return;
        try {
            await organizationApi.addMember(orgId, { userLogin: memberLogin, role: memberRole });
            setMemberLogin('');
            await load();
        } catch (e: any) {
            setError(e.response?.data?.error || 'Ошибка');
        }
    };

    const removeMember = async (userId: string) => {
        if (!orgId || !window.confirm('Удалить сотрудника?')) return;
        try {
            await organizationApi.removeMember(orgId, userId);
            await load();
        } catch (e: any) {
            setError(e.response?.data?.error || 'Ошибка');
        }
    };

    const openAccount = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!orgId) return;
        try {
            await accountApi.create({ currency: newAccCurrency, accountType: 'corporate_current', organizationId: orgId });
            await load();
        } catch (e: any) {
            setError(e.response?.data?.error || 'Ошибка');
        }
    };

    const createOrder = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!orgId) return;
        try {
            await paymentOrderApi.create({
                organizationId: orgId,
                fromAccountId: poForm.fromAccountId,
                amount: Number(poForm.amount),
                recipientName: poForm.recipientName,
                recipientInn: poForm.recipientInn || undefined,
                recipientAccountNumber: poForm.recipientAccountNumber || undefined,
                purpose: poForm.purpose,
            });
            setPoForm({ ...poForm, amount: '', purpose: '' });
            await load();
        } catch (e: any) {
            setError(e.response?.data?.error || 'Ошибка');
        }
    };

    const executeOrder = async (id: string) => {
        try {
            await paymentOrderApi.execute(id);
            await load();
        } catch (e: any) {
            setError(e.response?.data?.error || 'Ошибка исполнения');
        }
    };

    const openDeposit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!orgId) return;
        try {
            await depositApi.open({
                organizationId: orgId,
                fromAccountId: depForm.fromAccountId,
                amount: Number(depForm.amount),
                termMonths: Number(depForm.termMonths),
            });
            setDepForm({ ...depForm, amount: '' });
            await load();
        } catch (e: any) {
            setError(e.response?.data?.error || 'Ошибка');
        }
    };

    if (!orgId) return null;
    if (!org && !error) return <div className="loading">Загрузка…</div>;

    return (
        <div className="container page-stack">
            <div className="page-toolbar">
                <div>
                    {org && (
                        <>
                            <h1 className="page-title mb-1">{org.name}</h1>
                            <p className="text-muted mb-0">
                                ИНН {org.inn}
                                {org.kpp ? ` · КПП ${org.kpp}` : ''}
                            </p>
                        </>
                    )}
                    {!org && <h1 className="page-title mb-0">Организация</h1>}
                </div>
                <Link to="/corporate" className="nav-link">
                    Все организации
                </Link>
            </div>
            {error && <div className="alert alert-danger">{error}</div>}

                    <div className="row">
                        <div className="col-lg-6 mb-4">
                            <div className="card">
                                <div className="card-header"><h3>Сотрудники</h3></div>
                                <div className="p-3">
                                    <ul className="list-unstyled">
                                        {members.map(m => (
                                            <li key={m.id} className="d-flex justify-content-between align-items-center mb-2">
                                                <span>{m.userFullName} ({m.userLogin}) — <strong>{m.role === 'Director' ? 'Директор' : 'Бухгалтер'}</strong></span>
                                                <button type="button" className="btn btn-sm btn-outline-danger" onClick={() => removeMember(m.userId)}>Удалить</button>
                                            </li>
                                        ))}
                                    </ul>
                                    <form onSubmit={addMember} className="border-top pt-3 mt-3">
                                        <h4>Добавить</h4>
                                        <div className="form-group">
                                            <label className="form-label">Логин пользователя в банке</label>
                                            <input className="form-input" value={memberLogin} onChange={e => setMemberLogin(e.target.value)} required />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Роль</label>
                                            <select className="form-input" value={memberRole} onChange={e => setMemberRole(e.target.value)}>
                                                <option value="Accountant">Бухгалтер</option>
                                                <option value="Director">Директор</option>
                                            </select>
                                        </div>
                                        <button type="submit" className="btn">Добавить</button>
                                    </form>
                                </div>
                            </div>
                        </div>

                        <div className="col-lg-6 mb-4">
                            <div className="card">
                                <div className="card-header"><h3>Корпоративные счета</h3></div>
                                <div className="p-3">
                                    {accounts.map(a => (
                                        <div key={a.id} className="mb-2">
                                            <strong>{a.accountNumber}</strong> — {a.balance} {a.currency} ({a.accountType})
                                        </div>
                                    ))}
                                    <form onSubmit={openAccount} className="border-top pt-3 mt-3">
                                        <div className="form-group">
                                            <label className="form-label">Валюта нового счёта</label>
                                            <select className="form-input" value={newAccCurrency} onChange={e => setNewAccCurrency(e.target.value)}>
                                                <option value="BYN">BYN</option>
                                                <option value="USD">USD</option>
                                                <option value="EUR">EUR</option>
                                            </select>
                                        </div>
                                        <button type="submit" className="btn">Открыть счёт (только директор)</button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-lg-6 mb-4">
                            <div className="card">
                                <div className="card-header"><h3>Платёжные поручения</h3></div>
                                <div className="p-3">
                                    {orders.map(o => (
                                        <div key={o.id} className="mb-3 pb-3 border-bottom">
                                            <div>{o.recipientName} · {o.amount} · {o.status}</div>
                                            <div className="small text-muted">{o.purpose}</div>
                                            {o.status === 'Draft' && o.recipientAccountNumber && (
                                                <button type="button" className="btn btn-sm mt-1" onClick={() => executeOrder(o.id)}>Исполнить</button>
                                            )}
                                        </div>
                                    ))}
                                    <form onSubmit={createOrder} className="mt-3">
                                        <h4>Новое поручение</h4>
                                        <div className="form-group">
                                            <label className="form-label">Счёт списания</label>
                                            <select className="form-input" value={poForm.fromAccountId} onChange={e => setPoForm({ ...poForm, fromAccountId: e.target.value })} required>
                                                <option value="">—</option>
                                                {accounts.map(a => <option key={a.id} value={a.id}>{a.accountNumber}</option>)}
                                            </select>
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Сумма</label>
                                            <input className="form-input" type="number" value={poForm.amount} onChange={e => setPoForm({ ...poForm, amount: e.target.value })} required />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Получатель (наименование)</label>
                                            <input className="form-input" value={poForm.recipientName} onChange={e => setPoForm({ ...poForm, recipientName: e.target.value })} required />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">ИНН получателя</label>
                                            <input className="form-input" value={poForm.recipientInn} onChange={e => setPoForm({ ...poForm, recipientInn: e.target.value })} />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Номер счёта получателя в банке</label>
                                            <input className="form-input" value={poForm.recipientAccountNumber} onChange={e => setPoForm({ ...poForm, recipientAccountNumber: e.target.value })} />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Назначение платежа</label>
                                            <input className="form-input" value={poForm.purpose} onChange={e => setPoForm({ ...poForm, purpose: e.target.value })} required />
                                        </div>
                                        <button type="submit" className="btn">Создать черновик</button>
                                    </form>
                                </div>
                            </div>
                        </div>

                        <div className="col-lg-6 mb-4">
                            <div className="card">
                                <div className="card-header"><h3>Срочные вклады организации</h3></div>
                                <div className="p-3">
                                    {deposits.map(d => (
                                        <div key={d.id} className="mb-2 small">
                                            {d.depositAccountNumber} · {d.principal} {d.annualRatePercent}% годовых · до {new Date(d.maturityDate).toLocaleDateString('ru-RU')} · {d.status}
                                        </div>
                                    ))}
                                    <form onSubmit={openDeposit} className="border-top pt-3 mt-3">
                                        <div className="form-group">
                                            <label className="form-label">Со счёта</label>
                                            <select className="form-input" value={depForm.fromAccountId} onChange={e => setDepForm({ ...depForm, fromAccountId: e.target.value })} required>
                                                <option value="">—</option>
                                                {accounts.filter(a => a.accountType !== 'time_deposit').map(a => (
                                                    <option key={a.id} value={a.id}>{a.accountNumber}</option>
                                                ))}
                                            </select>
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Сумма</label>
                                            <input className="form-input" type="number" value={depForm.amount} onChange={e => setDepForm({ ...depForm, amount: e.target.value })} required />
                                        </div>
                                        <div className="form-group">
                                            <label className="form-label">Срок (мес.)</label>
                                            <select className="form-input" value={depForm.termMonths} onChange={e => setDepForm({ ...depForm, termMonths: Number(e.target.value) })}>
                                                <option value={3}>3</option>
                                                <option value={6}>6</option>
                                                <option value={12}>12</option>
                                            </select>
                                        </div>
                                        <button type="submit" className="btn">Открыть вклад</button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="card mb-5">
                        <div className="card-header"><h3>Выписка / история по организации</h3></div>
                        <div className="p-3 table-responsive">
                            <table className="table table-sm">
                                <thead><tr><th>Дата</th><th>Сумма</th><th>Тип</th><th>Описание</th></tr></thead>
                                <tbody>
                                    {operations.map(op => (
                                        <tr key={op.id}>
                                            <td>{new Date(op.createdAt).toLocaleString('ru-RU')}</td>
                                            <td>{op.amount}</td>
                                            <td>{op.operationType}</td>
                                            <td>{op.description}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                            {operations.length === 0 && <p className="text-muted">Нет операций</p>}
                        </div>
                    </div>
        </div>
    );
};

export default CorporateOrganization;

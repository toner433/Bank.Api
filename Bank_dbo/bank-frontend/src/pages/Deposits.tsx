import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { accountApi, depositApi, organizationApi } from '../services/api';

const Deposits: React.FC = () => {
    const [list, setList] = useState<any[]>([]);
    const [accounts, setAccounts] = useState<any[]>([]);
    const [fromId, setFromId] = useState('');
    const [amount, setAmount] = useState('');
    const [term, setTerm] = useState(6);
    const [closeId, setCloseId] = useState('');
    const [targetId, setTargetId] = useState('');
    const [error, setError] = useState('');
    const [msg, setMsg] = useState('');

    const load = useCallback(async () => {
        try {
            const accRes = await accountApi.getAccessible();
            const acc = accRes.data as any[];
            setAccounts(acc.filter((a) => String(a.accountType).toLowerCase() !== 'time_deposit'));

            const myDeps = await depositApi.my();
            const orgsRes = await organizationApi.my().catch(() => ({ data: [] }));
            const orgs = (orgsRes.data as any[]) || [];
            const orgParts = await Promise.all(
                orgs.map((o) =>
                    depositApi
                        .organization(o.id)
                        .then((r) => (r.data as any[]).map((d) => ({ ...d, __orgName: o.name })))
                        .catch(() => []),
                ),
            );
            const personal = (myDeps.data as any[]).map((d) => ({ ...d, __orgName: null as string | null }));
            setList([...personal, ...orgParts.flat()]);
        } catch {
            setList([]);
        }
    }, []);

    useEffect(() => {
        load();
    }, [load]);

    const selectedFrom = accounts.find((a) => a.id === fromId);
    const selectedDeposit = list.find((d) => d.id === closeId);

    const closeTargetAccounts = useMemo(() => {
        if (!selectedDeposit) return [];
        if (selectedDeposit.organizationId)
            return accounts.filter(
                (a) => a.organizationId === selectedDeposit.organizationId && String(a.accountType).toLowerCase() !== 'time_deposit',
            );
        return accounts.filter((a) => !a.organizationId && String(a.accountType).toLowerCase() !== 'time_deposit');
    }, [accounts, selectedDeposit]);

    const open = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setMsg('');
        try {
            const payload: { fromAccountId: string; amount: number; termMonths: number; organizationId?: string } = {
                fromAccountId: fromId,
                amount: Number(amount),
                termMonths: term,
            };
            if (selectedFrom?.organizationId) payload.organizationId = selectedFrom.organizationId;
            await depositApi.open(payload);
            setAmount('');
            setMsg('Вклад открыт');
            load();
        } catch (err: any) {
            setError(err.response?.data?.error || 'Ошибка');
        }
    };

    const close = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setMsg('');
        try {
            await depositApi.close({ timeDepositId: closeId, targetAccountId: targetId });
            setCloseId('');
            setTargetId('');
            setMsg('Вклад закрыт, средства переведены');
            load();
        } catch (err: any) {
            setError(err.response?.data?.error || 'Ошибка');
        }
    };

    return (
        <div className="container page-stack">
            <h1 className="page-title">Срочные вклады</h1>
            <p className="text-muted page-lead">
                Ставка: до 3 мес. — 5,5% годовых, до 6 — 7%, 12 мес. — 9,5%. Проценты начисляются при закрытии (упрощённая модель). Со счёта
                вклада нельзя переводить через «Переводы» — только закрытие здесь.
            </p>
            {error && <div className="alert alert-danger">{error}</div>}
            {msg && (
                <div className="alert" style={{ background: '#d4edda', color: '#155724' }}>
                    {msg}
                </div>
            )}

            <div className="row">
                <div className="col-md-6 mb-4">
                    <div className="card h-100">
                        <div className="card-header">
                            <h3>Открыть вклад</h3>
                        </div>
                        <form className="p-3" onSubmit={open}>
                            <div className="form-group">
                                <label className="form-label">Счёт списания</label>
                                <select className="form-input" value={fromId} onChange={(e) => setFromId(e.target.value)} required>
                                    <option value="">—</option>
                                    {accounts.map((a) => (
                                        <option key={a.id} value={a.id}>
                                            {a.accountNumber}
                                            {a.organizationName ? ` · ${a.organizationName}` : ' · личный'} ({a.balance} {a.currency})
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="form-group">
                                <label className="form-label">Сумма</label>
                                <input className="form-input" type="number" value={amount} onChange={(e) => setAmount(e.target.value)} required />
                            </div>
                            <div className="form-group">
                                <label className="form-label">Срок (мес.)</label>
                                <select className="form-input" value={term} onChange={(e) => setTerm(Number(e.target.value))}>
                                    <option value={3}>3</option>
                                    <option value={6}>6</option>
                                    <option value={12}>12</option>
                                </select>
                            </div>
                            <button type="submit" className="btn btn-block">
                                Открыть
                            </button>
                        </form>
                    </div>
                </div>
                <div className="col-md-6 mb-4">
                    <div className="card h-100">
                        <div className="card-header">
                            <h3>Закрыть вклад</h3>
                        </div>
                        <form className="p-3" onSubmit={close}>
                            <div className="form-group">
                                <label className="form-label">Вклад</label>
                                <select className="form-input" value={closeId} onChange={(e) => { setCloseId(e.target.value); setTargetId(''); }} required>
                                    <option value="">—</option>
                                    {list
                                        .filter((d) => d.status === 'Active')
                                        .map((d) => (
                                            <option key={d.id} value={d.id}>
                                                {d.__orgName ? `${d.__orgName}: ` : 'Личный: '}
                                                {d.depositAccountNumber} · {d.principal} {d.status === 'Active' ? 'Активен' : d.status === 'Closed' ? 'Закрыт' : d.status}
                                            </option>
                                        ))}
                                </select>
                            </div>
                            <div className="form-group">
                                <label className="form-label">Зачислить на счёт</label>
                                <select className="form-input" value={targetId} onChange={(e) => setTargetId(e.target.value)} required>
                                    <option value="">—</option>
                                    {closeTargetAccounts.map((a) => (
                                        <option key={a.id} value={a.id}>
                                            {a.accountNumber}
                                            {a.organizationName ? ` · ${a.organizationName}` : ' · личный'}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <button type="submit" className="btn btn-block">
                                Закрыть и перевести
                            </button>
                        </form>
                    </div>
                </div>
            </div>

            <div className="card">
                <div className="card-header">
                    <h3>Мои вклады</h3>
                </div>
                <ul className="list-group list-group-flush">
                    {list.map((d) => (
                        <li key={d.id} className="list-group-item">
                            {d.__orgName ? `${d.__orgName} · ` : 'Личный · '}
                            {d.depositAccountNumber} — {d.principal} @ {d.annualRatePercent}% — {d.status === 'Active' ? 'Активен' : d.status === 'Closed' ? 'Закрыт' : d.status}
                        </li>
                    ))}
                    {list.length === 0 && <li className="list-group-item text-muted">Нет вкладов</li>}
                </ul>
            </div>
        </div>
    );
};

export default Deposits;

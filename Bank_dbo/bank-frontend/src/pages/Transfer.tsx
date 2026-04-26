import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { accountApi, operationApi } from '../services/api';

type RecipientPreview = {
    accountNumber: string;
    currency: string;
    accountType: string;
    recipientKind: string;
    displayName: string;
    inn?: string | null;
};

const Transfer: React.FC = () => {
    const [accounts, setAccounts] = useState<any[]>([]);
    const [recipientMode, setRecipientMode] = useState<'mine' | 'external'>('mine');
    const [fromAccountId, setFromAccountId] = useState('');
    const [toAccountId, setToAccountId] = useState('');
    const [toAccountNumber, setToAccountNumber] = useState('');
    const [recipientInn, setRecipientInn] = useState('');
    const [amount, setAmount] = useState('');
    const [description, setDescription] = useState('');
    const [preview, setPreview] = useState<RecipientPreview | null>(null);
    const [previewLoading, setPreviewLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [completedOperationId, setCompletedOperationId] = useState<string | null>(null);
    const navigate = useNavigate();

    const fromAccounts = useMemo(
        () => accounts.filter((a) => String(a.accountType).toLowerCase() !== 'time_deposit'),
        [accounts],
    );

    const recipientMineOptions = useMemo(() => {
        if (!fromAccountId) return fromAccounts;
        return fromAccounts.filter((a) => a.id !== fromAccountId);
    }, [fromAccounts, fromAccountId]);

    useEffect(() => {
        accountApi
            .getAccessible()
            .then((r) => setAccounts(r.data as any[]))
            .catch(() => setAccounts([]));
    }, []);

    useEffect(() => {
        if (recipientMode !== 'external') {
            setPreview(null);
            return;
        }
        const num = toAccountNumber.replace(/\s/g, '');
        if (num.length < 10) {
            setPreview(null);
            return;
        }
        const t = window.setTimeout(() => {
            setPreviewLoading(true);
            accountApi
                .getTransferRecipient(num)
                .then((r) => setPreview(r.data as RecipientPreview))
                .catch(() => setPreview(null))
                .finally(() => setPreviewLoading(false));
        }, 500);
        return () => window.clearTimeout(t);
    }, [toAccountNumber, recipientMode]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setSuccess('');
        try {
            const base = {
                fromAccountId,
                amount: Number(amount),
                description: description.trim() || 'Перевод',
            };

            if (recipientMode === 'mine') {
                if (!toAccountId) {
                    setError('Выберите счёт получателя из списка');
                    return;
                }
                const res = await accountApi.transfer({ ...base, toAccountId });
                setCompletedOperationId((res.data as any).id);
            } else {
                const num = toAccountNumber.trim();
                if (!num) {
                    setError('Введите номер счёта получателя');
                    return;
                }
                const needInn = preview?.recipientKind === 'organization';
                const inn = recipientInn.replace(/\s/g, '');
                if (needInn && !inn) {
                    setError('Для перевода на счёт юридического лица укажите ИНН организации (как в платёжном поручении).');
                    return;
                }
                const res = await accountApi.transfer({
                    ...base,
                    toAccountNumber: num,
                    recipientInn: needInn ? inn : undefined,
                });
                setCompletedOperationId((res.data as any).id);
            }
            setSuccess('Перевод выполнен успешно!');
        } catch (err: any) {
            setError(err.response?.data?.error || 'Ошибка при переводе');
        }
    };

    return (
        <div className="container page-stack">
            <h1 className="page-title">Перевод средств</h1>
            <p className="text-muted page-lead">
                На счёт своей организации удобнее выбрать получателя из списка (без ИНН). Внешнему юрлицу по номеру счёта
                нужен ИНН для проверки реквизитов.
            </p>
            <div className="row justify-content-center">
                <div className="col-md-8 col-lg-7">
                    <div className="card">
                        <div className="card-header">
                            <h2>Новый перевод</h2>
                        </div>

                        {error && <div className="alert alert-danger">{error}</div>}
                        {success && (
                            <div className="alert" style={{ background: '#d4edda', borderColor: '#c3e6cb', color: '#155724' }}>
                                {success}
                            </div>
                        )}

                        <form onSubmit={handleSubmit}>
                            <div className="form-group">
                                <label className="form-label">Счёт списания</label>
                                <select
                                    className="form-input"
                                    value={fromAccountId}
                                    onChange={(e) => {
                                        setFromAccountId(e.target.value);
                                        setToAccountId('');
                                    }}
                                    required
                                >
                                    <option value="">Выберите счёт</option>
                                    {fromAccounts.map((acc: any) => (
                                        <option key={acc.id} value={acc.id}>
                                            {acc.accountNumber} — {acc.organizationName ? `«${acc.organizationName}»` : 'личный'}{' '}
                                            ({acc.balance} {acc.currency})
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="form-group">
                                <label className="form-label">Кому</label>
                                <div className="d-flex flex-wrap gap-3 mb-2">
                                    <label className="d-flex align-items-center gap-2" style={{ cursor: 'pointer' }}>
                                        <input
                                            type="radio"
                                            name="rec"
                                            checked={recipientMode === 'mine'}
                                            onChange={() => {
                                                setRecipientMode('mine');
                                                setToAccountNumber('');
                                                setRecipientInn('');
                                                setPreview(null);
                                            }}
                                        />
                                        <span>Мой счёт (личный или организации)</span>
                                    </label>
                                    <label className="d-flex align-items-center gap-2" style={{ cursor: 'pointer' }}>
                                        <input
                                            type="radio"
                                            name="rec"
                                            checked={recipientMode === 'external'}
                                            onChange={() => {
                                                setRecipientMode('external');
                                                setToAccountId('');
                                            }}
                                        />
                                        <span>По номеру счёта (другой клиент / внешнее юрлицо)</span>
                                    </label>
                                </div>

                                {recipientMode === 'mine' ? (
                                    <select
                                        className="form-input"
                                        value={toAccountId}
                                        onChange={(e) => setToAccountId(e.target.value)}
                                        required
                                    >
                                        <option value="">Выберите счёт получателя</option>
                                        {recipientMineOptions.map((acc: any) => (
                                            <option key={acc.id} value={acc.id}>
                                                {acc.accountNumber} — {acc.organizationName ? `«${acc.organizationName}»` : 'личный'}{' '}
                                                ({acc.balance} {acc.currency})
                                            </option>
                                        ))}
                                    </select>
                                ) : (
                                    <>
                                        <input
                                            type="text"
                                            className="form-input"
                                            value={toAccountNumber}
                                            onChange={(e) => {
                                                setToAccountNumber(e.target.value);
                                                setRecipientInn('');
                                            }}
                                            placeholder="Номер счёта в банке"
                                            required
                                        />
                                        {previewLoading && <p className="text-muted small mt-2">Проверка счёта…</p>}
                                        {preview && !previewLoading && (
                                            <div
                                                className="mt-3 p-3"
                                                style={{
                                                    borderRadius: 12,
                                                    border: '1px solid var(--border)',
                                                    background: 'rgba(10, 37, 64, 0.04)',
                                                }}
                                            >
                                                <div className="small text-muted mb-1">Получатель</div>
                                                <div style={{ fontWeight: 600 }}>{preview.displayName}</div>
                                                <div className="small mt-1">
                                                    <span
                                                        className="badge"
                                                        style={{
                                                            borderColor: preview.recipientKind === 'organization' ? '#0a2540' : '#4a5568',
                                                            color: preview.recipientKind === 'organization' ? '#0a2540' : '#4a5568',
                                                        }}
                                                    >
                                                        {preview.recipientKind === 'organization' ? 'Юридическое лицо' : 'Физическое лицо'}
                                                    </span>{' '}
                                                    · {preview.currency} · счёт {preview.accountNumber}
                                                </div>
                                                {preview.recipientKind === 'organization' && preview.inn && (
                                                    <p className="small text-muted mb-0 mt-2">
                                                        ИНН в банке по этому счёту: <strong>{preview.inn}</strong> — введите тот же ИНН ниже.
                                                    </p>
                                                )}
                                            </div>
                                        )}
                                        {preview?.recipientKind === 'organization' && (
                                            <div className="form-group mt-3">
                                                <label className="form-label">ИНН получателя (обязательно)</label>
                                                <input
                                                    type="text"
                                                    className="form-input"
                                                    value={recipientInn}
                                                    onChange={(e) => setRecipientInn(e.target.value)}
                                                    placeholder="Например, 123456789"
                                                    required
                                                />
                                            </div>
                                        )}
                                    </>
                                )}
                            </div>

                            <div className="form-group">
                                <label className="form-label">Сумма</label>
                                <input
                                    type="number"
                                    className="form-input"
                                    value={amount}
                                    onChange={(e) => setAmount(e.target.value)}
                                    min="0.01"
                                    step="0.01"
                                    required
                                />
                            </div>

                            <div className="form-group">
                                <label className="form-label">Назначение платежа (необязательно)</label>
                                <input
                                    type="text"
                                    className="form-input"
                                    value={description}
                                    onChange={(e) => setDescription(e.target.value)}
                                    placeholder="Можно оставить пустым"
                                />
                            </div>

                            <button type="submit" className="btn btn-block">
                                Выполнить перевод
                            </button>
                        </form>

                        {completedOperationId && (
                            <div className="mt-4 p-3" style={{ borderTop: '1px solid var(--border)' }}>
                                <div style={{ fontWeight: 600, marginBottom: '0.5rem' }}>Чек по операции</div>
                                <p className="text-muted small mb-3">Перевод проведён. Вы можете скачать квитанцию в формате PDF.</p>
                                <div className="d-flex gap-2">
                                    <button
                                        type="button"
                                        className="btn btn--sm"
                                        onClick={async () => {
                                            try {
                                                const response = await operationApi.downloadReceiptPdf(completedOperationId);
                                                const url = window.URL.createObjectURL(response.data);
                                                const link = document.createElement('a');
                                                link.href = url;
                                                link.download = `receipt-${completedOperationId}.pdf`;
                                                document.body.appendChild(link);
                                                link.click();
                                                document.body.removeChild(link);
                                                window.URL.revokeObjectURL(url);
                                            } catch {
                                                setError('Ошибка скачивания квитанции');
                                            }
                                        }}
                                    >
                                        Скачать квитанцию PDF
                                    </button>
                                    <button
                                        type="button"
                                        className="btn btn--sm btn-outline-secondary"
                                        onClick={() => navigate('/accounts')}
                                    >
                                        К счетам
                                    </button>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Transfer;

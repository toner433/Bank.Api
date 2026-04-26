/**
 * Генерация RSA ключевой пары и подпись через Web Crypto API.
 * Ключи хранятся в localStorage для каждой организации.
 */

const ALGO = {
    name: 'RSASSA-PKCS1-v1_5',
    modulusLength: 2048,
    publicExponent: new Uint8Array([1, 0, 1]),
    hash: 'SHA-256',
};

function privateKeyStorageKey(orgId: string) {
    return `org_private_key_${orgId}`;
}
function publicKeyStorageKey(orgId: string) {
    return `org_public_key_pem_${orgId}`;
}

/** Генерирует RSA ключевую пару для организации, сохраняет в localStorage, возвращает PEM публичный ключ */
export async function generateOrgKeyPair(orgId: string): Promise<string> {
    const keyPair = await crypto.subtle.generateKey(ALGO, true, ['sign', 'verify']);

    // Экспортируем приватный ключ как JWK и сохраняем
    const privateJwk = await crypto.subtle.exportKey('jwk', keyPair.privateKey);
    localStorage.setItem(privateKeyStorageKey(orgId), JSON.stringify(privateJwk));

    // Экспортируем публичный ключ в формате SPKI → PEM
    const spki = await crypto.subtle.exportKey('spki', keyPair.publicKey);
    const pem = spkiToPem(spki);
    localStorage.setItem(publicKeyStorageKey(orgId), pem);

    return pem;
}

/** Строит канонический payload — должен совпадать с серверной стороной */
export function buildSignPayload(order: {
    id: string;
    amount: number | string;
    recipientName: string;
    recipientAccountNumber?: string | null;
    purpose: string;
}): string {
    return `${order.id}|${order.amount}|${order.recipientName}|${order.recipientAccountNumber ?? ''}|${order.purpose}`;
}

/** Подписывает payload приватным ключом организации из localStorage */
export async function signPayload(orgId: string, payload: string): Promise<string> {
    const jwkRaw = localStorage.getItem(privateKeyStorageKey(orgId));
    if (!jwkRaw) throw new Error('Приватный ключ ЭЦП не найден. Возможно, организация зарегистрирована в другом браузере.');

    const jwk = JSON.parse(jwkRaw);
    const privateKey = await crypto.subtle.importKey('jwk', jwk, ALGO, false, ['sign']);

    const encoder = new TextEncoder();
    const data = encoder.encode(payload);
    const sigBuffer = await crypto.subtle.sign(ALGO.name, privateKey, data);

    return bufferToBase64(sigBuffer);
}

/** Проверяет наличие приватного ключа для организации в localStorage */
export function hasPrivateKey(orgId: string): boolean {
    return !!localStorage.getItem(privateKeyStorageKey(orgId));
}

function spkiToPem(spki: ArrayBuffer): string {
    const b64 = bufferToBase64(spki);
    const lines = b64.match(/.{1,64}/g)?.join('\n') ?? b64;
    return `-----BEGIN PUBLIC KEY-----\n${lines}\n-----END PUBLIC KEY-----`;
}

function bufferToBase64(buffer: ArrayBuffer): string {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    bytes.forEach(b => (binary += String.fromCharCode(b)));
    return btoa(binary);
}

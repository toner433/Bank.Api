/** Поддержка camelCase и PascalCase из ASP.NET. */
export function parseAuthResponse(data: unknown): { token: string; userId: string; isAdmin: boolean } | null {
    if (!data || typeof data !== 'object') return null;
    const d = data as Record<string, unknown>;
    const token = (d.token ?? d.Token) as string | undefined;
    const user = (d.user ?? d.User) as Record<string, unknown> | undefined;
    const userId = (user?.id ?? user?.Id) as string | undefined;
    if (!token || !userId) return null;
    const rawAdmin = user?.isAdmin ?? user?.IsAdmin;
    const isAdmin = rawAdmin === true || rawAdmin === 'true';
    return { token, userId, isAdmin };
}

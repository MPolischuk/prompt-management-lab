const defaultBase = 'https://localhost:7106';

export function getApiBaseUrl(): string {
  const u = import.meta.env.VITE_API_BASE_URL;
  return u && u.length > 0 ? u.replace(/\/$/, '') : defaultBase;
}

export interface OperationResult {
  success: boolean;
  entityId: string | null;
  message: string | null;
  errorCode: string | null;
}

const defaultHeaders: HeadersInit = {
  'Content-Type': 'application/json',
  'x-api-version': '1.0',
};

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number,
    public readonly operation?: OperationResult
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

async function parseJsonSafe<T>(res: Response): Promise<T | null> {
  const text = await res.text();
  if (!text) return null;
  try {
    return JSON.parse(text) as T;
  } catch {
    return null;
  }
}

export async function apiGet<T>(path: string): Promise<T> {
  const res = await fetch(`${getApiBaseUrl()}${path}`, { headers: defaultHeaders });
  if (!res.ok) {
    const op = await parseJsonSafe<OperationResult>(res);
    throw new ApiError(op?.message ?? res.statusText, res.status, op ?? undefined);
  }
  return (await res.json()) as T;
}

export async function apiDelete(path: string): Promise<OperationResult | null> {
  const res = await fetch(`${getApiBaseUrl()}${path}`, { method: 'DELETE', headers: defaultHeaders });
  const body = await parseJsonSafe<OperationResult>(res);
  if (!res.ok) {
    throw new ApiError(body?.message ?? res.statusText, res.status, body ?? undefined);
  }
  return body;
}

export async function apiPost<TBody extends object>(path: string, body: TBody): Promise<unknown> {
  const res = await fetch(`${getApiBaseUrl()}${path}`, {
    method: 'POST',
    headers: defaultHeaders,
    body: JSON.stringify(body),
  });
  const parsed = await parseJsonSafe<unknown>(res);
  if (!res.ok) {
    const op = parsed as OperationResult | null;
    throw new ApiError((op?.message as string) ?? res.statusText, res.status, op ?? undefined);
  }
  return parsed;
}

export async function apiPut<TBody extends object>(path: string, body: TBody): Promise<unknown> {
  const res = await fetch(`${getApiBaseUrl()}${path}`, {
    method: 'PUT',
    headers: defaultHeaders,
    body: JSON.stringify(body),
  });
  const parsed = await parseJsonSafe<unknown>(res);
  if (!res.ok) {
    const op = parsed as OperationResult | null;
    throw new ApiError((op?.message as string) ?? res.statusText, res.status, op ?? undefined);
  }
  return parsed;
}

export function assertSuccess(result: unknown): OperationResult {
  const op = result as OperationResult;
  if (!op?.success) {
    throw new ApiError(op?.message ?? 'Operation failed', 400, op);
  }
  return op;
}

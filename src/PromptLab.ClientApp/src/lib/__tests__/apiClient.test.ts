import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ApiError, apiDelete, apiGet, apiPost, apiPut, assertSuccess, getApiBaseUrl, type OperationResult } from '../apiClient';

describe('getApiBaseUrl', () => {
  afterEach(() => {
    vi.unstubAllEnvs();
  });

  it('usa VITE_API_BASE_URL sin barra final', () => {
    vi.stubEnv('VITE_API_BASE_URL', 'https://api.example.com/');
    expect(getApiBaseUrl()).toBe('https://api.example.com');
  });

  it('usa el fallback cuando no hay variable', () => {
    vi.stubEnv('VITE_API_BASE_URL', undefined);
    expect(getApiBaseUrl()).toBe('https://localhost:7106');
  });
});

describe('ApiError', () => {
  it('expone status, message y name', () => {
    const op: OperationResult = {
      success: false,
      entityId: null,
      message: 'falló',
      errorCode: 'E1',
    };
    const err = new ApiError('msg', 400, op);
    expect(err.name).toBe('ApiError');
    expect(err.message).toBe('msg');
    expect(err.status).toBe(400);
    expect(err.operation).toEqual(op);
  });
});

describe('assertSuccess', () => {
  it('retorna el resultado cuando success es true', () => {
    const op: OperationResult = { success: true, entityId: 'x', message: null, errorCode: null };
    expect(assertSuccess(op)).toEqual(op);
  });

  it('lanza ApiError cuando success es false', () => {
    const op: OperationResult = { success: false, entityId: null, message: 'bad', errorCode: 'E' };
    expect(() => assertSuccess(op)).toThrow(ApiError);
    try {
      assertSuccess(op);
    } catch (e) {
      expect(e).toBeInstanceOf(ApiError);
      if (e instanceof ApiError) {
        expect(e.status).toBe(400);
        expect(e.message).toBe('bad');
      }
    }
  });
});

describe('apiGet / apiPost / apiPut / apiDelete', () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it('apiGet usa fetch con headers y parsea JSON', async () => {
    const fetchMock = vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(JSON.stringify({ ok: true }), {
        status: 200,
        headers: { 'Content-Type': 'application/json' },
      })
    );

    const data = await apiGet<{ ok: boolean }>('/api/foo');

    expect(data).toEqual({ ok: true });
    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0] as [string, RequestInit];
    expect(url).toContain('/api/foo');
    expect(init.headers).toMatchObject({
      'Content-Type': 'application/json',
      'x-api-version': '1.0',
    });
  });

  it('apiGet lanza ApiError en 404', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(JSON.stringify({ success: false, message: 'nf', entityId: null, errorCode: null }), {
        status: 404,
        headers: { 'Content-Type': 'application/json' },
      })
    );

    await expect(apiGet('/api/missing')).rejects.toMatchObject({
      name: 'ApiError',
      status: 404,
    });
  });

  it('apiPost envía JSON y retorna cuerpo parseado', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(JSON.stringify({ a: 1 }), {
        status: 201,
        headers: { 'Content-Type': 'application/json' },
      })
    );

    const body = await apiPost('/api/x', { hello: 'world' });
    expect(body).toEqual({ a: 1 });
    const init = (vi.mocked(fetch).mock.calls[0] as [string, RequestInit])[1];
    expect(init.method).toBe('POST');
    expect(init.body).toBe(JSON.stringify({ hello: 'world' }));
  });

  it('apiPut lanza ApiError en 400', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(JSON.stringify({ success: false, message: 'bad put', entityId: null, errorCode: null }), {
        status: 400,
        headers: { 'Content-Type': 'application/json' },
      })
    );

    await expect(apiPut('/api/x', { x: 1 })).rejects.toMatchObject({
      name: 'ApiError',
      status: 400,
    });
  });

  it('apiDelete retorna OperationResult cuando hay JSON', async () => {
    const op: OperationResult = { success: true, entityId: null, message: null, errorCode: null };
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(JSON.stringify(op), {
        status: 200,
        headers: { 'Content-Type': 'application/json' },
      })
    );

    const result = await apiDelete('/api/x');
    expect(result).toEqual(op);
    const init = (vi.mocked(fetch).mock.calls[0] as [string, RequestInit])[1];
    expect(init.method).toBe('DELETE');
  });
});

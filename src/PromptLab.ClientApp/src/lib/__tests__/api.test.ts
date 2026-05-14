import { http, HttpResponse } from 'msw';
import { afterAll, afterEach, beforeAll, describe, expect, it } from 'vitest';
import { createPrompt, createTag, getPrompt, searchPrompts } from '../api';
import { API_BASE, resetPromptsState } from '../../test/msw/handlers';
import { server } from '../../test/msw/server';

beforeAll(() => {
  server.listen({ onUnhandledRequest: 'error' });
});

afterEach(() => {
  server.resetHandlers();
  resetPromptsState();
});

afterAll(() => {
  server.close();
});

describe('searchPrompts', () => {
  it('construye query string con pageNumber y pageSize por defecto', async () => {
    let capturedUrl = '';
    server.use(
      http.get(`${API_BASE}/api/Prompts`, ({ request }) => {
        capturedUrl = request.url;
        return HttpResponse.json({ items: [], pageNumber: 1, pageSize: 100, totalRows: 0 });
      })
    );

    await searchPrompts({});
    const url = new URL(capturedUrl);
    expect(url.searchParams.get('pageNumber')).toBe('1');
    expect(url.searchParams.get('pageSize')).toBe('100');
  });

  it('incluye query y tagId cuando se pasan', async () => {
    let capturedUrl = '';
    server.use(
      http.get(`${API_BASE}/api/Prompts`, ({ request }) => {
        capturedUrl = request.url;
        return HttpResponse.json({ items: [], pageNumber: 1, pageSize: 12, totalRows: 0 });
      })
    );

    await searchPrompts({ query: 'hola', tagId: 'tag-1', pageNumber: 2, pageSize: 12 });
    const url = new URL(capturedUrl);
    expect(url.searchParams.get('query')).toBe('hola');
    expect(url.searchParams.get('tagId')).toBe('tag-1');
    expect(url.searchParams.get('pageNumber')).toBe('2');
    expect(url.searchParams.get('pageSize')).toBe('12');
  });
});

describe('getPrompt', () => {
  it('retorna null en 404', async () => {
    const result = await getPrompt('no-existe');
    expect(result).toBeNull();
  });

  it('retorna el prompt cuando existe', async () => {
    const p = await getPrompt('prompt-1');
    expect(p?.id).toBe('prompt-1');
    expect(p?.title).toBe('Prompt uno');
  });

  it('lanza en error distinto de 404', async () => {
    server.use(
      http.get(`${API_BASE}/api/Prompts/:id`, () =>
        HttpResponse.json({ success: false, message: 'srv', entityId: null, errorCode: null }, { status: 500 })
      )
    );

    await expect(getPrompt('prompt-1')).rejects.toMatchObject({ name: 'ApiError', status: 500 });
  });
});

describe('createPrompt', () => {
  it('hace POST y luego GET para devolver el prompt creado', async () => {
    const before = (await searchPrompts({ pageSize: 100 })).totalRows;
    const created = await createPrompt({
      title: 'Nuevo',
      description: null,
      content: 'Cuerpo',
      category: null,
      language: null,
      modelHint: null,
      targetModelId: null,
      temperature: 0.2,
      maxTokens: 100,
      topP: 1,
      isActive: true,
      tagIds: ['tag-1'],
    });
    expect(created.title).toBe('Nuevo');
    expect(created.content).toBe('Cuerpo');
    expect(created.tagSummaries?.some((t) => t.id === 'tag-1')).toBe(true);
    const after = (await searchPrompts({ pageSize: 100 })).totalRows;
    expect(after).toBe(before + 1);
  });
});

describe('createTag', () => {
  it('extrae entityId de la respuesta', async () => {
    const id = await createTag('Mi tag');
    expect(id).toMatch(/^tag-new-/);
  });
});

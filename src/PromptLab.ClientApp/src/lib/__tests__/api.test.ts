import { http, HttpResponse } from 'msw';
import { afterAll, afterEach, beforeAll, describe, expect, it } from 'vitest';
import {
  createPrompt,
  createTag,
  createTestCase,
  createTestResult,
  createTestRun,
  createTestSuite,
  deletePrompt,
  deleteTestCase,
  deleteTestSuite,
  getAiModels,
  getAllTestRuns,
  getPrompt,
  getPromptVersions,
  getTags,
  getTestCasesBySuite,
  getTestRunDetail,
  getTestRunsBySuite,
  getTestSuiteDetail,
  getTestSuitesByPrompt,
  searchPrompts,
  setPromptTags,
  updatePrompt,
  updateTestCase,
  updateTestRun,
  updateTestSuite,
} from '../api';
import { API_BASE, resetAllStores } from '../../test/msw/handlers';
import { server } from '../../test/msw/server';

beforeAll(() => {
  server.listen({ onUnhandledRequest: 'error' });
});

afterEach(() => {
  server.resetHandlers();
  resetAllStores();
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

describe('getPromptVersions', () => {
  it('retorna versiones del prompt', async () => {
    const v = await getPromptVersions('prompt-1');
    expect(v.length).toBeGreaterThanOrEqual(1);
    expect(v[0]?.promptId).toBe('prompt-1');
  });
});

describe('updatePrompt', () => {
  it('actualiza y devuelve el prompt', async () => {
    const p = await updatePrompt('prompt-1', {
      title: 'Título nuevo',
      description: null,
      content: 'Nuevo contenido',
      category: null,
      language: null,
      modelHint: null,
      targetModelId: null,
      temperature: 0.5,
      maxTokens: 500,
      topP: 0.9,
      isActive: true,
      tagIds: [],
    });
    expect(p.title).toBe('Título nuevo');
    expect(p.content).toBe('Nuevo contenido');
  });
});

describe('deletePrompt', () => {
  it('elimina el prompt', async () => {
    await deletePrompt('prompt-2');
    expect(await getPrompt('prompt-2')).toBeNull();
  });
});

describe('setPromptTags', () => {
  it('completa sin error', async () => {
    await expect(setPromptTags('prompt-1', ['tag-1'])).resolves.toBeUndefined();
  });
});

describe('getTags', () => {
  it('sin query retorna todos', async () => {
    const t = await getTags();
    expect(t.length).toBeGreaterThanOrEqual(2);
  });

  it('con query filtra', async () => {
    const t = await getTags('alp');
    expect(t.every((x) => x.name.toLowerCase().includes('alp'))).toBe(true);
  });
});

describe('getAiModels', () => {
  it('retorna modelos', async () => {
    const m = await getAiModels();
    expect(m.some((x) => x.id === 'model-1')).toBe(true);
  });
});

describe('TestSuites API', () => {
  it('getTestSuitesByPrompt filtra por promptId', async () => {
    const list = await getTestSuitesByPrompt('prompt-1');
    expect(list.some((s) => s.id === 'suite-1')).toBe(true);
  });

  it('getTestSuiteDetail retorna suite y casos', async () => {
    const d = await getTestSuiteDetail('suite-1');
    expect(d?.suite.name).toBe('Suite A');
    expect(d?.cases.length).toBeGreaterThanOrEqual(1);
  });

  it('getTestSuiteDetail retorna null si no existe', async () => {
    expect(await getTestSuiteDetail('suite-inexistente')).toBeNull();
  });

  it('createTestSuite crea y devuelve la suite', async () => {
    const s = await createTestSuite('prompt-1', 'Nueva suite', null);
    expect(s.name).toBe('Nueva suite');
    const list = await getTestSuitesByPrompt('prompt-1');
    expect(list.some((x) => x.name === 'Nueva suite')).toBe(true);
  });

  it('updateTestSuite actualiza', async () => {
    await updateTestSuite('suite-1', 'Renombrada', 'desc');
    const d = await getTestSuiteDetail('suite-1');
    expect(d?.suite.name).toBe('Renombrada');
    expect(d?.suite.description).toBe('desc');
  });

  it('deleteTestSuite elimina', async () => {
    await deleteTestSuite('suite-1');
    expect(await getTestSuiteDetail('suite-1')).toBeNull();
  });
});

describe('TestCases API', () => {
  it('getTestCasesBySuite', async () => {
    const cases = await getTestCasesBySuite('suite-1');
    expect(cases.some((c) => c.id === 'case-1')).toBe(true);
  });

  it('createTestCase', async () => {
    const c = await createTestCase('suite-1', 'Nuevo caso', { nombre: 'x' }, 'exp');
    expect(c.name).toBe('Nuevo caso');
  });

  it('updateTestCase', async () => {
    const c = await updateTestCase('case-1', 'suite-1', 'Editado', { nombre: 'y' }, null);
    expect(c.name).toBe('Editado');
  });

  it('deleteTestCase', async () => {
    await deleteTestCase('case-1');
    const cases = await getTestCasesBySuite('suite-1');
    expect(cases.find((c) => c.id === 'case-1')).toBeUndefined();
  });
});

describe('TestRuns API', () => {
  it('getAllTestRuns', async () => {
    const r = await getAllTestRuns();
    expect(r.length).toBeGreaterThanOrEqual(4);
  });

  it('getTestRunsBySuite filtra', async () => {
    const r = await getTestRunsBySuite('suite-1');
    expect(r.every((x) => x.suiteId === 'suite-1')).toBe(true);
  });

  it('getTestRunDetail', async () => {
    const d = await getTestRunDetail('run-1');
    expect(d?.run.id).toBe('run-1');
    expect(d?.results.length).toBeGreaterThanOrEqual(1);
  });

  it('getTestRunDetail null si no existe', async () => {
    expect(await getTestRunDetail('run-zzz')).toBeNull();
  });

  it('createTestRun', async () => {
    const run = await createTestRun({
      suiteId: 'suite-1',
      promptId: 'prompt-1',
      promptVersion: 1,
      model: 'm',
      temperature: 0.1,
      maxTokens: null,
      status: 'pending',
    });
    expect(run.status).toBe('pending');
    const d = await getTestRunDetail(run.id);
    expect(d?.results).toEqual([]);
  });

  it('updateTestRun', async () => {
    await updateTestRun('run-1', { status: 'failed', startedAt: null, completedAt: null });
    const d = await getTestRunDetail('run-1');
    expect(d?.run.status).toBe('failed');
  });

  it('createTestResult', async () => {
    await createTestResult('run-1', {
      caseId: 'case-1',
      actualOutput: 'out',
      passed: true,
      score: 99,
      latencyMs: 10,
      error: null,
    });
    const d = await getTestRunDetail('run-1');
    expect(d?.results.some((r) => r.actualOutput === 'out')).toBe(true);
  });
});

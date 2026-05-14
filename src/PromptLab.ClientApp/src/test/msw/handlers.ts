import { http, HttpResponse } from 'msw';
import type {
  AiModel,
  PagedResponse,
  Prompt,
  PromptVersion,
  Tag,
  TestCase,
  TestResult,
  TestRun,
  TestRunDetail,
  TestSuite,
  TestSuiteDetail,
} from '../../types';

/** Debe coincidir con el fallback de `getApiBaseUrl()` en `apiClient.ts`. */
export const API_BASE = 'https://localhost:7106';

const now = new Date().toISOString();

export const defaultTags: Tag[] = [
  { id: 'tag-1', name: 'Alpha', slug: 'alpha', createdAt: now, updatedAt: now },
  { id: 'tag-2', name: 'Beta', slug: 'beta', createdAt: now, updatedAt: now },
];

export const defaultPrompts: Prompt[] = [
  {
    id: 'prompt-1',
    title: 'Prompt uno',
    description: 'Desc',
    content: 'Hola {{nombre}} — plantilla',
    category: null,
    language: null,
    modelHint: null,
    targetModelId: 'model-1',
    temperature: 0.7,
    maxTokens: 1000,
    topP: 1,
    version: 1,
    isActive: true,
    createdAt: now,
    updatedAt: now,
    tags: [],
    tagSummaries: [{ id: 'tag-1', name: 'Alpha', slug: 'alpha' }],
  },
  {
    id: 'prompt-2',
    title: 'Prompt dos',
    description: null,
    content: 'Otro contenido',
    category: null,
    language: null,
    modelHint: null,
    targetModelId: null,
    temperature: 0.5,
    maxTokens: 500,
    topP: null,
    version: 2,
    isActive: true,
    createdAt: now,
    updatedAt: now,
    tags: [],
    tagSummaries: [],
  },
];

export const defaultAiModels: AiModel[] = [
  { id: 'model-1', displayName: 'GPT Test', provider: 'test', enabled: true },
  { id: 'model-2', displayName: 'Disabled', provider: 'x', enabled: false },
];

export const defaultPromptVersions: PromptVersion[] = [
  {
    id: 'pv-1',
    promptId: 'prompt-1',
    content: 'Versión antigua',
    version: 1,
    createdAt: now,
  },
  {
    id: 'pv-2',
    promptId: 'prompt-1',
    content: 'Hola {{nombre}} — plantilla',
    version: 2,
    createdAt: now,
  },
];

export const defaultSuites: TestSuite[] = [
  {
    id: 'suite-1',
    promptId: 'prompt-1',
    name: 'Suite A',
    description: 'Descripción suite',
    isActive: true,
    createdAt: now,
    updatedAt: now,
  },
];

export const defaultCasesBySuite: Record<string, TestCase[]> = {
  'suite-1': [
    {
      id: 'case-1',
      suiteId: 'suite-1',
      name: 'Case uno',
      inputVariables: JSON.stringify({ nombre: 'Mundo' }),
      expectedOutput: 'esperado',
      isActive: true,
      createdAt: now,
      updatedAt: now,
    },
  ],
};

const defaultRun1: TestRun = {
  id: 'run-1',
  suiteId: 'suite-1',
  promptId: 'prompt-1',
  promptVersion: 1,
  model: 'gpt',
  temperature: 0.7,
  maxTokens: 100,
  status: 'completed',
  startedAt: now,
  completedAt: now,
  createdAt: now,
  promptTitle: 'Prompt uno',
  suiteName: 'Suite A',
};

const defaultRun2: TestRun = {
  id: 'run-2',
  suiteId: 'suite-1',
  promptId: 'prompt-1',
  promptVersion: 1,
  model: 'gpt',
  temperature: 0.5,
  maxTokens: null,
  status: 'failed',
  startedAt: now,
  completedAt: now,
  createdAt: now,
  promptTitle: 'Prompt uno',
  suiteName: 'Suite A',
};

const defaultRun3: TestRun = {
  id: 'run-3',
  suiteId: 'suite-1',
  promptId: 'prompt-1',
  promptVersion: 1,
  model: 'gpt',
  temperature: 0.2,
  maxTokens: null,
  status: 'running',
  startedAt: now,
  completedAt: null,
  createdAt: now,
  promptTitle: null,
  suiteName: null,
};

const defaultRun4: TestRun = {
  id: 'run-4',
  suiteId: 'suite-1',
  promptId: 'prompt-1',
  promptVersion: 1,
  model: 'gpt',
  temperature: 0.1,
  maxTokens: null,
  status: 'pending',
  startedAt: null,
  completedAt: null,
  createdAt: now,
  promptTitle: 'Prompt uno',
  suiteName: 'Suite A',
};

export const defaultTestRuns: TestRun[] = [defaultRun1, defaultRun2, defaultRun3, defaultRun4];

const defaultResultWithError: TestResult = {
  id: 'res-err',
  runId: 'run-2',
  caseId: 'case-1',
  actualOutput: 'bad',
  passed: false,
  score: 10,
  latencyMs: 50,
  error: 'Error de aserción',
  createdAt: now,
  caseName: 'Case uno',
  inputVariables: null,
  expectedOutput: null,
};

export const defaultRunDetails: Record<string, TestRunDetail> = {
  'run-1': {
    run: defaultRun1,
    results: [
      {
        id: 'res-1',
        runId: 'run-1',
        caseId: 'case-1',
        actualOutput: 'salida ok',
        passed: true,
        score: 90,
        latencyMs: 100,
        error: null,
        createdAt: now,
        caseName: 'Case uno',
        inputVariables: null,
        expectedOutput: null,
      },
      {
        id: 'res-2',
        runId: 'run-1',
        caseId: 'case-1',
        actualOutput: 'otro',
        passed: false,
        score: 40,
        latencyMs: 200,
        error: null,
        createdAt: now,
        caseName: 'Case uno',
        inputVariables: null,
        expectedOutput: null,
      },
    ],
  },
  'run-2': {
    run: defaultRun2,
    results: [defaultResultWithError],
  },
  'run-3': {
    run: defaultRun3,
    results: [],
  },
  'run-4': {
    run: defaultRun4,
    results: [],
  },
};

/** Copia mutable para mutaciones en tests. */
let promptsState: Prompt[] = [...defaultPrompts];
let promptVersionsState: PromptVersion[] = [...defaultPromptVersions];
let suitesState: TestSuite[] = [...defaultSuites];
let casesBySuite: Record<string, TestCase[]> = JSON.parse(JSON.stringify(defaultCasesBySuite)) as Record<string, TestCase[]>;
let runsState: TestRun[] = [...defaultTestRuns];
let runDetailsState: Record<string, TestRunDetail> = JSON.parse(JSON.stringify(defaultRunDetails)) as Record<string, TestRunDetail>;

export function resetPromptsState() {
  promptsState = defaultPrompts.map((p) => ({ ...p, tagSummaries: p.tagSummaries?.map((t) => ({ ...t })) ?? [] }));
}

export function resetTestStores() {
  promptVersionsState = [...defaultPromptVersions];
  suitesState = [...defaultSuites];
  casesBySuite = JSON.parse(JSON.stringify(defaultCasesBySuite)) as Record<string, TestCase[]>;
  runsState = [...defaultTestRuns];
  runDetailsState = JSON.parse(JSON.stringify(defaultRunDetails)) as Record<string, TestRunDetail>;
}

/** Resetea prompts + datos de suites/runs/casos/versiones (usar en afterEach de tests que mutan MSW). */
export function resetAllStores() {
  resetPromptsState();
  resetTestStores();
}

/** Solo tests: reemplaza el listado de suites (y limpia casos no referenciados). */
export function setSuitesStateForTest(suites: TestSuite[]) {
  suitesState = suites.map((s) => ({ ...s }));
  const next: Record<string, TestCase[]> = {};
  for (const s of suites) {
    next[s.id] = casesBySuite[s.id] ?? [];
  }
  casesBySuite = next;
}

export function getPromptsState(): Prompt[] {
  return promptsState;
}

export function setPromptsStateForTest(next: Prompt[]) {
  promptsState = next.map((p) => ({
    ...p,
    tagSummaries: p.tagSummaries?.map((t) => ({ ...t })) ?? [],
  }));
}

function paged(items: Prompt[], pageNumber: number, pageSize: number): PagedResponse<Prompt> {
  const start = (pageNumber - 1) * pageSize;
  const slice = items.slice(start, start + pageSize);
  return {
    items: slice,
    pageNumber,
    pageSize,
    totalRows: items.length,
  };
}

function tagSummariesForIds(tagIds: string[]) {
  return tagIds
    .map((id) => defaultTags.find((t) => t.id === id))
    .filter((t): t is Tag => !!t)
    .map((t) => ({ id: t.id, name: t.name, slug: t.slug }));
}

function suiteDetail(suiteId: string): TestSuiteDetail | null {
  const suite = suitesState.find((s) => s.id === suiteId);
  if (!suite) return null;
  return { suite, cases: casesBySuite[suiteId] ?? [] };
}

export const handlers = [
  http.get(`${API_BASE}/api/Tags`, ({ request }) => {
    const url = new URL(request.url);
    const q = (url.searchParams.get('query') ?? '').toLowerCase();
    const list = q ? defaultTags.filter((t) => t.name.toLowerCase().includes(q)) : defaultTags;
    return HttpResponse.json(list);
  }),

  http.post(`${API_BASE}/api/Tags`, async ({ request }) => {
    const body = (await request.json()) as { name: string };
    const id = `tag-new-${body.name.replace(/\s+/g, '-')}`;
    return HttpResponse.json({ success: true, entityId: id, message: null, errorCode: null });
  }),

  http.get(`${API_BASE}/api/ai-models`, () => HttpResponse.json(defaultAiModels)),

  http.get(`${API_BASE}/api/Prompts`, ({ request }) => {
    const url = new URL(request.url);
    const pageNumber = Number(url.searchParams.get('pageNumber') ?? '1');
    const pageSize = Number(url.searchParams.get('pageSize') ?? '100');
    const tagId = url.searchParams.get('tagId');
    const query = (url.searchParams.get('query') ?? '').toLowerCase();

    let list = [...promptsState];
    if (tagId) {
      list = list.filter((p) => (p.tagSummaries ?? []).some((t) => t.id === tagId));
    }
    if (query) {
      list = list.filter((p) => p.title.toLowerCase().includes(query) || p.content.toLowerCase().includes(query));
    }
    return HttpResponse.json(paged(list, pageNumber, pageSize));
  }),

  http.get(`${API_BASE}/api/Prompts/:id/versions`, ({ params }) => {
    const id = params.id as string;
    const list = promptVersionsState.filter((v) => v.promptId === id);
    return HttpResponse.json(list);
  }),

  http.put(`${API_BASE}/api/Prompts/:promptId/tags`, async () =>
    HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null })
  ),

  http.get(`${API_BASE}/api/Prompts/:id`, ({ params }) => {
    const id = params.id as string;
    const p = promptsState.find((x) => x.id === id);
    if (!p) {
      return HttpResponse.json(
        { success: false, message: 'Not found', entityId: null, errorCode: '404' },
        { status: 404 }
      );
    }
    return HttpResponse.json(p);
  }),

  http.post(`${API_BASE}/api/Prompts`, async ({ request }) => {
    const body = (await request.json()) as {
      title: string;
      description?: string | null;
      content: string;
      temperature?: number | null;
      maxTokens?: number | null;
      topP?: number | null;
      targetModelId?: string | null;
      tagIds?: string[];
    };
    const id = `prompt-${crypto.randomUUID()}`;
    const tagIds = body.tagIds ?? [];
    const newPrompt: Prompt = {
      id,
      title: body.title,
      description: body.description ?? null,
      content: body.content,
      category: null,
      language: null,
      modelHint: body.targetModelId ?? null,
      targetModelId: body.targetModelId ?? null,
      temperature: body.temperature ?? null,
      maxTokens: body.maxTokens ?? null,
      topP: body.topP ?? null,
      version: 1,
      isActive: true,
      createdAt: now,
      updatedAt: now,
      tags: tagIds,
      tagSummaries: tagSummariesForIds(tagIds),
    };
    promptsState = [...promptsState, newPrompt];
    return HttpResponse.json({ success: true, entityId: id, message: null, errorCode: null });
  }),

  http.put(`${API_BASE}/api/Prompts/:id`, async ({ params, request }) => {
    const id = params.id as string;
    const body = (await request.json()) as {
      title: string;
      description?: string | null;
      content: string;
      temperature?: number | null;
      maxTokens?: number | null;
      topP?: number | null;
      targetModelId?: string | null;
      tagIds?: string[];
    };
    const idx = promptsState.findIndex((p) => p.id === id);
    if (idx >= 0) {
      const prev = promptsState[idx]!;
      const tagIds = body.tagIds ?? [];
      promptsState[idx] = {
        ...prev,
        title: body.title,
        description: body.description ?? null,
        content: body.content,
        temperature: body.temperature ?? null,
        maxTokens: body.maxTokens ?? null,
        topP: body.topP ?? null,
        targetModelId: body.targetModelId ?? null,
        modelHint: body.targetModelId ?? null,
        tags: tagIds,
        tagSummaries: tagSummariesForIds(tagIds),
        version: prev.version + 1,
        updatedAt: new Date().toISOString(),
      };
    }
    return HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null });
  }),

  http.delete(`${API_BASE}/api/Prompts/:id`, ({ params }) => {
    const id = params.id as string;
    promptsState = promptsState.filter((p) => p.id !== id);
    return HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null });
  }),

  http.get(`${API_BASE}/api/TestSuites`, ({ request }) => {
    const url = new URL(request.url);
    const promptId = url.searchParams.get('promptId');
    const list = promptId ? suitesState.filter((s) => s.promptId === promptId) : suitesState;
    return HttpResponse.json(list);
  }),

  http.get(`${API_BASE}/api/TestSuites/:suiteId`, ({ params }) => {
    const suiteId = params.suiteId as string;
    const detail = suiteDetail(suiteId);
    if (!detail) {
      return HttpResponse.json({ success: false, message: 'Not found' }, { status: 404 });
    }
    return HttpResponse.json(detail);
  }),

  http.post(`${API_BASE}/api/TestSuites`, async ({ request }) => {
    const body = (await request.json()) as { promptId: string; name: string; description: string | null };
    const id = `suite-${crypto.randomUUID().slice(0, 8)}`;
    const suite: TestSuite = {
      id,
      promptId: body.promptId,
      name: body.name,
      description: body.description,
      isActive: true,
      createdAt: now,
      updatedAt: now,
    };
    suitesState = [...suitesState, suite];
    casesBySuite[id] = [];
    return HttpResponse.json({ success: true, entityId: id, message: null, errorCode: null });
  }),

  http.put(`${API_BASE}/api/TestSuites/:suiteId`, async ({ params, request }) => {
    const suiteId = params.suiteId as string;
    const body = (await request.json()) as { name: string; description: string | null };
    const idx = suitesState.findIndex((s) => s.id === suiteId);
    if (idx >= 0) {
      suitesState[idx] = { ...suitesState[idx]!, name: body.name, description: body.description, updatedAt: now };
    }
    return HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null });
  }),

  http.delete(`${API_BASE}/api/TestSuites/:suiteId`, ({ params }) => {
    const suiteId = params.suiteId as string;
    suitesState = suitesState.filter((s) => s.id !== suiteId);
    delete casesBySuite[suiteId];
    return HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null });
  }),

  http.get(`${API_BASE}/api/TestCases`, ({ request }) => {
    const url = new URL(request.url);
    const suiteId = url.searchParams.get('suiteId') ?? '';
    return HttpResponse.json(casesBySuite[suiteId] ?? []);
  }),

  http.post(`${API_BASE}/api/TestCases`, async ({ request }) => {
    const body = (await request.json()) as {
      suiteId: string;
      name: string;
      inputVariables: string;
      expectedOutput: string | null;
    };
    const id = `case-${crypto.randomUUID().slice(0, 8)}`;
    const tc: TestCase = {
      id,
      suiteId: body.suiteId,
      name: body.name,
      inputVariables: body.inputVariables,
      expectedOutput: body.expectedOutput,
      isActive: true,
      createdAt: now,
      updatedAt: now,
    };
    const arr = casesBySuite[body.suiteId] ?? [];
    casesBySuite[body.suiteId] = [...arr, tc];
    return HttpResponse.json({ success: true, entityId: id, message: null, errorCode: null });
  }),

  http.put(`${API_BASE}/api/TestCases/:caseId`, async ({ params, request }) => {
    const caseId = params.caseId as string;
    const body = (await request.json()) as { name: string; inputVariables: string; expectedOutput: string | null };
    for (const sid of Object.keys(casesBySuite)) {
      const arr = casesBySuite[sid] ?? [];
      const idx = arr.findIndex((c) => c.id === caseId);
      if (idx >= 0) {
        arr[idx] = { ...arr[idx]!, name: body.name, inputVariables: body.inputVariables, expectedOutput: body.expectedOutput, updatedAt: now };
        casesBySuite[sid] = arr;
        break;
      }
    }
    return HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null });
  }),

  http.delete(`${API_BASE}/api/TestCases/:caseId`, ({ params }) => {
    const caseId = params.caseId as string;
    for (const sid of Object.keys(casesBySuite)) {
      casesBySuite[sid] = (casesBySuite[sid] ?? []).filter((c) => c.id !== caseId);
    }
    return HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null });
  }),

  http.get(`${API_BASE}/api/TestRuns`, ({ request }) => {
    const url = new URL(request.url);
    const suiteId = url.searchParams.get('suiteId');
    const list = suiteId ? runsState.filter((r) => r.suiteId === suiteId) : runsState;
    return HttpResponse.json(list);
  }),

  http.get(`${API_BASE}/api/TestRuns/:runId`, ({ params }) => {
    const runId = params.runId as string;
    const detail = runDetailsState[runId];
    if (!detail) {
      return HttpResponse.json({ success: false, message: 'Not found' }, { status: 404 });
    }
    return HttpResponse.json(detail);
  }),

  http.post(`${API_BASE}/api/TestRuns`, async ({ request }) => {
    const body = (await request.json()) as {
      suiteId: string;
      promptId: string;
      promptVersion: number;
      model: string;
      temperature: number;
      maxTokens?: number | null;
      status: string;
    };
    const id = `run-${crypto.randomUUID().slice(0, 8)}`;
    const run: TestRun = {
      id,
      suiteId: body.suiteId,
      promptId: body.promptId,
      promptVersion: body.promptVersion,
      model: body.model,
      temperature: body.temperature,
      maxTokens: body.maxTokens ?? null,
      status: body.status,
      startedAt: null,
      completedAt: null,
      createdAt: now,
      promptTitle: promptsState.find((p) => p.id === body.promptId)?.title ?? null,
      suiteName: suitesState.find((s) => s.id === body.suiteId)?.name ?? null,
    };
    runsState = [...runsState, run];
    runDetailsState[id] = { run, results: [] };
    return HttpResponse.json({ success: true, entityId: id, message: null, errorCode: null });
  }),

  http.put(`${API_BASE}/api/TestRuns/:runId`, async ({ params, request }) => {
    const runId = params.runId as string;
    const body = (await request.json()) as { status: string; startedAt?: string | null; completedAt?: string | null };
    const detail = runDetailsState[runId];
    if (detail) {
      detail.run = {
        ...detail.run,
        status: body.status,
        startedAt: body.startedAt ?? detail.run.startedAt,
        completedAt: body.completedAt !== undefined ? body.completedAt : detail.run.completedAt,
      };
    }
    const idx = runsState.findIndex((r) => r.id === runId);
    if (idx >= 0) {
      runsState[idx] = {
        ...runsState[idx]!,
        status: body.status,
        startedAt: body.startedAt ?? runsState[idx]!.startedAt,
        completedAt: body.completedAt !== undefined ? body.completedAt : runsState[idx]!.completedAt,
      };
    }
    return HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null });
  }),

  http.post(`${API_BASE}/api/TestRuns/:runId/results`, async ({ params, request }) => {
    const runId = params.runId as string;
    const body = (await request.json()) as {
      caseId: string;
      actualOutput: string;
      passed: boolean;
      score: number;
      latencyMs: number;
      error?: string | null;
    };
    const detail = runDetailsState[runId];
    if (detail) {
      const res: TestResult = {
        id: `res-${crypto.randomUUID().slice(0, 8)}`,
        runId,
        caseId: body.caseId,
        actualOutput: body.actualOutput,
        passed: body.passed,
        score: body.score,
        latencyMs: body.latencyMs,
        error: body.error ?? null,
        createdAt: now,
        caseName: null,
        inputVariables: null,
        expectedOutput: null,
      };
      detail.results = [...detail.results, res];
    }
    return HttpResponse.json({ success: true, entityId: null, message: null, errorCode: null });
  }),
];

import { http, HttpResponse } from 'msw';
import type { AiModel, PagedResponse, Prompt, Tag } from '../../types';

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
    content: 'Contenido del prompt',
    category: null,
    language: null,
    modelHint: null,
    targetModelId: null,
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
];

/** Copia mutable para mutaciones en tests. */
let promptsState: Prompt[] = [...defaultPrompts];

export function resetPromptsState() {
  promptsState = defaultPrompts.map((p) => ({ ...p, tagSummaries: p.tagSummaries?.map((t) => ({ ...t })) ?? [] }));
}

export function getPromptsState(): Prompt[] {
  return promptsState;
}

/** Solo para tests: reemplaza el listado de prompts en memoria. */
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
];

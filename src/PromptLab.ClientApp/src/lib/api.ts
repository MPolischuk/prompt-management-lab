import type {
  AiModel,
  PagedResponse,
  Prompt,
  PromptVersion,
  Tag,
  TestCase,
  TestRun,
  TestRunDetail,
  TestSuite,
  TestSuiteDetail,
} from '../types';
import { ApiError, apiDelete, apiGet, apiPost, apiPut, assertSuccess, type OperationResult } from './apiClient';

const enc = encodeURIComponent;

export async function searchPrompts(params: {
  query?: string;
  tagId?: string;
  pageNumber?: number;
  pageSize?: number;
}): Promise<PagedResponse<Prompt>> {
  const q = new URLSearchParams();
  if (params.query) q.set('query', params.query);
  if (params.tagId) q.set('tagId', params.tagId);
  q.set('pageNumber', String(params.pageNumber ?? 1));
  q.set('pageSize', String(params.pageSize ?? 100));
  return apiGet<PagedResponse<Prompt>>(`/api/Prompts?${q.toString()}`);
}

export async function getPrompt(id: string): Promise<Prompt | null> {
  try {
    return await apiGet<Prompt>(`/api/Prompts/${enc(id)}`);
  } catch (e) {
    if (e instanceof ApiError && e.status === 404) return null;
    throw e;
  }
}

export async function getPromptVersions(promptId: string): Promise<PromptVersion[]> {
  return apiGet<PromptVersion[]>(`/api/Prompts/${enc(promptId)}/versions`);
}

export interface UpsertPromptBody {
  title: string;
  description?: string | null;
  content: string;
  category?: string | null;
  language?: string | null;
  modelHint?: string | null;
  targetModelId?: string | null;
  temperature?: number | null;
  maxTokens?: number | null;
  topP?: number | null;
  isActive: boolean;
  tagIds: string[];
}

export async function createPrompt(body: UpsertPromptBody): Promise<Prompt> {
  const result = (await apiPost('/api/Prompts', {
    title: body.title,
    description: body.description ?? null,
    content: body.content,
    category: body.category ?? null,
    language: body.language ?? null,
    modelHint: body.modelHint ?? null,
    targetModelId: body.targetModelId ?? null,
    temperature: body.temperature ?? null,
    maxTokens: body.maxTokens ?? null,
    topP: body.topP ?? null,
    isActive: body.isActive,
    tagIds: body.tagIds,
  })) as OperationResult;
  assertSuccess(result);
  const id = result.entityId;
  if (!id) throw new Error('Missing entity id');
  const created = await getPrompt(id);
  if (!created) throw new Error('Prompt not found after create');
  return created;
}

export async function updatePrompt(id: string, body: UpsertPromptBody): Promise<Prompt> {
  const result = (await apiPut(`/api/Prompts/${enc(id)}`, {
    title: body.title,
    description: body.description ?? null,
    content: body.content,
    category: body.category ?? null,
    language: body.language ?? null,
    modelHint: body.modelHint ?? null,
    targetModelId: body.targetModelId ?? null,
    temperature: body.temperature ?? null,
    maxTokens: body.maxTokens ?? null,
    topP: body.topP ?? null,
    isActive: body.isActive,
    tagIds: body.tagIds,
  })) as OperationResult;
  assertSuccess(result);
  const updated = await getPrompt(id);
  if (!updated) throw new Error('Prompt not found after update');
  return updated;
}

export async function deletePrompt(id: string): Promise<void> {
  await apiDelete(`/api/Prompts/${enc(id)}`);
}

export async function setPromptTags(promptId: string, tagIds: string[]): Promise<void> {
  await apiPut(`/api/Prompts/${enc(promptId)}/tags`, tagIds);
}

export async function getTags(query?: string): Promise<Tag[]> {
  const q = query ? `?query=${enc(query)}` : '';
  return apiGet<Tag[]>(`/api/Tags${q}`);
}

export async function createTag(name: string): Promise<string> {
  const result = (await apiPost('/api/Tags', { name })) as OperationResult;
  assertSuccess(result);
  if (!result.entityId) throw new Error('Missing tag id');
  return result.entityId;
}

export async function getAiModels(): Promise<AiModel[]> {
  return apiGet<AiModel[]>('/api/ai-models');
}

export async function getTestSuitesByPrompt(promptId: string): Promise<TestSuite[]> {
  return apiGet<TestSuite[]>(`/api/TestSuites?promptId=${enc(promptId)}`);
}

export async function getTestSuiteDetail(id: string): Promise<TestSuiteDetail | null> {
  try {
    return await apiGet<TestSuiteDetail>(`/api/TestSuites/${enc(id)}`);
  } catch {
    return null;
  }
}

export async function createTestSuite(promptId: string, name: string, description?: string | null): Promise<TestSuite> {
  const result = (await apiPost('/api/TestSuites', { promptId, name, description: description ?? null })) as OperationResult;
  assertSuccess(result);
  const id = result.entityId;
  if (!id) throw new Error('Missing suite id');
  const detail = await getTestSuiteDetail(id);
  if (!detail) throw new Error('Suite not found after create');
  return detail.suite;
}

export async function updateTestSuite(id: string, name: string, description?: string | null): Promise<void> {
  const result = (await apiPut(`/api/TestSuites/${enc(id)}`, { name, description: description ?? null })) as OperationResult;
  assertSuccess(result);
}

export async function deleteTestSuite(id: string): Promise<void> {
  await apiDelete(`/api/TestSuites/${enc(id)}`);
}

export async function getTestCasesBySuite(suiteId: string): Promise<TestCase[]> {
  return apiGet<TestCase[]>(`/api/TestCases?suiteId=${enc(suiteId)}`);
}

export async function createTestCase(
  suiteId: string,
  name: string,
  inputVariables: Record<string, string>,
  expectedOutput?: string | null
): Promise<TestCase> {
  const result = (await apiPost('/api/TestCases', {
    suiteId,
    name,
    inputVariables: JSON.stringify(inputVariables),
    expectedOutput: expectedOutput ?? null,
  })) as OperationResult;
  assertSuccess(result);
  const id = result.entityId;
  if (!id) throw new Error('Missing case id');
  const list = await getTestCasesBySuite(suiteId);
  const created = list.find((c) => c.id === id);
  if (!created) throw new Error('Case not found after create');
  return created;
}

export async function updateTestCase(
  id: string,
  suiteId: string,
  name: string,
  inputVariables: Record<string, string>,
  expectedOutput?: string | null
): Promise<TestCase> {
  const result = (await apiPut(`/api/TestCases/${enc(id)}`, {
    name,
    inputVariables: JSON.stringify(inputVariables),
    expectedOutput: expectedOutput ?? null,
  })) as OperationResult;
  assertSuccess(result);
  const list = await getTestCasesBySuite(suiteId);
  const updated = list.find((c) => c.id === id);
  if (!updated) throw new Error('Case not found after update');
  return updated;
}

export async function deleteTestCase(id: string): Promise<void> {
  await apiDelete(`/api/TestCases/${enc(id)}`);
}

export async function getAllTestRuns(): Promise<TestRun[]> {
  return apiGet<TestRun[]>('/api/TestRuns');
}

export async function getTestRunsBySuite(suiteId: string): Promise<TestRun[]> {
  return apiGet<TestRun[]>(`/api/TestRuns?suiteId=${enc(suiteId)}`);
}

export async function getTestRunDetail(id: string): Promise<TestRunDetail | null> {
  try {
    return await apiGet<TestRunDetail>(`/api/TestRuns/${enc(id)}`);
  } catch {
    return null;
  }
}

export async function createTestRun(body: {
  suiteId: string;
  promptId: string;
  promptVersion: number;
  model: string;
  temperature: number;
  status: string;
}): Promise<TestRun> {
  const result = (await apiPost('/api/TestRuns', body)) as OperationResult;
  assertSuccess(result);
  const id = result.entityId;
  if (!id) throw new Error('Missing run id');
  const detail = await getTestRunDetail(id);
  if (!detail) throw new Error('Run not found after create');
  return detail.run;
}

export async function updateTestRun(
  id: string,
  body: { status: string; startedAt?: string | null; completedAt?: string | null }
): Promise<void> {
  const result = (await apiPut(`/api/TestRuns/${enc(id)}`, {
    status: body.status,
    startedAt: body.startedAt ?? null,
    completedAt: body.completedAt ?? null,
  })) as OperationResult;
  assertSuccess(result);
}

export async function createTestResult(
  runId: string,
  body: {
    caseId: string;
    actualOutput: string;
    passed: boolean;
    score: number;
    latencyMs: number;
    error?: string | null;
  }
): Promise<void> {
  const result = (await apiPost(`/api/TestRuns/${enc(runId)}/results`, {
    caseId: body.caseId,
    actualOutput: body.actualOutput,
    passed: body.passed,
    score: body.score,
    latencyMs: body.latencyMs,
    error: body.error ?? null,
  })) as OperationResult;
  assertSuccess(result);
}

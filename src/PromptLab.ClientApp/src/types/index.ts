/** Tipos alineados con la API .NET (JSON camelCase). */

export interface PromptTagSummary {
  id: string;
  name: string;
  slug: string;
}

export interface Prompt {
  id: string;
  title: string;
  description: string | null;
  content: string;
  category: string | null;
  language: string | null;
  modelHint: string | null;
  targetModelId: string | null;
  temperature: number | null;
  maxTokens: number | null;
  topP: number | null;
  version: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  tags: string[];
  tagSummaries: PromptTagSummary[];
}

export interface PromptVersion {
  id: string;
  promptId: string;
  content: string;
  version: number;
  createdAt: string;
}

export interface TestSuite {
  id: string;
  promptId: string;
  name: string;
  description: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface TestCase {
  id: string;
  suiteId: string;
  name: string;
  inputVariables: string;
  expectedOutput: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface TestSuiteDetail {
  suite: TestSuite;
  cases: TestCase[];
}

export interface TestRun {
  id: string;
  suiteId: string;
  promptId: string;
  promptVersion: number;
  model: string;
  temperature: number;
  status: string;
  startedAt: string | null;
  completedAt: string | null;
  createdAt: string;
  promptTitle: string | null;
  suiteName: string | null;
}

export interface TestResult {
  id: string;
  runId: string;
  caseId: string;
  actualOutput: string;
  passed: boolean;
  score: number;
  latencyMs: number;
  error: string | null;
  createdAt: string;
  caseName: string | null;
  inputVariables: string | null;
  expectedOutput: string | null;
}

export interface TestRunDetail {
  run: TestRun;
  results: TestResult[];
}

export interface Tag {
  id: string;
  name: string;
  slug: string;
  createdAt: string;
  updatedAt: string;
}

export interface AiModel {
  id: string;
  displayName: string;
  provider: string;
  enabled: boolean;
}

export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalRows: number;
}

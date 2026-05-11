export const queryKeys = {
  prompts: ['prompts'] as const,
  prompt: (id: string) => ['prompt', id] as const,
  promptVersions: (id: string) => ['promptVersions', id] as const,
  tags: (q?: string) => ['tags', q ?? ''] as const,
  aiModels: ['aiModels'] as const,
  testSuites: (promptId: string) => ['testSuites', promptId] as const,
  testSuiteDetail: (id: string) => ['testSuiteDetail', id] as const,
  testCases: (suiteId: string) => ['testCases', suiteId] as const,
  testRuns: (suiteId?: string) => ['testRuns', suiteId ?? 'all'] as const,
  testRunDetail: (id: string) => ['testRunDetail', id] as const,
};

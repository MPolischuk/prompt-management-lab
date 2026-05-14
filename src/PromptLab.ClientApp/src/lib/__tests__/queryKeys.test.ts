import { describe, expect, it } from 'vitest';
import { queryKeys } from '../queryKeys';

describe('queryKeys', () => {
  it('prompts', () => {
    expect(queryKeys.prompts).toEqual(['prompts']);
  });

  it('prompt', () => {
    expect(queryKeys.prompt('abc')).toEqual(['prompt', 'abc']);
  });

  it('promptVersions', () => {
    expect(queryKeys.promptVersions('abc')).toEqual(['promptVersions', 'abc']);
  });

  it('tags', () => {
    expect(queryKeys.tags()).toEqual(['tags', '']);
    expect(queryKeys.tags('q')).toEqual(['tags', 'q']);
  });

  it('aiModels', () => {
    expect(queryKeys.aiModels).toEqual(['aiModels']);
  });

  it('testSuites', () => {
    expect(queryKeys.testSuites('p1')).toEqual(['testSuites', 'p1']);
  });

  it('testSuiteDetail', () => {
    expect(queryKeys.testSuiteDetail('s1')).toEqual(['testSuiteDetail', 's1']);
  });

  it('testCases', () => {
    expect(queryKeys.testCases('s1')).toEqual(['testCases', 's1']);
  });

  it('testRuns', () => {
    expect(queryKeys.testRuns()).toEqual(['testRuns', 'all']);
    expect(queryKeys.testRuns('suite-x')).toEqual(['testRuns', 'suite-x']);
  });

  it('testRunDetail', () => {
    expect(queryKeys.testRunDetail('r1')).toEqual(['testRunDetail', 'r1']);
  });
});

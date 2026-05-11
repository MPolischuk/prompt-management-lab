import { supabase } from './supabase';
import type { Prompt, PromptVersion, TestSuite, TestCase, TestRun, TestResult } from '../types';

// Prompts
export async function getPrompts(): Promise<Prompt[]> {
  const { data, error } = await supabase
    .from('prompts')
    .select('*')
    .order('updated_at', { ascending: false });
  if (error) throw error;
  return data ?? [];
}

export async function getPrompt(id: string): Promise<Prompt | null> {
  const { data, error } = await supabase
    .from('prompts')
    .select('*')
    .eq('id', id)
    .maybeSingle();
  if (error) throw error;
  return data;
}

export async function createPrompt(prompt: Omit<Prompt, 'id' | 'version' | 'created_at' | 'updated_at'>): Promise<Prompt> {
  const { data, error } = await supabase
    .from('prompts')
    .insert({ ...prompt, version: 1 })
    .select()
    .single();
  if (error) throw error;
  await supabase.from('prompt_versions').insert({
    prompt_id: data.id,
    content: data.content,
    version: 1,
  });
  return data;
}

export async function updatePrompt(id: string, updates: Partial<Omit<Prompt, 'id' | 'created_at'>>): Promise<Prompt> {
  const current = await getPrompt(id);
  if (!current) throw new Error('Prompt not found');

  const newVersion = (updates.content && updates.content !== current.content)
    ? current.version + 1
    : current.version;

  const { data, error } = await supabase
    .from('prompts')
    .update({ ...updates, version: newVersion, updated_at: new Date().toISOString() })
    .eq('id', id)
    .select()
    .single();
  if (error) throw error;

  if (updates.content && updates.content !== current.content) {
    await supabase.from('prompt_versions').insert({
      prompt_id: id,
      content: updates.content,
      version: newVersion,
    });
  }

  return data;
}

export async function deletePrompt(id: string): Promise<void> {
  const { error } = await supabase.from('prompts').delete().eq('id', id);
  if (error) throw error;
}

export async function getPromptVersions(promptId: string): Promise<PromptVersion[]> {
  const { data, error } = await supabase
    .from('prompt_versions')
    .select('*')
    .eq('prompt_id', promptId)
    .order('version', { ascending: false });
  if (error) throw error;
  return data ?? [];
}

// Test Suites
export async function getTestSuites(promptId: string): Promise<TestSuite[]> {
  const { data, error } = await supabase
    .from('test_suites')
    .select('*, test_cases(*)')
    .eq('prompt_id', promptId)
    .order('created_at', { ascending: false });
  if (error) throw error;
  return data ?? [];
}

export async function getTestSuite(id: string): Promise<TestSuite | null> {
  const { data, error } = await supabase
    .from('test_suites')
    .select('*, test_cases(*)')
    .eq('id', id)
    .maybeSingle();
  if (error) throw error;
  return data;
}

export async function createTestSuite(suite: Omit<TestSuite, 'id' | 'created_at' | 'updated_at' | 'test_cases'>): Promise<TestSuite> {
  const { data, error } = await supabase
    .from('test_suites')
    .insert(suite)
    .select()
    .single();
  if (error) throw error;
  return data;
}

export async function updateTestSuite(id: string, updates: Partial<Pick<TestSuite, 'name' | 'description'>>): Promise<TestSuite> {
  const { data, error } = await supabase
    .from('test_suites')
    .update({ ...updates, updated_at: new Date().toISOString() })
    .eq('id', id)
    .select()
    .single();
  if (error) throw error;
  return data;
}

export async function deleteTestSuite(id: string): Promise<void> {
  const { error } = await supabase.from('test_suites').delete().eq('id', id);
  if (error) throw error;
}

// Test Cases
export async function createTestCase(tc: Omit<TestCase, 'id' | 'created_at' | 'updated_at'>): Promise<TestCase> {
  const { data, error } = await supabase
    .from('test_cases')
    .insert(tc)
    .select()
    .single();
  if (error) throw error;
  return data;
}

export async function updateTestCase(id: string, updates: Partial<Omit<TestCase, 'id' | 'suite_id' | 'created_at'>>): Promise<TestCase> {
  const { data, error } = await supabase
    .from('test_cases')
    .update({ ...updates, updated_at: new Date().toISOString() })
    .eq('id', id)
    .select()
    .single();
  if (error) throw error;
  return data;
}

export async function deleteTestCase(id: string): Promise<void> {
  const { error } = await supabase.from('test_cases').delete().eq('id', id);
  if (error) throw error;
}

// Test Runs
export async function getTestRuns(suiteId: string): Promise<TestRun[]> {
  const { data, error } = await supabase
    .from('test_runs')
    .select('*')
    .eq('suite_id', suiteId)
    .order('created_at', { ascending: false });
  if (error) throw error;
  return data ?? [];
}

export async function getTestRun(id: string): Promise<TestRun | null> {
  const { data, error } = await supabase
    .from('test_runs')
    .select('*, test_results(*, test_cases(*))')
    .eq('id', id)
    .maybeSingle();
  if (error) throw error;
  return data;
}

export async function createTestRun(run: Omit<TestRun, 'id' | 'created_at' | 'started_at' | 'completed_at' | 'test_results'>): Promise<TestRun> {
  const { data, error } = await supabase
    .from('test_runs')
    .insert(run)
    .select()
    .single();
  if (error) throw error;
  return data;
}

export async function updateTestRun(id: string, updates: Partial<Omit<TestRun, 'id' | 'created_at' | 'test_results'>>): Promise<TestRun> {
  const { data, error } = await supabase
    .from('test_runs')
    .update(updates)
    .eq('id', id)
    .select()
    .single();
  if (error) throw error;
  return data;
}

export async function createTestResult(result: Omit<TestResult, 'id' | 'created_at' | 'test_cases'>): Promise<TestResult> {
  const { data, error } = await supabase
    .from('test_results')
    .insert(result)
    .select()
    .single();
  if (error) throw error;
  return data;
}

export async function getAllTestRuns(): Promise<(TestRun & { prompts?: Prompt; test_suites?: TestSuite })[]> {
  const { data, error } = await supabase
    .from('test_runs')
    .select('*, prompts(*), test_suites(*)')
    .order('created_at', { ascending: false });
  if (error) throw error;
  return data ?? [];
}

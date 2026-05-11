export interface Prompt {
    id: string;
    name: string;
    description: string;
    content: string;
    tags: string[];
    model: string;
    temperature: number;
    max_tokens: number;
    version: number;
    created_at: string;
    updated_at: string;
  }
  
  export interface PromptVersion {
    id: string;
    prompt_id: string;
    content: string;
    version: number;
    created_at: string;
  }
  
  export interface TestSuite {
    id: string;
    prompt_id: string;
    name: string;
    description: string;
    created_at: string;
    updated_at: string;
    test_cases?: TestCase[];
  }
  
  export interface TestCase {
    id: string;
    suite_id: string;
    name: string;
    input_variables: Record<string, string>;
    expected_output: string;
    created_at: string;
    updated_at: string;
  }
  
  export interface TestRun {
    id: string;
    suite_id: string;
    prompt_id: string;
    prompt_version: number;
    model: string;
    temperature: number;
    status: 'pending' | 'running' | 'completed' | 'failed';
    started_at: string | null;
    completed_at: string | null;
    created_at: string;
    test_results?: TestResult[];
  }
  
  export interface TestResult {
    id: string;
    run_id: string;
    case_id: string;
    actual_output: string;
    passed: boolean;
    score: number;
    latency_ms: number;
    error: string;
    created_at: string;
    test_cases?: TestCase;
  }
  
  export type View =
    | 'prompts'
    | 'prompt-detail'
    | 'prompt-edit'
    | 'prompt-create'
    | 'test-suites'
    | 'test-suite-detail'
    | 'test-runs'
    | 'test-run-detail';
  
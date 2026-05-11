/*
  # Prompt Management and Test Analysis Schema

  ## Overview
  Creates the full schema for a prompt management system with CRUD operations
  and test run analysis capabilities.

  ## New Tables

  1. `prompts`
     - Core prompt storage with versioning support
     - Fields: id, name, description, content, tags, model, temperature, max_tokens, version, created_at, updated_at

  2. `prompt_versions`
     - Immutable history of all prompt versions
     - Fields: id, prompt_id, content, version, created_at

  3. `test_suites`
     - Groups of test cases for a prompt
     - Fields: id, prompt_id, name, description, created_at, updated_at

  4. `test_cases`
     - Individual test cases with input and expected output
     - Fields: id, suite_id, name, input_variables, expected_output, created_at, updated_at

  5. `test_runs`
     - A single execution of a test suite
     - Fields: id, suite_id, prompt_id, prompt_version, model, temperature, status, started_at, completed_at, created_at

  6. `test_results`
     - Results of individual test cases within a run
     - Fields: id, run_id, case_id, actual_output, passed, score, latency_ms, error, created_at

  ## Security
  - RLS enabled on all tables
  - Policies allow public access (no auth required for this app)
*/

-- Prompts table
CREATE TABLE IF NOT EXISTS prompts (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  name text NOT NULL DEFAULT '',
  description text DEFAULT '',
  content text NOT NULL DEFAULT '',
  tags text[] DEFAULT '{}',
  model text DEFAULT 'gpt-4',
  temperature numeric(3,2) DEFAULT 0.7,
  max_tokens integer DEFAULT 1000,
  version integer NOT NULL DEFAULT 1,
  created_at timestamptz DEFAULT now(),
  updated_at timestamptz DEFAULT now()
);

ALTER TABLE prompts ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Public read prompts"
  ON prompts FOR SELECT
  TO anon, authenticated
  USING (true);

CREATE POLICY "Public insert prompts"
  ON prompts FOR INSERT
  TO anon, authenticated
  WITH CHECK (true);

CREATE POLICY "Public update prompts"
  ON prompts FOR UPDATE
  TO anon, authenticated
  USING (true)
  WITH CHECK (true);

CREATE POLICY "Public delete prompts"
  ON prompts FOR DELETE
  TO anon, authenticated
  USING (true);

-- Prompt versions history
CREATE TABLE IF NOT EXISTS prompt_versions (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  prompt_id uuid NOT NULL REFERENCES prompts(id) ON DELETE CASCADE,
  content text NOT NULL DEFAULT '',
  version integer NOT NULL DEFAULT 1,
  created_at timestamptz DEFAULT now()
);

ALTER TABLE prompt_versions ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Public read prompt_versions"
  ON prompt_versions FOR SELECT
  TO anon, authenticated
  USING (true);

CREATE POLICY "Public insert prompt_versions"
  ON prompt_versions FOR INSERT
  TO anon, authenticated
  WITH CHECK (true);

CREATE POLICY "Public delete prompt_versions"
  ON prompt_versions FOR DELETE
  TO anon, authenticated
  USING (true);

-- Test suites
CREATE TABLE IF NOT EXISTS test_suites (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  prompt_id uuid NOT NULL REFERENCES prompts(id) ON DELETE CASCADE,
  name text NOT NULL DEFAULT '',
  description text DEFAULT '',
  created_at timestamptz DEFAULT now(),
  updated_at timestamptz DEFAULT now()
);

ALTER TABLE test_suites ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Public read test_suites"
  ON test_suites FOR SELECT
  TO anon, authenticated
  USING (true);

CREATE POLICY "Public insert test_suites"
  ON test_suites FOR INSERT
  TO anon, authenticated
  WITH CHECK (true);

CREATE POLICY "Public update test_suites"
  ON test_suites FOR UPDATE
  TO anon, authenticated
  USING (true)
  WITH CHECK (true);

CREATE POLICY "Public delete test_suites"
  ON test_suites FOR DELETE
  TO anon, authenticated
  USING (true);

-- Test cases
CREATE TABLE IF NOT EXISTS test_cases (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  suite_id uuid NOT NULL REFERENCES test_suites(id) ON DELETE CASCADE,
  name text NOT NULL DEFAULT '',
  input_variables jsonb DEFAULT '{}',
  expected_output text DEFAULT '',
  created_at timestamptz DEFAULT now(),
  updated_at timestamptz DEFAULT now()
);

ALTER TABLE test_cases ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Public read test_cases"
  ON test_cases FOR SELECT
  TO anon, authenticated
  USING (true);

CREATE POLICY "Public insert test_cases"
  ON test_cases FOR INSERT
  TO anon, authenticated
  WITH CHECK (true);

CREATE POLICY "Public update test_cases"
  ON test_cases FOR UPDATE
  TO anon, authenticated
  USING (true)
  WITH CHECK (true);

CREATE POLICY "Public delete test_cases"
  ON test_cases FOR DELETE
  TO anon, authenticated
  USING (true);

-- Test runs
CREATE TABLE IF NOT EXISTS test_runs (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  suite_id uuid NOT NULL REFERENCES test_suites(id) ON DELETE CASCADE,
  prompt_id uuid NOT NULL REFERENCES prompts(id) ON DELETE CASCADE,
  prompt_version integer NOT NULL DEFAULT 1,
  model text DEFAULT 'gpt-4',
  temperature numeric(3,2) DEFAULT 0.7,
  status text NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'running', 'completed', 'failed')),
  started_at timestamptz,
  completed_at timestamptz,
  created_at timestamptz DEFAULT now()
);

ALTER TABLE test_runs ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Public read test_runs"
  ON test_runs FOR SELECT
  TO anon, authenticated
  USING (true);

CREATE POLICY "Public insert test_runs"
  ON test_runs FOR INSERT
  TO anon, authenticated
  WITH CHECK (true);

CREATE POLICY "Public update test_runs"
  ON test_runs FOR UPDATE
  TO anon, authenticated
  USING (true)
  WITH CHECK (true);

CREATE POLICY "Public delete test_runs"
  ON test_runs FOR DELETE
  TO anon, authenticated
  USING (true);

-- Test results
CREATE TABLE IF NOT EXISTS test_results (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  run_id uuid NOT NULL REFERENCES test_runs(id) ON DELETE CASCADE,
  case_id uuid NOT NULL REFERENCES test_cases(id) ON DELETE CASCADE,
  actual_output text DEFAULT '',
  passed boolean DEFAULT false,
  score numeric(5,2) DEFAULT 0,
  latency_ms integer DEFAULT 0,
  error text DEFAULT '',
  created_at timestamptz DEFAULT now()
);

ALTER TABLE test_results ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Public read test_results"
  ON test_results FOR SELECT
  TO anon, authenticated
  USING (true);

CREATE POLICY "Public insert test_results"
  ON test_results FOR INSERT
  TO anon, authenticated
  WITH CHECK (true);

CREATE POLICY "Public update test_results"
  ON test_results FOR UPDATE
  TO anon, authenticated
  USING (true)
  WITH CHECK (true);

CREATE POLICY "Public delete test_results"
  ON test_results FOR DELETE
  TO anon, authenticated
  USING (true);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_prompt_versions_prompt_id ON prompt_versions(prompt_id);
CREATE INDEX IF NOT EXISTS idx_test_suites_prompt_id ON test_suites(prompt_id);
CREATE INDEX IF NOT EXISTS idx_test_cases_suite_id ON test_cases(suite_id);
CREATE INDEX IF NOT EXISTS idx_test_runs_suite_id ON test_runs(suite_id);
CREATE INDEX IF NOT EXISTS idx_test_runs_prompt_id ON test_runs(prompt_id);
CREATE INDEX IF NOT EXISTS idx_test_results_run_id ON test_results(run_id);
CREATE INDEX IF NOT EXISTS idx_test_results_case_id ON test_results(case_id);

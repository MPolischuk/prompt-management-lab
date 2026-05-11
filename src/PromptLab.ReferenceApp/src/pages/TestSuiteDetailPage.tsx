import { useState, useEffect } from 'react';
import { ArrowLeft, Plus, Trash2, CreditCard as Edit2, Play, ChevronRight, AlertCircle, Check, X } from 'lucide-react';
import { getTestSuite, getPrompt, createTestCase, updateTestCase, deleteTestCase, getTestRuns, createTestRun, updateTestRun, createTestResult } from '../lib/api';
import type { Prompt, TestSuite, TestCase, TestRun, View } from '../types';
import { Modal } from '../components/Modal';
import { Badge } from '../components/Badge';

interface Props {
  suiteId: string;
  onNavigate: (view: View, id?: string) => void;
}

interface CaseForm {
  name: string;
  expected_output: string;
  input_variables: Record<string, string>;
}

function detectVariables(content: string): string[] {
  return Array.from(new Set(Array.from(content.matchAll(/\{\{(\w+)\}\}/g)).map((m) => m[1])));
}

function simulateOutput(content: string, vars: Record<string, string>): string {
  let out = content;
  for (const [k, v] of Object.entries(vars)) {
    out = out.replace(new RegExp(`\\{\\{${k}\\}\\}`, 'g'), v || `[${k}]`);
  }
  return `[Simulated] ${out.slice(0, 200)}...`;
}

export function TestSuiteDetailPage({ suiteId, onNavigate }: Props) {
  const [suite, setSuite] = useState<TestSuite | null>(null);
  const [prompt, setPrompt] = useState<Prompt | null>(null);
  const [runs, setRuns] = useState<TestRun[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCaseModal, setShowCaseModal] = useState(false);
  const [editCase, setEditCase] = useState<TestCase | null>(null);
  const [saving, setSaving] = useState(false);
  const [running, setRunning] = useState(false);
  const [error, setError] = useState('');
  const [variables, setVariables] = useState<string[]>([]);
  const [form, setForm] = useState<CaseForm>({ name: '', expected_output: '', input_variables: {} });

  useEffect(() => {
    loadData();
  }, [suiteId]);

  async function loadData() {
    const s = await getTestSuite(suiteId);
    if (!s) return;
    setSuite(s);
    const [p, r] = await Promise.all([getPrompt(s.prompt_id), getTestRuns(suiteId)]);
    setPrompt(p);
    setRuns(r);
    if (p) setVariables(detectVariables(p.content));
    setLoading(false);
  }

  function openCreateCase() {
    setEditCase(null);
    const initVars: Record<string, string> = {};
    variables.forEach((v) => { initVars[v] = ''; });
    setForm({ name: '', expected_output: '', input_variables: initVars });
    setError('');
    setShowCaseModal(true);
  }

  function openEditCase(tc: TestCase, e: React.MouseEvent) {
    e.stopPropagation();
    setEditCase(tc);
    const vars: Record<string, string> = {};
    variables.forEach((v) => { vars[v] = tc.input_variables[v] ?? ''; });
    setForm({ name: tc.name, expected_output: tc.expected_output, input_variables: vars });
    setError('');
    setShowCaseModal(true);
  }

  async function handleSaveCase() {
    if (!form.name.trim()) { setError('Case name is required'); return; }
    setSaving(true);
    setError('');
    try {
      if (editCase) {
        const updated = await updateTestCase(editCase.id, form);
        setSuite((s) => s ? { ...s, test_cases: s.test_cases?.map((c) => c.id === updated.id ? updated : c) } : s);
      } else {
        const created = await createTestCase({ ...form, suite_id: suiteId });
        setSuite((s) => s ? { ...s, test_cases: [...(s.test_cases ?? []), created] } : s);
      }
      setShowCaseModal(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save');
    } finally {
      setSaving(false);
    }
  }

  async function handleDeleteCase(id: string, e: React.MouseEvent) {
    e.stopPropagation();
    if (!confirm('Delete this test case?')) return;
    await deleteTestCase(id);
    setSuite((s) => s ? { ...s, test_cases: s.test_cases?.filter((c) => c.id !== id) } : s);
  }

  async function runTests() {
    if (!suite || !prompt || !suite.test_cases?.length) return;
    setRunning(true);
    try {
      const run = await createTestRun({
        suite_id: suiteId,
        prompt_id: suite.prompt_id,
        prompt_version: prompt.version,
        model: prompt.model,
        temperature: prompt.temperature,
        status: 'running',
      });

      await updateTestRun(run.id, { started_at: new Date().toISOString(), status: 'running' });

      const results = [];
      for (const tc of suite.test_cases ?? []) {
        const start = Date.now();
        await new Promise((r) => setTimeout(r, 200 + Math.random() * 400));
        const latency = Date.now() - start;
        const actual = simulateOutput(prompt.content, tc.input_variables);
        const passed = tc.expected_output
          ? actual.toLowerCase().includes(tc.expected_output.toLowerCase().slice(0, 20))
          : Math.random() > 0.3;
        const score = passed ? 80 + Math.random() * 20 : Math.random() * 50;

        const result = await createTestResult({
          run_id: run.id,
          case_id: tc.id,
          actual_output: actual,
          passed,
          score: Math.round(score * 100) / 100,
          latency_ms: latency,
          error: '',
        });
        results.push(result);
      }

      await updateTestRun(run.id, { status: 'completed', completed_at: new Date().toISOString() });
      setRuns((prev) => [{ ...run, status: 'completed' }, ...prev]);
      onNavigate('test-run-detail', run.id);
    } catch (err) {
      console.error(err);
    } finally {
      setRunning(false);
    }
  }

  if (loading) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="w-6 h-6 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!suite) {
    return <div className="flex-1 flex items-center justify-center text-gray-400">Suite not found.</div>;
  }

  const cases = suite.test_cases ?? [];

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <button
              onClick={() => onNavigate('test-suites', suite.prompt_id)}
              className="text-gray-400 hover:text-white p-1.5 hover:bg-gray-800 rounded-lg transition-colors"
            >
              <ArrowLeft className="w-4 h-4" />
            </button>
            <div>
              <div className="flex items-center gap-2 text-xs text-gray-500 mb-1">
                <span className="hover:text-gray-300 cursor-pointer" onClick={() => onNavigate('prompts')}>Prompts</span>
                <ChevronRight className="w-3 h-3" />
                <span className="hover:text-gray-300 cursor-pointer" onClick={() => onNavigate('prompt-detail', suite.prompt_id)}>{prompt?.name}</span>
                <ChevronRight className="w-3 h-3" />
                <span className="hover:text-gray-300 cursor-pointer" onClick={() => onNavigate('test-suites', suite.prompt_id)}>Test Suites</span>
                <ChevronRight className="w-3 h-3" />
                <span className="text-gray-300">{suite.name}</span>
              </div>
              <h2 className="text-xl font-semibold text-white">{suite.name}</h2>
              <p className="text-gray-400 text-sm mt-0.5">{suite.description || 'No description'}</p>
            </div>
          </div>
          <div className="flex items-center gap-2">
            <button
              onClick={openCreateCase}
              className="flex items-center gap-2 px-3 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-lg text-sm font-medium transition-colors border border-gray-700"
            >
              <Plus className="w-4 h-4" />
              Add Case
            </button>
            <button
              onClick={runTests}
              disabled={running || cases.length === 0}
              className="flex items-center gap-2 px-4 py-2 bg-emerald-600 hover:bg-emerald-500 disabled:opacity-50 disabled:cursor-not-allowed text-white rounded-lg text-sm font-medium transition-colors"
            >
              {running ? (
                <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              ) : (
                <Play className="w-4 h-4" />
              )}
              {running ? 'Running...' : 'Run Tests'}
            </button>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        <div className="max-w-5xl grid grid-cols-3 gap-6">
          <div className="col-span-2">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-sm font-semibold text-gray-300">Test Cases ({cases.length})</h3>
            </div>
            {cases.length === 0 ? (
              <div className="flex flex-col items-center justify-center h-48 bg-gray-900 rounded-xl border border-gray-800 text-center">
                <p className="text-gray-500 text-sm">No test cases yet</p>
                <button onClick={openCreateCase} className="mt-3 text-blue-400 hover:text-blue-300 text-sm">
                  Add your first test case
                </button>
              </div>
            ) : (
              <div className="space-y-3">
                {cases.map((tc) => (
                  <div key={tc.id} className="bg-gray-900 rounded-xl border border-gray-800 p-4 group">
                    <div className="flex items-start justify-between mb-3">
                      <h4 className="text-white text-sm font-medium">{tc.name}</h4>
                      <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                        <button onClick={(e) => openEditCase(tc, e)} className="p-1 text-gray-500 hover:text-gray-300 hover:bg-gray-800 rounded transition-colors">
                          <Edit2 className="w-3.5 h-3.5" />
                        </button>
                        <button onClick={(e) => handleDeleteCase(tc.id, e)} className="p-1 text-gray-500 hover:text-red-400 hover:bg-gray-800 rounded transition-colors">
                          <Trash2 className="w-3.5 h-3.5" />
                        </button>
                      </div>
                    </div>
                    {Object.keys(tc.input_variables).length > 0 && (
                      <div className="mb-3">
                        <p className="text-xs text-gray-500 mb-1.5">Input Variables</p>
                        <div className="grid grid-cols-2 gap-2">
                          {Object.entries(tc.input_variables).map(([k, v]) => (
                            <div key={k} className="bg-gray-800 rounded-lg px-3 py-2">
                              <p className="text-xs text-gray-500 font-mono mb-0.5">{`{{${k}}}`}</p>
                              <p className="text-xs text-gray-300 truncate">{v || <span className="text-gray-600 italic">empty</span>}</p>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}
                    {tc.expected_output && (
                      <div>
                        <p className="text-xs text-gray-500 mb-1.5">Expected Output</p>
                        <p className="text-xs text-gray-300 bg-gray-800 rounded-lg px-3 py-2 line-clamp-2">{tc.expected_output}</p>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>

          <div>
            <h3 className="text-sm font-semibold text-gray-300 mb-4">Recent Runs ({runs.length})</h3>
            {runs.length === 0 ? (
              <div className="bg-gray-900 rounded-xl border border-gray-800 p-4 text-center">
                <p className="text-gray-500 text-xs">No runs yet</p>
              </div>
            ) : (
              <div className="space-y-2">
                {runs.slice(0, 10).map((run) => (
                  <button
                    key={run.id}
                    onClick={() => onNavigate('test-run-detail', run.id)}
                    className="w-full bg-gray-900 rounded-xl border border-gray-800 p-3 hover:border-gray-700 text-left transition-colors group"
                  >
                    <div className="flex items-center justify-between mb-1.5">
                      <Badge variant={run.status === 'completed' ? 'success' : run.status === 'failed' ? 'error' : run.status === 'running' ? 'warning' : 'neutral'}>
                        {run.status}
                      </Badge>
                      <span className="text-xs text-gray-600">v{run.prompt_version}</span>
                    </div>
                    <p className="text-xs text-gray-500">{new Date(run.created_at).toLocaleString()}</p>
                  </button>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>

      {showCaseModal && (
        <Modal title={editCase ? 'Edit Test Case' : 'New Test Case'} onClose={() => setShowCaseModal(false)} size="lg">
          <div className="space-y-4">
            {error && (
              <div className="flex items-center gap-2 px-3 py-2 bg-red-900/30 border border-red-800/50 rounded-lg text-red-400 text-sm">
                <AlertCircle className="w-4 h-4 flex-shrink-0" />
                {error}
              </div>
            )}
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Name <span className="text-red-400">*</span></label>
              <input
                autoFocus
                type="text"
                value={form.name}
                onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
                placeholder="e.g., Basic greeting test"
                className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 text-sm"
              />
            </div>

            {variables.length > 0 && (
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">Input Variables</label>
                <div className="space-y-2">
                  {variables.map((v) => (
                    <div key={v}>
                      <label className="block text-xs text-gray-500 mb-1 font-mono">{`{{${v}}}`}</label>
                      <input
                        type="text"
                        value={form.input_variables[v] ?? ''}
                        onChange={(e) => setForm((f) => ({ ...f, input_variables: { ...f.input_variables, [v]: e.target.value } }))}
                        placeholder={`Value for ${v}`}
                        className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 text-sm"
                      />
                    </div>
                  ))}
                </div>
              </div>
            )}

            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Expected Output <span className="text-gray-500 font-normal text-xs">(optional)</span></label>
              <textarea
                value={form.expected_output}
                onChange={(e) => setForm((f) => ({ ...f, expected_output: e.target.value }))}
                placeholder="What output do you expect from the model?"
                rows={4}
                className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 text-sm resize-none"
              />
            </div>

            <div className="flex justify-end gap-2 pt-1">
              <button onClick={() => setShowCaseModal(false)} className="px-4 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-lg text-sm transition-colors">Cancel</button>
              <button
                onClick={handleSaveCase}
                disabled={saving}
                className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-500 disabled:opacity-50 text-white rounded-lg text-sm font-medium transition-colors"
              >
                {saving ? <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" /> : <Check className="w-4 h-4" />}
                {editCase ? 'Save' : 'Create'}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}

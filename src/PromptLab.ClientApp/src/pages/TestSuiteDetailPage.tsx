import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Plus, Trash2, Play, ChevronRight, AlertCircle } from 'lucide-react';
import {
  createTestCase,
  createTestResult,
  createTestRun,
  deleteTestCase,
  getPrompt,
  getTestRunsBySuite,
  getTestSuiteDetail,
  updateTestCase,
  updateTestRun,
} from '../lib/api';
import type { Prompt, TestCase, TestRun } from '../types';
import { Modal } from '../components/Modal';
import { Badge } from '../components/Badge';

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

function parseVars(json: string): Record<string, string> {
  try {
    const o = JSON.parse(json) as Record<string, string>;
    return typeof o === 'object' && o ? o : {};
  } catch {
    return {};
  }
}

export function TestSuiteDetailPage() {
  const { suiteId } = useParams();
  const navigate = useNavigate();
  const [suite, setSuite] = useState<{ id: string; promptId: string; name: string; description: string | null } | null>(null);
  const [cases, setCases] = useState<TestCase[]>([]);
  const [prompt, setPrompt] = useState<Prompt | null>(null);
  const [runs, setRuns] = useState<TestRun[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCaseModal, setShowCaseModal] = useState(false);
  const [editCase, setEditCase] = useState<TestCase | null>(null);
  const [saving, setSaving] = useState(false);
  const [running, setRunning] = useState(false);
  const [error, setError] = useState('');
  const [variables, setVariables] = useState<string[]>([]);
  const [form, setForm] = useState({ name: '', expectedOutput: '', input_variables: {} as Record<string, string> });

  const loadData = useCallback(async () => {
    if (!suiteId) return;
    const detail = await getTestSuiteDetail(suiteId);
    if (!detail) {
      setLoading(false);
      return;
    }
    setSuite(detail.suite);
    setCases(detail.cases);
    const p = await getPrompt(detail.suite.promptId);
    setPrompt(p);
    if (p) setVariables(detectVariables(p.content));
    const r = await getTestRunsBySuite(suiteId);
    setRuns(r);
    setLoading(false);
  }, [suiteId]);

  useEffect(() => {
    void loadData();
  }, [loadData]);

  function openCreateCase() {
    setEditCase(null);
    const init: Record<string, string> = {};
    variables.forEach((v) => {
      init[v] = '';
    });
    setForm({ name: '', expectedOutput: '', input_variables: init });
    setError('');
    setShowCaseModal(true);
  }

  function openEditCase(tc: TestCase, e: React.MouseEvent) {
    e.stopPropagation();
    setEditCase(tc);
    const vars: Record<string, string> = {};
    const parsed = parseVars(tc.inputVariables);
    variables.forEach((v) => {
      vars[v] = parsed[v] ?? '';
    });
    setForm({ name: tc.name, expectedOutput: tc.expectedOutput ?? '', input_variables: vars });
    setError('');
    setShowCaseModal(true);
  }

  async function handleSaveCase() {
    if (!suiteId || !form.name.trim()) {
      setError('Nombre obligatorio');
      return;
    }
    setSaving(true);
    setError('');
    try {
      if (editCase) {
        await updateTestCase(editCase.id, suiteId, form.name, form.input_variables, form.expectedOutput || null);
      } else {
        await createTestCase(suiteId, form.name, form.input_variables, form.expectedOutput || null);
      }
      setShowCaseModal(false);
      await loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error');
    } finally {
      setSaving(false);
    }
  }

  async function handleDeleteCase(id: string, e: React.MouseEvent) {
    e.stopPropagation();
    if (!confirm('¿Eliminar caso?')) return;
    await deleteTestCase(id);
    await loadData();
  }

  async function runTests() {
    if (!suite || !prompt || cases.length === 0 || !suiteId) return;
    setRunning(true);
    try {
      const run = await createTestRun({
        suiteId,
        promptId: suite.promptId,
        promptVersion: prompt.version,
        model: prompt.modelHint ?? prompt.targetModelId ?? 'mock',
        temperature: prompt.temperature ?? 0.7,
        status: 'pending',
      });

      const startedAt = new Date().toISOString();
      await updateTestRun(run.id, { status: 'running', startedAt, completedAt: null });

      for (const tc of cases) {
        const start = Date.now();
        await new Promise((r) => setTimeout(r, 200 + Math.random() * 400));
        const latency = Date.now() - start;
        const actual = simulateOutput(prompt.content, parseVars(tc.inputVariables));
        const exp = tc.expectedOutput ?? '';
        const passed = exp ? actual.toLowerCase().includes(exp.toLowerCase().slice(0, Math.min(20, exp.length))) : Math.random() > 0.3;
        const score = passed ? 80 + Math.random() * 20 : Math.random() * 50;

        await createTestResult(run.id, {
          caseId: tc.id,
          actualOutput: actual,
          passed,
          score: Math.round(score * 100) / 100,
          latencyMs: latency,
          error: '',
        });
      }

      await updateTestRun(run.id, { status: 'completed', startedAt, completedAt: new Date().toISOString() });
      navigate(`/test-runs/${run.id}`);
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

  if (!suite || !suiteId) {
    return <div className="flex-1 flex items-center justify-center text-gray-400">Suite no encontrada</div>;
  }

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div className="flex items-center gap-4">
            <Link to={`/prompts/${suite.promptId}/test-suites`} className="text-gray-400 hover:text-white p-1.5 hover:bg-gray-800 rounded-lg">
              <ArrowLeft className="w-4 h-4" />
            </Link>
            <div>
              <div className="flex items-center gap-2 text-xs text-gray-500 mb-1">
                <Link to="/prompts" className="hover:text-gray-300">
                  Prompts
                </Link>
                <ChevronRight className="w-3 h-3" />
                <Link to={`/prompts/${suite.promptId}`} className="hover:text-gray-300">
                  {prompt?.title}
                </Link>
                <ChevronRight className="w-3 h-3" />
                <span className="text-gray-300">{suite.name}</span>
              </div>
              <h2 className="text-xl font-semibold text-white">{suite.name}</h2>
              <p className="text-gray-400 text-sm">{suite.description || 'Sin descripción'}</p>
            </div>
          </div>
          <div className="flex gap-2">
            <button type="button" onClick={openCreateCase} className="flex items-center gap-2 px-3 py-2 bg-gray-800 rounded-lg text-sm border border-gray-700">
              <Plus className="w-4 h-4" />
              Caso
            </button>
            <button
              type="button"
              disabled={running || cases.length === 0}
              onClick={() => void runTests()}
              className="flex items-center gap-2 px-3 py-2 bg-emerald-600 hover:bg-emerald-500 disabled:opacity-40 rounded-lg text-sm"
            >
              <Play className="w-4 h-4" />
              {running ? 'Ejecutando...' : 'Run tests'}
            </button>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        <h3 className="text-sm font-medium text-gray-400 mb-3">Casos ({cases.length})</h3>
        <div className="space-y-2">
          {cases.map((tc) => (
            <div key={tc.id} className="bg-gray-900 border border-gray-800 rounded-lg p-4 flex justify-between gap-4">
              <div>
                <div className="text-white font-medium">{tc.name}</div>
                <pre className="text-xs text-gray-500 mt-1 font-mono">{tc.inputVariables}</pre>
                {tc.expectedOutput && <p className="text-xs text-gray-400 mt-1">Esperado: {tc.expectedOutput}</p>}
              </div>
              <div className="flex gap-2 shrink-0">
                <button type="button" onClick={(e) => openEditCase(tc, e)} className="text-xs text-blue-400">
                  Editar
                </button>
                <button type="button" onClick={(e) => void handleDeleteCase(tc.id, e)} className="text-xs text-red-400">
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            </div>
          ))}
        </div>

        <h3 className="text-sm font-medium text-gray-400 mt-8 mb-3">Últimas ejecuciones</h3>
        <div className="space-y-2">
          {runs.slice(0, 10).map((r) => (
            <Link key={r.id} to={`/test-runs/${r.id}`} className="block bg-gray-900 border border-gray-800 rounded-lg p-3 text-sm hover:border-gray-600">
              <Badge variant={r.status === 'completed' ? 'success' : r.status === 'failed' ? 'error' : 'neutral'}>{r.status}</Badge>
              <span className="ml-2 text-gray-300">{new Date(r.createdAt).toLocaleString()}</span>
            </Link>
          ))}
        </div>
      </div>

      <Modal isOpen={showCaseModal} onClose={() => setShowCaseModal(false)} title={editCase ? 'Editar caso' : 'Nuevo caso'} size="lg">
        {error && (
          <div className="flex items-center gap-2 text-red-400 text-sm mb-3">
            <AlertCircle className="w-4 h-4" />
            {error}
          </div>
        )}
        <div className="space-y-3">
          <input
            placeholder="Nombre del caso"
            value={form.name}
            onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm"
          />
          {variables.map((v) => (
            <div key={v}>
              <label className="text-xs text-gray-500">{`{{${v}}}`}</label>
              <input
                value={form.input_variables[v] ?? ''}
                onChange={(e) =>
                  setForm((f) => ({
                    ...f,
                    input_variables: { ...f.input_variables, [v]: e.target.value },
                  }))
                }
                className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm mt-0.5"
              />
            </div>
          ))}
          <textarea
            placeholder="Salida esperada (opcional)"
            value={form.expectedOutput}
            onChange={(e) => setForm((f) => ({ ...f, expectedOutput: e.target.value }))}
            className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm"
            rows={3}
          />
          <button type="button" disabled={saving} onClick={() => void handleSaveCase()} className="w-full py-2 bg-blue-600 rounded-lg text-sm font-medium disabled:opacity-50">
            {saving ? 'Guardando...' : 'Guardar'}
          </button>
        </div>
      </Modal>
    </div>
  );
}

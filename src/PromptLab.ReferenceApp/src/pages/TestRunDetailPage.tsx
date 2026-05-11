import { useState, useEffect } from 'react';
import { ArrowLeft, ChevronRight, CheckCircle, XCircle, Clock, Zap, BarChart2, TrendingUp } from 'lucide-react';
import { getTestRun, getPrompt, getTestSuite } from '../lib/api';
import type { TestRun, TestResult, Prompt, TestSuite, View } from '../types';
import { Badge } from '../components/Badge';

interface Props {
  runId: string;
  onNavigate: (view: View, id?: string) => void;
}

export function TestRunDetailPage({ runId, onNavigate }: Props) {
  const [run, setRun] = useState<TestRun | null>(null);
  const [prompt, setPrompt] = useState<Prompt | null>(null);
  const [suite, setSuite] = useState<TestSuite | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedResult, setSelectedResult] = useState<TestResult | null>(null);

  useEffect(() => {
    getTestRun(runId).then(async (r) => {
      if (!r) { setLoading(false); return; }
      setRun(r);
      const [p, s] = await Promise.all([getPrompt(r.prompt_id), getTestSuite(r.suite_id)]);
      setPrompt(p);
      setSuite(s);
      setLoading(false);
      if (r.test_results?.length) setSelectedResult(r.test_results[0]);
    });
  }, [runId]);

  if (loading) {
    return <div className="flex-1 flex items-center justify-center"><div className="w-6 h-6 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" /></div>;
  }

  if (!run) {
    return <div className="flex-1 flex items-center justify-center text-gray-400">Run not found.</div>;
  }

  const results = run.test_results ?? [];
  const passed = results.filter((r) => r.passed).length;
  const failed = results.length - passed;
  const passRate = results.length ? Math.round((passed / results.length) * 100) : 0;
  const avgScore = results.length ? Math.round(results.reduce((s, r) => s + r.score, 0) / results.length * 10) / 10 : 0;
  const avgLatency = results.length ? Math.round(results.reduce((s, r) => s + r.latency_ms, 0) / results.length) : 0;
  const duration = run.started_at && run.completed_at
    ? Math.round((new Date(run.completed_at).getTime() - new Date(run.started_at).getTime()) / 1000)
    : null;

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center gap-4">
          <button
            onClick={() => suite ? onNavigate('test-suite-detail', run.suite_id) : onNavigate('test-runs')}
            className="text-gray-400 hover:text-white p-1.5 hover:bg-gray-800 rounded-lg transition-colors"
          >
            <ArrowLeft className="w-4 h-4" />
          </button>
          <div>
            <div className="flex items-center gap-2 text-xs text-gray-500 mb-1">
              <span className="hover:text-gray-300 cursor-pointer" onClick={() => onNavigate('prompts')}>Prompts</span>
              <ChevronRight className="w-3 h-3" />
              {prompt && <><span className="hover:text-gray-300 cursor-pointer" onClick={() => onNavigate('prompt-detail', run.prompt_id)}>{prompt.name}</span><ChevronRight className="w-3 h-3" /></>}
              {suite && <><span className="hover:text-gray-300 cursor-pointer" onClick={() => onNavigate('test-suites', run.prompt_id)}>{suite.name}</span><ChevronRight className="w-3 h-3" /></>}
              <span className="text-gray-300">Run Analysis</span>
            </div>
            <div className="flex items-center gap-3">
              <h2 className="text-xl font-semibold text-white">Test Run Analysis</h2>
              <Badge variant={run.status === 'completed' ? 'success' : run.status === 'failed' ? 'error' : 'warning'}>
                {run.status}
              </Badge>
            </div>
            <p className="text-gray-400 text-sm mt-0.5">
              {prompt?.name} — v{run.prompt_version} — {run.model}
            </p>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        <div className="max-w-6xl space-y-6">
          {/* Stats */}
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
              <div className="flex items-center gap-2 mb-3">
                <TrendingUp className="w-4 h-4 text-blue-400" />
                <span className="text-xs text-gray-400 font-medium">Pass Rate</span>
              </div>
              <div className="flex items-end gap-2">
                <span className="text-3xl font-bold text-white">{passRate}%</span>
              </div>
              <div className="mt-3 bg-gray-800 rounded-full h-1.5">
                <div
                  className={`h-1.5 rounded-full transition-all ${passRate >= 80 ? 'bg-emerald-500' : passRate >= 50 ? 'bg-amber-500' : 'bg-red-500'}`}
                  style={{ width: `${passRate}%` }}
                />
              </div>
              <p className="text-xs text-gray-500 mt-2">{passed} passed / {failed} failed</p>
            </div>

            <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
              <div className="flex items-center gap-2 mb-3">
                <BarChart2 className="w-4 h-4 text-amber-400" />
                <span className="text-xs text-gray-400 font-medium">Avg Score</span>
              </div>
              <span className="text-3xl font-bold text-white">{avgScore}</span>
              <p className="text-xs text-gray-500 mt-1">out of 100</p>
              <div className="mt-3 bg-gray-800 rounded-full h-1.5">
                <div
                  className="h-1.5 rounded-full bg-amber-500 transition-all"
                  style={{ width: `${avgScore}%` }}
                />
              </div>
            </div>

            <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
              <div className="flex items-center gap-2 mb-3">
                <Zap className="w-4 h-4 text-cyan-400" />
                <span className="text-xs text-gray-400 font-medium">Avg Latency</span>
              </div>
              <span className="text-3xl font-bold text-white">{avgLatency}</span>
              <span className="text-gray-500 text-sm ml-1">ms</span>
              <p className="text-xs text-gray-500 mt-2">per test case</p>
            </div>

            <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
              <div className="flex items-center gap-2 mb-3">
                <Clock className="w-4 h-4 text-gray-400" />
                <span className="text-xs text-gray-400 font-medium">Total Time</span>
              </div>
              <span className="text-3xl font-bold text-white">{duration ?? '—'}</span>
              {duration !== null && <span className="text-gray-500 text-sm ml-1">s</span>}
              <p className="text-xs text-gray-500 mt-2">{results.length} test cases</p>
            </div>
          </div>

          {/* Results */}
          <div className="grid grid-cols-5 gap-4">
            <div className="col-span-2 bg-gray-900 rounded-xl border border-gray-800 overflow-hidden">
              <div className="px-4 py-3 border-b border-gray-800">
                <h3 className="text-sm font-medium text-gray-300">Test Cases</h3>
              </div>
              <div className="divide-y divide-gray-800 overflow-y-auto max-h-[400px]">
                {results.map((result) => {
                  const tc = result.test_cases;
                  const isSelected = selectedResult?.id === result.id;
                  return (
                    <button
                      key={result.id}
                      onClick={() => setSelectedResult(result)}
                      className={`w-full flex items-center gap-3 px-4 py-3 text-left transition-colors ${isSelected ? 'bg-gray-800' : 'hover:bg-gray-800/50'}`}
                    >
                      {result.passed ? (
                        <CheckCircle className="w-4 h-4 text-emerald-400 flex-shrink-0" />
                      ) : (
                        <XCircle className="w-4 h-4 text-red-400 flex-shrink-0" />
                      )}
                      <div className="flex-1 min-w-0">
                        <p className="text-sm text-white truncate">{tc?.name ?? 'Unknown'}</p>
                        <p className="text-xs text-gray-500">{result.latency_ms}ms · {result.score.toFixed(0)}/100</p>
                      </div>
                    </button>
                  );
                })}
              </div>
            </div>

            <div className="col-span-3 bg-gray-900 rounded-xl border border-gray-800 overflow-hidden">
              <div className="px-4 py-3 border-b border-gray-800">
                <h3 className="text-sm font-medium text-gray-300">Result Detail</h3>
              </div>
              {selectedResult ? (
                <div className="p-4 space-y-4 overflow-y-auto max-h-[400px]">
                  <div className="flex items-center gap-3">
                    {selectedResult.passed ? (
                      <Badge variant="success">Passed</Badge>
                    ) : (
                      <Badge variant="error">Failed</Badge>
                    )}
                    <span className="text-xs text-gray-500">Score: {selectedResult.score.toFixed(1)}/100</span>
                    <span className="text-xs text-gray-500">Latency: {selectedResult.latency_ms}ms</span>
                  </div>

                  {Object.keys(selectedResult.test_cases?.input_variables ?? {}).length > 0 && (
                    <div>
                      <p className="text-xs font-medium text-gray-400 mb-2">Inputs</p>
                      <div className="space-y-1.5">
                        {Object.entries(selectedResult.test_cases?.input_variables ?? {}).map(([k, v]) => (
                          <div key={k} className="bg-gray-800 rounded px-3 py-2">
                            <span className="text-xs text-gray-500 font-mono">{`{{${k}}}`}: </span>
                            <span className="text-xs text-gray-300">{v}</span>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}

                  {selectedResult.test_cases?.expected_output && (
                    <div>
                      <p className="text-xs font-medium text-gray-400 mb-2">Expected Output</p>
                      <div className="bg-gray-800 rounded-lg px-3 py-2.5 text-xs text-gray-300">
                        {selectedResult.test_cases.expected_output}
                      </div>
                    </div>
                  )}

                  <div>
                    <p className="text-xs font-medium text-gray-400 mb-2">Actual Output</p>
                    <div className="bg-gray-800 rounded-lg px-3 py-2.5 text-xs text-gray-300 whitespace-pre-wrap">
                      {selectedResult.actual_output}
                    </div>
                  </div>

                  {selectedResult.error && (
                    <div>
                      <p className="text-xs font-medium text-red-400 mb-2">Error</p>
                      <div className="bg-red-900/20 border border-red-800/40 rounded-lg px-3 py-2.5 text-xs text-red-300">
                        {selectedResult.error}
                      </div>
                    </div>
                  )}
                </div>
              ) : (
                <div className="flex items-center justify-center h-32 text-gray-600 text-sm">
                  Select a test case to view details
                </div>
              )}
            </div>
          </div>

          {/* Score distribution */}
          {results.length > 0 && (
            <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
              <h3 className="text-sm font-medium text-gray-300 mb-4">Score Distribution</h3>
              <div className="space-y-2">
                {results.map((result) => {
                  const tc = result.test_cases;
                  return (
                    <div key={result.id} className="flex items-center gap-3">
                      <div className="w-36 text-xs text-gray-400 truncate">{tc?.name ?? 'Unknown'}</div>
                      <div className="flex-1 bg-gray-800 rounded-full h-2">
                        <div
                          className={`h-2 rounded-full transition-all ${result.passed ? 'bg-emerald-500' : 'bg-red-500'}`}
                          style={{ width: `${result.score}%` }}
                        />
                      </div>
                      <div className="w-12 text-xs text-gray-400 text-right tabular-nums">
                        {result.score.toFixed(0)}
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

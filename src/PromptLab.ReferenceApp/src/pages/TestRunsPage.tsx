import { useState, useEffect } from 'react';
import { BarChart3, CheckCircle, XCircle, Clock, TrendingUp, Zap } from 'lucide-react';
import { getAllTestRuns } from '../lib/api';
import type { TestRun, Prompt, TestSuite, View } from '../types';
import { Badge } from '../components/Badge';

interface ExtendedRun extends TestRun {
  prompts?: Prompt;
  test_suites?: TestSuite;
}

interface Props {
  onNavigate: (view: View, id?: string) => void;
}

export function TestRunsPage({ onNavigate }: Props) {
  const [runs, setRuns] = useState<ExtendedRun[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState('');

  useEffect(() => {
    getAllTestRuns().then((data) => {
      setRuns(data as ExtendedRun[]);
      setLoading(false);
    });
  }, []);

  const filtered = runs.filter((r) => !statusFilter || r.status === statusFilter);
  const completed = runs.filter((r) => r.status === 'completed');
  const failed = runs.filter((r) => r.status === 'failed');

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-xl font-semibold text-white">Test Analysis</h2>
            <p className="text-gray-400 text-sm mt-0.5">{runs.length} total run{runs.length !== 1 ? 's' : ''} across all prompts</p>
          </div>
        </div>

        <div className="flex items-center gap-2 mt-4">
          {['', 'completed', 'failed', 'running', 'pending'].map((s) => (
            <button
              key={s}
              onClick={() => setStatusFilter(s)}
              className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${statusFilter === s ? 'bg-blue-600 text-white' : 'bg-gray-800 text-gray-400 hover:text-gray-200'}`}
            >
              {s || 'All'} {s ? `(${runs.filter((r) => r.status === s).length})` : `(${runs.length})`}
            </button>
          ))}
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        {loading ? (
          <div className="space-y-3">
            {[...Array(4)].map((_, i) => <div key={i} className="h-20 bg-gray-900 rounded-xl border border-gray-800 animate-pulse" />)}
          </div>
        ) : (
          <div className="max-w-5xl space-y-6">
            {/* Summary cards */}
            <div className="grid grid-cols-4 gap-4">
              <div className="bg-gray-900 rounded-xl border border-gray-800 p-4">
                <div className="flex items-center gap-2 mb-2">
                  <BarChart3 className="w-4 h-4 text-blue-400" />
                  <span className="text-xs text-gray-400">Total Runs</span>
                </div>
                <span className="text-2xl font-bold text-white">{runs.length}</span>
              </div>
              <div className="bg-gray-900 rounded-xl border border-gray-800 p-4">
                <div className="flex items-center gap-2 mb-2">
                  <CheckCircle className="w-4 h-4 text-emerald-400" />
                  <span className="text-xs text-gray-400">Completed</span>
                </div>
                <span className="text-2xl font-bold text-white">{completed.length}</span>
              </div>
              <div className="bg-gray-900 rounded-xl border border-gray-800 p-4">
                <div className="flex items-center gap-2 mb-2">
                  <XCircle className="w-4 h-4 text-red-400" />
                  <span className="text-xs text-gray-400">Failed</span>
                </div>
                <span className="text-2xl font-bold text-white">{failed.length}</span>
              </div>
              <div className="bg-gray-900 rounded-xl border border-gray-800 p-4">
                <div className="flex items-center gap-2 mb-2">
                  <TrendingUp className="w-4 h-4 text-amber-400" />
                  <span className="text-xs text-gray-400">Success Rate</span>
                </div>
                <span className="text-2xl font-bold text-white">
                  {runs.length ? Math.round((completed.length / runs.length) * 100) : 0}%
                </span>
              </div>
            </div>

            {/* Run list */}
            {filtered.length === 0 ? (
              <div className="flex flex-col items-center justify-center h-48 text-center">
                <div className="w-12 h-12 rounded-xl bg-gray-800 flex items-center justify-center mb-4">
                  <BarChart3 className="w-6 h-6 text-gray-600" />
                </div>
                <p className="text-gray-400 font-medium">No test runs yet</p>
                <p className="text-gray-600 text-sm mt-1">Go to a test suite and run your tests to see analysis here.</p>
              </div>
            ) : (
              <div className="space-y-2">
                {filtered.map((run) => (
                  <button
                    key={run.id}
                    onClick={() => onNavigate('test-run-detail', run.id)}
                    className="w-full bg-gray-900 rounded-xl border border-gray-800 p-4 hover:border-gray-700 text-left transition-all group"
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-4">
                        <div>
                          <div className="flex items-center gap-2 mb-1">
                            <span className="text-sm font-medium text-white">
                              {run.prompts?.name ?? 'Unknown Prompt'}
                            </span>
                            <Badge variant="neutral">v{run.prompt_version}</Badge>
                            <Badge variant={run.status === 'completed' ? 'success' : run.status === 'failed' ? 'error' : run.status === 'running' ? 'warning' : 'neutral'}>
                              {run.status}
                            </Badge>
                          </div>
                          <div className="flex items-center gap-3 text-xs text-gray-500">
                            <span>{run.test_suites?.name ?? 'Unknown Suite'}</span>
                            <span>·</span>
                            <span>{run.model}</span>
                            <span>·</span>
                            <span className="flex items-center gap-1">
                              <Clock className="w-3 h-3" />
                              {new Date(run.created_at).toLocaleString()}
                            </span>
                          </div>
                        </div>
                      </div>
                      <div className="text-xs text-blue-400 opacity-0 group-hover:opacity-100 transition-opacity">
                        View analysis →
                      </div>
                    </div>
                  </button>
                ))}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}

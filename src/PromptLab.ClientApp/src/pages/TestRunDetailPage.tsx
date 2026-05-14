import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ArrowLeft, ChevronRight } from 'lucide-react';
import { getTestRunDetail } from '../lib/api';
import type { TestRunDetail as TRDetail } from '../types';
import { Badge } from '../components/Badge';

export function TestRunDetailPage() {
  const { runId } = useParams();
  const [detail, setDetail] = useState<TRDetail | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!runId) return;
    getTestRunDetail(runId).then((d) => {
      setDetail(d);
      setLoading(false);
    });
  }, [runId]);

  if (loading) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="w-6 h-6 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!detail) {
    return <div className="flex-1 flex items-center justify-center text-gray-400">Run no encontrado</div>;
  }

  const { run, results } = detail;
  const passed = results.filter((r) => r.passed).length;
  const passRate = results.length ? Math.round((passed / results.length) * 100) : 0;
  const avgLatency = results.length ? Math.round(results.reduce((a, r) => a + r.latencyMs, 0) / results.length) : 0;
  const avgScore = results.length ? results.reduce((a, r) => a + Number(r.score), 0) / results.length : 0;

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center gap-4">
          <Link to="/test-runs" className="text-gray-400 hover:text-white p-1.5 hover:bg-gray-800 rounded-lg">
            <ArrowLeft className="w-4 h-4" />
          </Link>
          <div>
            <div className="flex items-center gap-2 text-xs text-gray-500 mb-1">
              <Link to="/test-runs" className="hover:text-gray-300">
                Test Runs
              </Link>
              <ChevronRight className="w-3 h-3" />
              <span className="text-gray-300">Detalle</span>
            </div>
            <h2 className="text-xl font-semibold text-white">{run.promptTitle ?? 'Run'}</h2>
            <p className="text-gray-400 text-sm">{run.suiteName}</p>
          </div>
          <div className="ml-auto">
            <Badge variant="neutral">{run.status}</Badge>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6 space-y-6">
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-7 gap-4">
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-4">
            <div className="text-xs text-gray-500">Pass rate</div>
            <div className="text-xl font-semibold text-white">{passRate}%</div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-4">
            <div className="text-xs text-gray-500">Latencia media</div>
            <div className="text-xl font-semibold text-white">{avgLatency} ms</div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-4">
            <div className="text-xs text-gray-500">Score medio</div>
            <div className="text-xl font-semibold text-white">{avgScore.toFixed(1)}</div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-4">
            <div className="text-xs text-gray-500">Casos</div>
            <div className="text-xl font-semibold text-white">{results.length}</div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-4">
            <div className="text-xs text-gray-500">Modelo</div>
            <div className="text-sm font-medium text-white font-mono truncate" title={run.model}>
              {run.model}
            </div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-4">
            <div className="text-xs text-gray-500">Temperatura</div>
            <div className="text-xl font-semibold text-white">{run.temperature}</div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-4">
            <div className="text-xs text-gray-500">Max tokens</div>
            <div className="text-xl font-semibold text-white">{run.maxTokens != null ? run.maxTokens : '—'}</div>
          </div>
        </div>

        <div className="space-y-3">
          <h3 className="text-sm font-medium text-gray-400">Resultados</h3>
          {results.map((res) => (
            <div key={res.id} className="bg-gray-900 border border-gray-800 rounded-lg p-4">
              <div className="flex items-center justify-between mb-2">
                <span className="text-white font-medium">{res.caseName ?? res.caseId}</span>
                <Badge variant={res.passed ? 'success' : 'error'}>{res.passed ? 'PASS' : 'FAIL'}</Badge>
              </div>
              <p className="text-xs text-gray-500">Score: {res.score} · {res.latencyMs} ms</p>
              {res.error && <p className="text-xs text-red-400 mt-1">{res.error}</p>}
              <pre className="text-xs text-gray-400 mt-2 font-mono whitespace-pre-wrap max-h-32 overflow-y-auto">{res.actualOutput}</pre>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

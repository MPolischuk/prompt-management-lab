import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { BarChart3 } from 'lucide-react';
import { getAllTestRuns } from '../lib/api';
import { queryKeys } from '../lib/queryKeys';
import { Badge } from '../components/Badge';

export function TestRunsPage() {
  const [statusFilter, setStatusFilter] = useState('');
  const { data: runs = [], isLoading } = useQuery({
    queryKey: queryKeys.testRuns(),
    queryFn: getAllTestRuns,
  });

  const filtered = useMemo(
    () => (statusFilter ? runs.filter((r) => r.status === statusFilter) : runs),
    [runs, statusFilter]
  );

  const stats = useMemo(() => {
    const total = runs.length;
    const completed = runs.filter((r) => r.status === 'completed').length;
    const failed = runs.filter((r) => r.status === 'failed').length;
    const rate = total ? Math.round((completed / total) * 100) : 0;
    return { total, completed, failed, rate };
  }, [runs]);

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <h2 className="text-xl font-semibold text-white flex items-center gap-2">
          <BarChart3 className="w-5 h-5" />
          Test Runs
        </h2>
        <div className="grid grid-cols-4 gap-4 mt-4 max-w-2xl">
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-3 text-center">
            <div className="text-2xl font-semibold text-white">{stats.total}</div>
            <div className="text-xs text-gray-500">Total</div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-3 text-center">
            <div className="text-2xl font-semibold text-emerald-400">{stats.completed}</div>
            <div className="text-xs text-gray-500">Completados</div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-3 text-center">
            <div className="text-2xl font-semibold text-red-400">{stats.failed}</div>
            <div className="text-xs text-gray-500">Fallidos</div>
          </div>
          <div className="bg-gray-900 border border-gray-800 rounded-lg p-3 text-center">
            <div className="text-2xl font-semibold text-blue-400">{stats.rate}%</div>
            <div className="text-xs text-gray-500">Éxito</div>
          </div>
        </div>
        <div className="flex gap-2 mt-4">
          {['', 'completed', 'failed', 'running', 'pending'].map((s) => (
            <button
              key={s || 'all'}
              type="button"
              onClick={() => setStatusFilter(s)}
              className={`px-3 py-1.5 rounded-lg text-xs font-medium ${
                statusFilter === s ? 'bg-blue-600 text-white' : 'bg-gray-800 text-gray-400'
              }`}
            >
              {s || 'Todos'}
            </button>
          ))}
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        {isLoading ? (
          <div className="flex justify-center py-12">
            <div className="w-6 h-6 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : (
          <div className="space-y-2">
            {filtered.map((r) => (
              <Link
                key={r.id}
                to={`/test-runs/${r.id}`}
                className="block bg-gray-900 border border-gray-800 rounded-lg p-4 hover:border-gray-600"
              >
                <div className="flex items-center justify-between flex-wrap gap-2">
                  <div>
                    <span className="text-white font-medium">{r.promptTitle ?? r.promptId}</span>
                    <span className="text-gray-500 mx-2">/</span>
                    <span className="text-gray-300">{r.suiteName ?? r.suiteId}</span>
                  </div>
                  <Badge variant={r.status === 'completed' ? 'success' : r.status === 'failed' ? 'error' : 'neutral'}>{r.status}</Badge>
                </div>
                <div className="text-xs text-gray-500 mt-2">{r.model} · {new Date(r.createdAt).toLocaleString()}</div>
              </Link>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

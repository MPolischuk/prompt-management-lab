import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import {
  ArrowLeft,
  CreditCard as Edit2,
  Trash2,
  Tag,
  Clock,
  FlaskConical,
  History,
  Copy,
  Check,
  ChevronDown,
  ChevronUp,
} from 'lucide-react';
import { deletePrompt, getPrompt, getPromptVersions } from '../lib/api';
import type { Prompt, PromptVersion } from '../types';
import { Badge } from '../components/Badge';

export function PromptDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const qc = useQueryClient();
  const [prompt, setPrompt] = useState<Prompt | null>(null);
  const [versions, setVersions] = useState<PromptVersion[]>([]);
  const [loading, setLoading] = useState(true);
  const [copied, setCopied] = useState(false);
  const [showVersions, setShowVersions] = useState(false);

  useEffect(() => {
    if (!id) return;
    Promise.all([getPrompt(id), getPromptVersions(id)]).then(([p, v]) => {
      setPrompt(p);
      setVersions(v);
      setLoading(false);
    });
  }, [id]);

  async function handleDelete() {
    if (!id) return;
    if (!confirm('¿Eliminar este prompt?')) return;
    await deletePrompt(id);
    await qc.invalidateQueries({ queryKey: ['prompts'] });
    navigate('/prompts');
  }

  function copyContent() {
    if (!prompt) return;
    void navigator.clipboard.writeText(prompt.content);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  if (loading) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="w-6 h-6 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!prompt || !id) {
    return <div className="flex-1 flex items-center justify-center text-gray-400">No encontrado</div>;
  }

  const variables = Array.from(prompt.content.matchAll(/\{\{(\w+)\}\}/g)).map((m) => m[1]);
  const uniqueVars = Array.from(new Set(variables));

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div className="flex items-center gap-4">
            <Link to="/prompts" className="text-gray-400 hover:text-white p-1.5 hover:bg-gray-800 rounded-lg">
              <ArrowLeft className="w-4 h-4" />
            </Link>
            <div>
              <div className="flex items-center gap-3">
                <h2 className="text-xl font-semibold text-white">{prompt.title}</h2>
                <Badge variant="neutral">v{prompt.version}</Badge>
              </div>
              <p className="text-gray-400 text-sm mt-0.5">{prompt.description || 'Sin descripción'}</p>
            </div>
          </div>
          <div className="flex items-center gap-2">
            <Link
              to={`/prompts/${id}/test-suites`}
              className="flex items-center gap-2 px-3 py-2 bg-gray-800 hover:bg-gray-700 rounded-lg text-sm border border-gray-700"
            >
              <FlaskConical className="w-4 h-4" />
              Test Suites
            </Link>
            <Link to={`/prompts/${id}/edit`} className="flex items-center gap-2 px-3 py-2 bg-gray-800 hover:bg-gray-700 rounded-lg text-sm border border-gray-700">
              <Edit2 className="w-4 h-4" />
              Editar
            </Link>
            <button type="button" onClick={handleDelete} className="flex items-center gap-2 px-3 py-2 bg-gray-800 hover:bg-red-900/40 rounded-lg text-sm border border-gray-700 text-red-400">
              <Trash2 className="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6 space-y-6">
        <div className="flex justify-between items-start gap-4">
          <div className="flex-1">
            <div className="flex items-center justify-between mb-2">
              <h3 className="text-sm font-medium text-gray-300">Contenido</h3>
              <button type="button" onClick={copyContent} className="text-xs text-blue-400 hover:text-blue-300 flex items-center gap-1">
                {copied ? <Check className="w-3 h-3" /> : <Copy className="w-3 h-3" />}
                {copied ? 'Copiado' : 'Copiar'}
              </button>
            </div>
            <pre className="bg-gray-900 border border-gray-800 rounded-xl p-4 text-sm text-gray-300 font-mono whitespace-pre-wrap overflow-x-auto">{prompt.content}</pre>
          </div>
        </div>

        {uniqueVars.length > 0 && (
          <div>
            <h3 className="text-sm font-medium text-gray-300 mb-2">Variables</h3>
            <div className="flex flex-wrap gap-2">
              {uniqueVars.map((v) => (
                <code key={v} className="px-2 py-1 bg-gray-800 rounded text-xs text-blue-300">
                  {`{{${v}}}`}
                </code>
              ))}
            </div>
          </div>
        )}

        <div>
          <button type="button" onClick={() => setShowVersions(!showVersions)} className="flex items-center gap-2 text-sm text-gray-300 mb-2">
            <History className="w-4 h-4" />
            Historial de versiones ({versions.length})
            {showVersions ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
          </button>
          {showVersions && (
            <ul className="space-y-2 border border-gray-800 rounded-lg p-3 bg-gray-900 max-h-64 overflow-y-auto">
              {versions.map((v) => (
                <li key={v.id} className="text-xs text-gray-400 border-b border-gray-800 pb-2 last:border-0">
                  <span className="text-white font-medium">v{v.version}</span> — {new Date(v.createdAt).toLocaleString()}
                  <pre className="mt-1 text-gray-500 line-clamp-2 font-mono">{v.content}</pre>
                </li>
              ))}
            </ul>
          )}
        </div>

        {(() => {
          const modelDisplay = prompt.targetModelId ?? prompt.modelHint;
          const hasConfig =
            (modelDisplay != null && modelDisplay !== '') ||
            prompt.temperature != null ||
            prompt.maxTokens != null ||
            prompt.topP != null;
          if (!hasConfig) return null;
          return (
            <div>
              <h3 className="text-sm font-medium text-gray-300 mb-2">Configuración</h3>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                <div className="bg-gray-900 border border-gray-800 rounded-lg p-3">
                  <div className="text-xs text-gray-500 mb-1">Modelo</div>
                  <div className="text-gray-200 font-mono break-all">{modelDisplay || '—'}</div>
                </div>
                <div className="bg-gray-900 border border-gray-800 rounded-lg p-3">
                  <div className="text-xs text-gray-500 mb-1">Temperatura</div>
                  <div className="text-gray-200">{prompt.temperature != null ? String(prompt.temperature) : '—'}</div>
                </div>
                <div className="bg-gray-900 border border-gray-800 rounded-lg p-3">
                  <div className="text-xs text-gray-500 mb-1">Max tokens</div>
                  <div className="text-gray-200">{prompt.maxTokens != null ? String(prompt.maxTokens) : '—'}</div>
                </div>
                <div className="bg-gray-900 border border-gray-800 rounded-lg p-3">
                  <div className="text-xs text-gray-500 mb-1">Top P</div>
                  <div className="text-gray-200">{prompt.topP != null ? String(prompt.topP) : '—'}</div>
                </div>
              </div>
            </div>
          );
        })()}

        <div className="grid grid-cols-2 gap-4 text-sm text-gray-400">
          <div>
            <Clock className="w-3 h-3 inline mr-1" />
            Creado: {new Date(prompt.createdAt).toLocaleString()}
          </div>
          <div>Actualizado: {new Date(prompt.updatedAt).toLocaleString()}</div>
        </div>

        {(prompt.tagSummaries?.length ?? 0) > 0 && (
          <div className="flex flex-wrap gap-2">
            {prompt.tagSummaries!.map((t) => (
              <span key={t.id} className="inline-flex items-center gap-1 px-2 py-1 bg-gray-800 rounded text-xs text-gray-300">
                <Tag className="w-3 h-3" />
                {t.name}
              </span>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

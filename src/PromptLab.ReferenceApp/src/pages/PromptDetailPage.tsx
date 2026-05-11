import { useState, useEffect } from 'react';
import { ArrowLeft, CreditCard as Edit2, Trash2, Tag, Clock, FlaskConical, History, Copy, Check, ChevronDown, ChevronUp } from 'lucide-react';
import { getPrompt, getPromptVersions, deletePrompt } from '../lib/api';
import type { Prompt, PromptVersion, View } from '../types';
import { Badge } from '../components/Badge';

interface PromptDetailPageProps {
  promptId: string;
  onNavigate: (view: View, id?: string) => void;
}

export function PromptDetailPage({ promptId, onNavigate }: PromptDetailPageProps) {
  const [prompt, setPrompt] = useState<Prompt | null>(null);
  const [versions, setVersions] = useState<PromptVersion[]>([]);
  const [loading, setLoading] = useState(true);
  const [copied, setCopied] = useState(false);
  const [showVersions, setShowVersions] = useState(false);

  useEffect(() => {
    Promise.all([getPrompt(promptId), getPromptVersions(promptId)]).then(([p, v]) => {
      setPrompt(p);
      setVersions(v);
      setLoading(false);
    });
  }, [promptId]);

  async function handleDelete() {
    if (!confirm('Delete this prompt? This will also delete all associated test suites and runs.')) return;
    await deletePrompt(promptId);
    onNavigate('prompts');
  }

  function copyContent() {
    if (!prompt) return;
    navigator.clipboard.writeText(prompt.content);
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

  if (!prompt) {
    return (
      <div className="flex-1 flex items-center justify-center text-gray-400">
        Prompt not found.
      </div>
    );
  }

  const variables = Array.from(prompt.content.matchAll(/\{\{(\w+)\}\}/g)).map((m) => m[1]);
  const uniqueVars = Array.from(new Set(variables));

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <button
              onClick={() => onNavigate('prompts')}
              className="text-gray-400 hover:text-white transition-colors p-1.5 hover:bg-gray-800 rounded-lg"
            >
              <ArrowLeft className="w-4 h-4" />
            </button>
            <div>
              <div className="flex items-center gap-3">
                <h2 className="text-xl font-semibold text-white">{prompt.name}</h2>
                <Badge variant="neutral">v{prompt.version}</Badge>
              </div>
              <p className="text-gray-400 text-sm mt-0.5">{prompt.description || 'No description'}</p>
            </div>
          </div>
          <div className="flex items-center gap-2">
            <button
              onClick={() => onNavigate('test-suites', promptId)}
              className="flex items-center gap-2 px-3 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 hover:text-white rounded-lg text-sm font-medium transition-colors border border-gray-700"
            >
              <FlaskConical className="w-4 h-4" />
              Test Suites
            </button>
            <button
              onClick={() => onNavigate('prompt-edit', promptId)}
              className="flex items-center gap-2 px-3 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 hover:text-white rounded-lg text-sm font-medium transition-colors border border-gray-700"
            >
              <Edit2 className="w-4 h-4" />
              Edit
            </button>
            <button
              onClick={handleDelete}
              className="flex items-center gap-2 px-3 py-2 bg-gray-800 hover:bg-red-900/40 text-gray-400 hover:text-red-400 rounded-lg text-sm font-medium transition-colors border border-gray-700 hover:border-red-800/50"
            >
              <Trash2 className="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        <div className="max-w-4xl grid grid-cols-3 gap-6">
          <div className="col-span-2 space-y-5">
            <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
              <div className="flex items-center justify-between mb-3">
                <h3 className="text-sm font-medium text-gray-300">Prompt Content</h3>
                <button
                  onClick={copyContent}
                  className="flex items-center gap-1.5 text-xs text-gray-500 hover:text-gray-300 transition-colors"
                >
                  {copied ? <Check className="w-3.5 h-3.5 text-emerald-400" /> : <Copy className="w-3.5 h-3.5" />}
                  {copied ? 'Copied!' : 'Copy'}
                </button>
              </div>
              <pre className="text-sm text-gray-300 font-mono whitespace-pre-wrap leading-relaxed bg-gray-800 rounded-lg p-4 max-h-96 overflow-y-auto">
                {prompt.content}
              </pre>
            </div>

            {uniqueVars.length > 0 && (
              <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
                <h3 className="text-sm font-medium text-gray-300 mb-3">Detected Variables</h3>
                <div className="flex flex-wrap gap-2">
                  {uniqueVars.map((v) => (
                    <span key={v} className="px-2.5 py-1 bg-amber-900/30 text-amber-400 text-xs rounded-full border border-amber-800/40 font-mono">
                      {`{{${v}}}`}
                    </span>
                  ))}
                </div>
              </div>
            )}

            <div className="bg-gray-900 rounded-xl border border-gray-800 overflow-hidden">
              <button
                onClick={() => setShowVersions(!showVersions)}
                className="w-full flex items-center justify-between px-5 py-4 text-sm font-medium text-gray-300 hover:text-white transition-colors"
              >
                <div className="flex items-center gap-2">
                  <History className="w-4 h-4 text-gray-500" />
                  Version History ({versions.length})
                </div>
                {showVersions ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
              </button>
              {showVersions && (
                <div className="border-t border-gray-800 divide-y divide-gray-800">
                  {versions.map((v) => (
                    <div key={v.id} className="px-5 py-3">
                      <div className="flex items-center justify-between mb-2">
                        <Badge variant={v.version === prompt.version ? 'info' : 'neutral'}>
                          v{v.version} {v.version === prompt.version ? '(current)' : ''}
                        </Badge>
                        <span className="text-xs text-gray-600">
                          {new Date(v.created_at).toLocaleString()}
                        </span>
                      </div>
                      <pre className="text-xs text-gray-500 font-mono bg-gray-800 rounded p-2.5 max-h-20 overflow-hidden truncate">
                        {v.content}
                      </pre>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          <div className="space-y-4">
            <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
              <h3 className="text-sm font-medium text-gray-300 mb-4">Configuration</h3>
              <div className="space-y-3">
                <div>
                  <p className="text-xs text-gray-500 mb-1">Model</p>
                  <p className="text-sm text-white font-mono">{prompt.model}</p>
                </div>
                <div>
                  <p className="text-xs text-gray-500 mb-1">Temperature</p>
                  <div className="flex items-center gap-2">
                    <div className="flex-1 bg-gray-800 rounded-full h-1.5">
                      <div
                        className="bg-blue-500 h-1.5 rounded-full"
                        style={{ width: `${(prompt.temperature / 2) * 100}%` }}
                      />
                    </div>
                    <span className="text-sm text-white tabular-nums">{prompt.temperature}</span>
                  </div>
                </div>
                <div>
                  <p className="text-xs text-gray-500 mb-1">Max Tokens</p>
                  <p className="text-sm text-white">{prompt.max_tokens.toLocaleString()}</p>
                </div>
              </div>
            </div>

            {prompt.tags.length > 0 && (
              <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
                <h3 className="text-sm font-medium text-gray-300 mb-3">Tags</h3>
                <div className="flex flex-wrap gap-2">
                  {prompt.tags.map((tag) => (
                    <span key={tag} className="inline-flex items-center gap-1 px-2 py-0.5 bg-gray-800 text-gray-400 text-xs rounded-full">
                      <Tag className="w-2.5 h-2.5" />
                      {tag}
                    </span>
                  ))}
                </div>
              </div>
            )}

            <div className="bg-gray-900 rounded-xl border border-gray-800 p-5">
              <h3 className="text-sm font-medium text-gray-300 mb-3">Metadata</h3>
              <div className="space-y-2.5">
                <div className="flex items-center gap-2 text-xs text-gray-500">
                  <Clock className="w-3.5 h-3.5" />
                  Created {new Date(prompt.created_at).toLocaleDateString()}
                </div>
                <div className="flex items-center gap-2 text-xs text-gray-500">
                  <Clock className="w-3.5 h-3.5" />
                  Updated {new Date(prompt.updated_at).toLocaleDateString()}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

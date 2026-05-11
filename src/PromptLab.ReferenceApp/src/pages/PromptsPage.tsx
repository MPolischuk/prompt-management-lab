import { useState, useEffect } from 'react';
import { Plus, Search, Tag, Clock, Copy, Trash2, CreditCard as Edit2, FlaskConical } from 'lucide-react';
import { getPrompts, deletePrompt } from '../lib/api';
import type { Prompt, View } from '../types';
import { Badge } from '../components/Badge';

interface PromptsPageProps {
  onNavigate: (view: View, id?: string) => void;
}

export function PromptsPage({ onNavigate }: PromptsPageProps) {
  const [prompts, setPrompts] = useState<Prompt[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [tagFilter, setTagFilter] = useState('');

  useEffect(() => {
    load();
  }, []);

  async function load() {
    setLoading(true);
    try {
      const data = await getPrompts();
      setPrompts(data);
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id: string, e: React.MouseEvent) {
    e.stopPropagation();
    if (!confirm('Delete this prompt? This will also delete all associated test suites and runs.')) return;
    await deletePrompt(id);
    setPrompts((prev) => prev.filter((p) => p.id !== id));
  }

  const allTags = Array.from(new Set(prompts.flatMap((p) => p.tags)));

  const filtered = prompts.filter((p) => {
    const matchSearch = !search || p.name.toLowerCase().includes(search.toLowerCase()) || p.description.toLowerCase().includes(search.toLowerCase()) || p.content.toLowerCase().includes(search.toLowerCase());
    const matchTag = !tagFilter || p.tags.includes(tagFilter);
    return matchSearch && matchTag;
  });

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-xl font-semibold text-white">Prompts</h2>
            <p className="text-gray-400 text-sm mt-0.5">{prompts.length} prompt{prompts.length !== 1 ? 's' : ''} in your library</p>
          </div>
          <button
            onClick={() => onNavigate('prompt-create')}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-sm font-medium transition-colors"
          >
            <Plus className="w-4 h-4" />
            New Prompt
          </button>
        </div>

        <div className="flex gap-3 mt-4">
          <div className="relative flex-1 max-w-xs">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-500" />
            <input
              type="text"
              placeholder="Search prompts..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full pl-9 pr-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-gray-200 placeholder-gray-500 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500/30"
            />
          </div>
          {allTags.length > 0 && (
            <div className="flex items-center gap-2 flex-wrap">
              <button
                onClick={() => setTagFilter('')}
                className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${!tagFilter ? 'bg-blue-600 text-white' : 'bg-gray-800 text-gray-400 hover:text-gray-200'}`}
              >
                All
              </button>
              {allTags.map((tag) => (
                <button
                  key={tag}
                  onClick={() => setTagFilter(tag === tagFilter ? '' : tag)}
                  className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-colors ${tag === tagFilter ? 'bg-blue-600 text-white' : 'bg-gray-800 text-gray-400 hover:text-gray-200'}`}
                >
                  {tag}
                </button>
              ))}
            </div>
          )}
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        {loading ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="bg-gray-900 rounded-xl border border-gray-800 h-48 animate-pulse" />
            ))}
          </div>
        ) : filtered.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-64 text-center">
            <div className="w-12 h-12 rounded-xl bg-gray-800 flex items-center justify-center mb-4">
              <Search className="w-6 h-6 text-gray-600" />
            </div>
            <p className="text-gray-400 font-medium">{search || tagFilter ? 'No prompts match your filters' : 'No prompts yet'}</p>
            {!search && !tagFilter && (
              <button
                onClick={() => onNavigate('prompt-create')}
                className="mt-4 text-blue-400 hover:text-blue-300 text-sm"
              >
                Create your first prompt
              </button>
            )}
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {filtered.map((prompt) => (
              <div
                key={prompt.id}
                onClick={() => onNavigate('prompt-detail', prompt.id)}
                className="bg-gray-900 rounded-xl border border-gray-800 p-5 hover:border-gray-700 hover:bg-gray-850 cursor-pointer group transition-all duration-150"
              >
                <div className="flex items-start justify-between mb-3">
                  <div className="flex-1 min-w-0">
                    <h3 className="text-white font-medium text-sm truncate">{prompt.name}</h3>
                    <p className="text-gray-500 text-xs mt-0.5 truncate">{prompt.description || 'No description'}</p>
                  </div>
                  <Badge variant="neutral">v{prompt.version}</Badge>
                </div>

                <div className="bg-gray-800 rounded-lg p-3 mb-3 h-16 overflow-hidden relative">
                  <p className="text-gray-400 text-xs font-mono leading-relaxed line-clamp-3">{prompt.content}</p>
                  <div className="absolute bottom-0 left-0 right-0 h-6 bg-gradient-to-t from-gray-800" />
                </div>

                {prompt.tags.length > 0 && (
                  <div className="flex flex-wrap gap-1.5 mb-3">
                    {prompt.tags.slice(0, 3).map((tag) => (
                      <span key={tag} className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-gray-800 text-gray-400 text-xs rounded">
                        <Tag className="w-2.5 h-2.5" />
                        {tag}
                      </span>
                    ))}
                    {prompt.tags.length > 3 && (
                      <span className="text-gray-600 text-xs">+{prompt.tags.length - 3}</span>
                    )}
                  </div>
                )}

                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-1 text-gray-600 text-xs">
                    <Clock className="w-3 h-3" />
                    {new Date(prompt.updated_at).toLocaleDateString()}
                  </div>
                  <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button
                      onClick={(e) => { e.stopPropagation(); onNavigate('test-suites', prompt.id); }}
                      className="p-1.5 text-gray-500 hover:text-blue-400 hover:bg-gray-800 rounded-md transition-colors"
                      title="Test suites"
                    >
                      <FlaskConical className="w-3.5 h-3.5" />
                    </button>
                    <button
                      onClick={(e) => { e.stopPropagation(); onNavigate('prompt-edit', prompt.id); }}
                      className="p-1.5 text-gray-500 hover:text-gray-300 hover:bg-gray-800 rounded-md transition-colors"
                      title="Edit"
                    >
                      <Edit2 className="w-3.5 h-3.5" />
                    </button>
                    <button
                      onClick={(e) => handleDelete(prompt.id, e)}
                      className="p-1.5 text-gray-500 hover:text-red-400 hover:bg-gray-800 rounded-md transition-colors"
                      title="Delete"
                    >
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

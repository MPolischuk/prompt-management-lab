import { useState } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { Plus, Search, Tag, Clock, Trash2, CreditCard as Edit2, FlaskConical } from 'lucide-react';
import { deletePrompt, getTags, searchPrompts } from '../lib/api';
import { queryKeys } from '../lib/queryKeys';
import type { Prompt } from '../types';
import { Badge } from '../components/Badge';
import { Pagination } from '../components/Pagination';

export function PromptsPage() {
  const qc = useQueryClient();
  const [search, setSearch] = useState('');
  const [tagFilter, setTagFilter] = useState('');
  const [page, setPage] = useState(1);
  const pageSize = 12;

  const { data: tagList = [] } = useQuery({
    queryKey: queryKeys.tags(),
    queryFn: () => getTags(),
  });

  const { data, isLoading } = useQuery({
    queryKey: [...queryKeys.prompts, search, tagFilter, page],
    queryFn: () =>
      searchPrompts({
        query: search || undefined,
        tagId: tagFilter || undefined,
        pageNumber: page,
        pageSize,
      }),
  });

  const prompts = data?.items ?? [];
  const totalRows = data?.totalRows ?? 0;

  async function handleDelete(id: string, e: React.MouseEvent) {
    e.preventDefault();
    e.stopPropagation();
    if (!confirm('¿Eliminar este prompt? (baja lógica en el servidor)')) return;
    await deletePrompt(id);
    await qc.invalidateQueries({ queryKey: queryKeys.prompts });
  }

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-xl font-semibold text-white">Prompts</h2>
            <p className="text-gray-400 text-sm mt-0.5">
              {totalRows} prompt{totalRows !== 1 ? 's' : ''} (página {page})
            </p>
          </div>
          <Link
            to="/prompts/new"
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-sm font-medium transition-colors"
          >
            <Plus className="w-4 h-4" />
            Nuevo prompt
          </Link>
        </div>

        <div className="flex gap-3 mt-4 flex-wrap">
          <div className="relative flex-1 max-w-xs min-w-[200px]">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-500" />
            <input
              type="text"
              placeholder="Buscar..."
              value={search}
              onChange={(e) => {
                setSearch(e.target.value);
                setPage(1);
              }}
              className="w-full pl-9 pr-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-gray-200 placeholder-gray-500 focus:outline-none focus:border-blue-500"
            />
          </div>
          {tagList.length > 0 && (
            <div className="flex items-center gap-2 flex-wrap">
              <button
                type="button"
                onClick={() => {
                  setTagFilter('');
                  setPage(1);
                }}
                className={`px-3 py-1.5 rounded-lg text-xs font-medium ${!tagFilter ? 'bg-blue-600 text-white' : 'bg-gray-800 text-gray-400'}`}
              >
                Todos
              </button>
              {tagList.map((t) => (
                <button
                  key={t.id}
                  type="button"
                  onClick={() => {
                    setTagFilter(tagFilter === t.id ? '' : t.id);
                    setPage(1);
                  }}
                  className={`px-3 py-1.5 rounded-lg text-xs font-medium ${
                    tagFilter === t.id ? 'bg-blue-600 text-white' : 'bg-gray-800 text-gray-400'
                  }`}
                >
                  {t.name}
                </button>
              ))}
            </div>
          )}
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        {isLoading ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="bg-gray-900 rounded-xl border border-gray-800 h-48 animate-pulse" />
            ))}
          </div>
        ) : prompts.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-64 text-center text-gray-400">
            <Search className="w-8 h-8 mb-2 opacity-50" />
            <p>No hay prompts</p>
            <Link to="/prompts/new" className="mt-4 text-blue-400 text-sm">
              Crear el primero
            </Link>
          </div>
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {prompts.map((prompt) => (
                <PromptCard key={prompt.id} prompt={prompt} onDelete={handleDelete} />
              ))}
            </div>
            <Pagination pageNumber={page} pageSize={pageSize} totalRows={totalRows} onPageChange={setPage} />
          </>
        )}
      </div>
    </div>
  );
}

function PromptCard({ prompt, onDelete }: { prompt: Prompt; onDelete: (id: string, e: React.MouseEvent) => void }) {
  return (
    <div
      data-testid={`prompt-card-${prompt.id}`}
      className="block bg-gray-900 rounded-xl border border-gray-800 p-5 hover:border-gray-700 hover:bg-gray-850 cursor-pointer group transition-all"
    >
      <Link
        to={`/prompts/${prompt.id}`}
        className="block text-inherit no-underline rounded-lg outline-none focus-visible:ring-2 focus-visible:ring-blue-500 focus-visible:ring-offset-2 focus-visible:ring-offset-gray-900"
      >
        <div className="flex items-start justify-between mb-3">
          <div className="flex-1 min-w-0">
            <h3 className="text-white font-medium text-sm truncate">{prompt.title}</h3>
            <p className="text-gray-500 text-xs mt-0.5 truncate">{prompt.description || 'Sin descripción'}</p>
          </div>
          <Badge variant="neutral">v{prompt.version}</Badge>
        </div>
        <div className="bg-gray-800 rounded-lg p-3 mb-3 h-16 overflow-hidden relative">
          <p className="text-gray-400 text-xs font-mono leading-relaxed line-clamp-3">{prompt.content}</p>
        </div>
        {(prompt.tagSummaries?.length ?? 0) > 0 && (
          <div className="flex flex-wrap gap-1.5 mb-3">
            {prompt.tagSummaries!.slice(0, 3).map((tag) => (
              <span key={tag.id} className="inline-flex items-center gap-1 px-1.5 py-0.5 bg-gray-800 text-gray-400 text-xs rounded">
                <Tag className="w-2.5 h-2.5" />
                {tag.name}
              </span>
            ))}
          </div>
        )}
      </Link>
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-1 text-gray-600 text-xs">
          <Clock className="w-3 h-3" />
          {new Date(prompt.updatedAt).toLocaleDateString()}
        </div>
        <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
          <Link
            to={`/prompts/${prompt.id}/test-suites`}
            className="p-1.5 text-gray-500 hover:text-blue-400 hover:bg-gray-800 rounded-md"
            title="Test suites"
          >
            <FlaskConical className="w-3.5 h-3.5" />
          </Link>
          <Link to={`/prompts/${prompt.id}/edit`} className="p-1.5 text-gray-500 hover:text-gray-300 hover:bg-gray-800 rounded-md">
            <Edit2 className="w-3.5 h-3.5" />
          </Link>
          <button type="button" onClick={(e) => onDelete(prompt.id, e)} className="p-1.5 text-gray-500 hover:text-red-400 hover:bg-gray-800 rounded-md">
            <Trash2 className="w-3.5 h-3.5" />
          </button>
        </div>
      </div>
    </div>
  );
}

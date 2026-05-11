import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { ArrowLeft, Save, AlertCircle } from 'lucide-react';
import { createPrompt, getPrompt, updatePrompt } from '../lib/api';
import { queryKeys } from '../lib/queryKeys';
import { TagSelector } from '../components/TagSelector';
import { ModelSelector } from '../components/ModelSelector';

export function PromptFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const qc = useQueryClient();
  const isEdit = !!id;
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [tagIds, setTagIds] = useState<string[]>([]);
  const [modelId, setModelId] = useState('');
  const [form, setForm] = useState({
    title: '',
    description: '',
    content: '',
    temperature: 0.7,
    maxTokens: 1000,
    topP: 1 as number | null,
  });

  useEffect(() => {
    if (!id) {
      setLoading(false);
      return;
    }
    getPrompt(id).then((p) => {
      if (p) {
        setForm({
          title: p.title,
          description: p.description ?? '',
          content: p.content,
          temperature: p.temperature ?? 0.7,
          maxTokens: p.maxTokens ?? 1000,
          topP: p.topP ?? 1,
        });
        setTagIds((p.tagSummaries ?? []).map((t) => t.id));
        setModelId(p.targetModelId ?? '');
      }
      setLoading(false);
    });
  }, [id]);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.title.trim()) {
      setError('El título es obligatorio');
      return;
    }
    if (!form.content.trim()) {
      setError('El contenido es obligatorio');
      return;
    }

    setSaving(true);
    setError('');
    try {
      const body = {
        title: form.title.trim(),
        description: form.description || null,
        content: form.content,
        category: null,
        language: null,
        modelHint: modelId || null,
        targetModelId: modelId || null,
        temperature: form.temperature,
        maxTokens: form.maxTokens,
        topP: form.topP,
        isActive: true,
        tagIds,
      };

      if (isEdit && id) {
        await updatePrompt(id, body);
        await qc.invalidateQueries({ queryKey: ['prompts'] });
        await qc.invalidateQueries({ queryKey: queryKeys.prompt(id) });
        navigate(`/prompts/${id}`);
      } else {
        const created = await createPrompt(body);
        await qc.invalidateQueries({ queryKey: ['prompts'] });
        navigate(`/prompts/${created.id}`);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al guardar');
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="w-6 h-6 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div className="flex-1 flex flex-col h-screen overflow-hidden">
      <div className="px-8 py-6 border-b border-gray-800 bg-gray-950">
        <div className="flex items-center gap-4">
          <Link to={isEdit && id ? `/prompts/${id}` : '/prompts'} className="text-gray-400 hover:text-white p-1.5 hover:bg-gray-800 rounded-lg">
            <ArrowLeft className="w-4 h-4" />
          </Link>
          <div>
            <h2 className="text-xl font-semibold text-white">{isEdit ? 'Editar prompt' : 'Nuevo prompt'}</h2>
            <p className="text-gray-400 text-sm mt-0.5">Al cambiar el contenido se crea una nueva versión en el servidor</p>
          </div>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        <form onSubmit={handleSubmit} className="max-w-3xl space-y-6">
          {error && (
            <div className="flex items-center gap-3 px-4 py-3 bg-red-900/30 border border-red-800/50 rounded-lg text-red-400 text-sm">
              <AlertCircle className="w-4 h-4 flex-shrink-0" />
              {error}
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-300 mb-1.5">Título *</label>
            <input
              type="text"
              value={form.title}
              onChange={(e) => setForm((f) => ({ ...f, title: e.target.value }))}
              className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-gray-200"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-300 mb-1.5">Descripción</label>
            <input
              type="text"
              value={form.description}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
              className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-gray-200"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-300 mb-1.5">Contenido *</label>
            <textarea
              rows={12}
              value={form.content}
              onChange={(e) => setForm((f) => ({ ...f, content: e.target.value }))}
              className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-gray-200 font-mono"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="col-span-2">
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Modelo</label>
              <ModelSelector value={modelId} onChange={setModelId} />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Temperatura</label>
              <input
                type="number"
                step="0.1"
                min={0}
                max={2}
                value={form.temperature}
                onChange={(e) => setForm((f) => ({ ...f, temperature: Number(e.target.value) }))}
                className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Max tokens</label>
              <input
                type="number"
                min={1}
                value={form.maxTokens}
                onChange={(e) => setForm((f) => ({ ...f, maxTokens: Number(e.target.value) }))}
                className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-300 mb-2">Tags</label>
            <TagSelector selectedIds={tagIds} onChange={setTagIds} />
          </div>

          <button
            type="submit"
            disabled={saving}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-500 disabled:opacity-50 text-white rounded-lg text-sm font-medium"
          >
            <Save className="w-4 h-4" />
            {saving ? 'Guardando...' : 'Guardar'}
          </button>
        </form>
      </div>
    </div>
  );
}

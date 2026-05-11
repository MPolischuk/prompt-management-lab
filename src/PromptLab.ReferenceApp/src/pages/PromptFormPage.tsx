import { useState, useEffect } from 'react';
import { ArrowLeft, Plus, X, Save, AlertCircle } from 'lucide-react';
import { getPrompt, createPrompt, updatePrompt } from '../lib/api';
import type { Prompt, View } from '../types';

const MODELS = ['gpt-4', 'gpt-4-turbo', 'gpt-3.5-turbo', 'claude-3-opus', 'claude-3-sonnet', 'claude-3-haiku', 'gemini-pro'];

interface PromptFormPageProps {
  promptId?: string;
  onNavigate: (view: View, id?: string) => void;
}

export function PromptFormPage({ promptId, onNavigate }: PromptFormPageProps) {
  const isEdit = !!promptId;
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [tagInput, setTagInput] = useState('');

  const [form, setForm] = useState({
    name: '',
    description: '',
    content: '',
    tags: [] as string[],
    model: 'gpt-4',
    temperature: 0.7,
    max_tokens: 1000,
  });

  useEffect(() => {
    if (!promptId) return;
    getPrompt(promptId).then((p) => {
      if (p) {
        setForm({
          name: p.name,
          description: p.description,
          content: p.content,
          tags: p.tags,
          model: p.model,
          temperature: p.temperature,
          max_tokens: p.max_tokens,
        });
      }
      setLoading(false);
    });
  }, [promptId]);

  function addTag() {
    const tag = tagInput.trim().toLowerCase();
    if (tag && !form.tags.includes(tag)) {
      setForm((f) => ({ ...f, tags: [...f.tags, tag] }));
    }
    setTagInput('');
  }

  function removeTag(tag: string) {
    setForm((f) => ({ ...f, tags: f.tags.filter((t) => t !== tag) }));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.name.trim()) { setError('Name is required'); return; }
    if (!form.content.trim()) { setError('Prompt content is required'); return; }

    setSaving(true);
    setError('');
    try {
      if (isEdit && promptId) {
        await updatePrompt(promptId, form);
        onNavigate('prompt-detail', promptId);
      } else {
        const created = await createPrompt(form);
        onNavigate('prompt-detail', created.id);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save prompt');
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
          <button
            onClick={() => onNavigate(isEdit && promptId ? 'prompt-detail' : 'prompts', promptId)}
            className="text-gray-400 hover:text-white transition-colors p-1.5 hover:bg-gray-800 rounded-lg"
          >
            <ArrowLeft className="w-4 h-4" />
          </button>
          <div>
            <h2 className="text-xl font-semibold text-white">{isEdit ? 'Edit Prompt' : 'New Prompt'}</h2>
            <p className="text-gray-400 text-sm mt-0.5">{isEdit ? 'Updating creates a new version automatically' : 'Create a reusable prompt template'}</p>
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

          <div className="grid grid-cols-2 gap-4">
            <div className="col-span-2">
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Name <span className="text-red-400">*</span></label>
              <input
                type="text"
                value={form.name}
                onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
                placeholder="e.g., Customer Support Response"
                className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500/30 text-sm"
              />
            </div>

            <div className="col-span-2">
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Description</label>
              <input
                type="text"
                value={form.description}
                onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
                placeholder="Brief description of what this prompt does"
                className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500/30 text-sm"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-300 mb-1.5">
              Prompt Content <span className="text-red-400">*</span>
              <span className="ml-2 text-gray-500 text-xs font-normal">Use {'{{variable}}'} for dynamic values</span>
            </label>
            <textarea
              value={form.content}
              onChange={(e) => setForm((f) => ({ ...f, content: e.target.value }))}
              placeholder="You are a helpful assistant. {{context}}&#10;&#10;User: {{input}}&#10;&#10;Assistant:"
              rows={10}
              className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500/30 text-sm font-mono resize-none leading-relaxed"
            />
            <div className="flex justify-between mt-1">
              <p className="text-gray-600 text-xs">Characters: {form.content.length}</p>
              <p className="text-gray-600 text-xs">~{Math.ceil(form.content.length / 4)} tokens</p>
            </div>
          </div>

          <div className="grid grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Model</label>
              <select
                value={form.model}
                onChange={(e) => setForm((f) => ({ ...f, model: e.target.value }))}
                className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white focus:outline-none focus:border-blue-500 text-sm appearance-none"
              >
                {MODELS.map((m) => <option key={m} value={m}>{m}</option>)}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">
                Temperature <span className="text-gray-500 font-normal">({form.temperature})</span>
              </label>
              <input
                type="range"
                min="0"
                max="2"
                step="0.1"
                value={form.temperature}
                onChange={(e) => setForm((f) => ({ ...f, temperature: parseFloat(e.target.value) }))}
                className="w-full mt-2 accent-blue-500"
              />
              <div className="flex justify-between text-xs text-gray-600 mt-1">
                <span>0 Precise</span>
                <span>2 Creative</span>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Max Tokens</label>
              <input
                type="number"
                min="1"
                max="32000"
                value={form.max_tokens}
                onChange={(e) => setForm((f) => ({ ...f, max_tokens: parseInt(e.target.value) || 1000 }))}
                className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white focus:outline-none focus:border-blue-500 text-sm"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-300 mb-1.5">Tags</label>
            <div className="flex flex-wrap gap-2 mb-2">
              {form.tags.map((tag) => (
                <span key={tag} className="inline-flex items-center gap-1 px-2.5 py-1 bg-blue-900/40 text-blue-300 text-xs rounded-full border border-blue-800/50">
                  {tag}
                  <button type="button" onClick={() => removeTag(tag)} className="hover:text-red-400 transition-colors">
                    <X className="w-3 h-3" />
                  </button>
                </span>
              ))}
            </div>
            <div className="flex gap-2">
              <input
                type="text"
                value={tagInput}
                onChange={(e) => setTagInput(e.target.value)}
                onKeyDown={(e) => { if (e.key === 'Enter') { e.preventDefault(); addTag(); } }}
                placeholder="Add a tag..."
                className="flex-1 px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 text-sm"
              />
              <button
                type="button"
                onClick={addTag}
                className="px-3 py-2 bg-gray-800 hover:bg-gray-700 border border-gray-700 rounded-lg text-gray-300 transition-colors"
              >
                <Plus className="w-4 h-4" />
              </button>
            </div>
          </div>

          <div className="flex items-center gap-3 pt-2 pb-8">
            <button
              type="submit"
              disabled={saving}
              className="flex items-center gap-2 px-5 py-2.5 bg-blue-600 hover:bg-blue-500 disabled:opacity-50 disabled:cursor-not-allowed text-white rounded-lg text-sm font-medium transition-colors"
            >
              {saving ? (
                <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              ) : (
                <Save className="w-4 h-4" />
              )}
              {saving ? 'Saving...' : isEdit ? 'Save Changes' : 'Create Prompt'}
            </button>
            <button
              type="button"
              onClick={() => onNavigate(isEdit && promptId ? 'prompt-detail' : 'prompts', promptId)}
              className="px-5 py-2.5 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-lg text-sm font-medium transition-colors"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

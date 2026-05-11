import { useState, useEffect } from 'react';
import { ArrowLeft, Plus, Trash2, CreditCard as Edit2, FlaskConical, ChevronRight, AlertCircle, Check, X } from 'lucide-react';
import { getPrompt, getTestSuites, createTestSuite, updateTestSuite, deleteTestSuite } from '../lib/api';
import type { Prompt, TestSuite, View } from '../types';
import { Modal } from '../components/Modal';

interface TestSuitesPageProps {
  promptId: string;
  onNavigate: (view: View, id?: string) => void;
}

interface SuiteForm {
  name: string;
  description: string;
}

export function TestSuitesPage({ promptId, onNavigate }: TestSuitesPageProps) {
  const [prompt, setPrompt] = useState<Prompt | null>(null);
  const [suites, setSuites] = useState<TestSuite[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editSuite, setEditSuite] = useState<TestSuite | null>(null);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [form, setForm] = useState<SuiteForm>({ name: '', description: '' });

  useEffect(() => {
    Promise.all([getPrompt(promptId), getTestSuites(promptId)]).then(([p, s]) => {
      setPrompt(p);
      setSuites(s);
      setLoading(false);
    });
  }, [promptId]);

  function openCreate() {
    setEditSuite(null);
    setForm({ name: '', description: '' });
    setError('');
    setShowModal(true);
  }

  function openEdit(suite: TestSuite, e: React.MouseEvent) {
    e.stopPropagation();
    setEditSuite(suite);
    setForm({ name: suite.name, description: suite.description });
    setError('');
    setShowModal(true);
  }

  async function handleSave() {
    if (!form.name.trim()) { setError('Suite name is required'); return; }
    setSaving(true);
    setError('');
    try {
      if (editSuite) {
        const updated = await updateTestSuite(editSuite.id, form);
        setSuites((prev) => prev.map((s) => s.id === updated.id ? { ...updated, test_cases: s.test_cases } : s));
      } else {
        const created = await createTestSuite({ ...form, prompt_id: promptId });
        setSuites((prev) => [{ ...created, test_cases: [] }, ...prev]);
      }
      setShowModal(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(id: string, e: React.MouseEvent) {
    e.stopPropagation();
    if (!confirm('Delete this test suite and all its cases and runs?')) return;
    await deleteTestSuite(id);
    setSuites((prev) => prev.filter((s) => s.id !== id));
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
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <button
              onClick={() => onNavigate('prompt-detail', promptId)}
              className="text-gray-400 hover:text-white transition-colors p-1.5 hover:bg-gray-800 rounded-lg"
            >
              <ArrowLeft className="w-4 h-4" />
            </button>
            <div>
              <div className="flex items-center gap-2 text-xs text-gray-500 mb-1">
                <span className="hover:text-gray-300 cursor-pointer" onClick={() => onNavigate('prompts')}>Prompts</span>
                <ChevronRight className="w-3 h-3" />
                <span className="hover:text-gray-300 cursor-pointer" onClick={() => onNavigate('prompt-detail', promptId)}>{prompt?.name}</span>
                <ChevronRight className="w-3 h-3" />
                <span className="text-gray-300">Test Suites</span>
              </div>
              <h2 className="text-xl font-semibold text-white">Test Suites</h2>
              <p className="text-gray-400 text-sm mt-0.5">{suites.length} suite{suites.length !== 1 ? 's' : ''} for {prompt?.name}</p>
            </div>
          </div>
          <button
            onClick={openCreate}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-sm font-medium transition-colors"
          >
            <Plus className="w-4 h-4" />
            New Suite
          </button>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        {suites.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-64 text-center">
            <div className="w-12 h-12 rounded-xl bg-gray-800 flex items-center justify-center mb-4">
              <FlaskConical className="w-6 h-6 text-gray-600" />
            </div>
            <p className="text-gray-400 font-medium">No test suites yet</p>
            <button onClick={openCreate} className="mt-4 text-blue-400 hover:text-blue-300 text-sm">
              Create your first test suite
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {suites.map((suite) => (
              <div
                key={suite.id}
                onClick={() => onNavigate('test-suite-detail', suite.id)}
                className="bg-gray-900 rounded-xl border border-gray-800 p-5 hover:border-gray-700 cursor-pointer group transition-all duration-150"
              >
                <div className="flex items-start justify-between mb-2">
                  <h3 className="text-white font-medium text-sm">{suite.name}</h3>
                  <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button
                      onClick={(e) => openEdit(suite, e)}
                      className="p-1 text-gray-500 hover:text-gray-300 hover:bg-gray-800 rounded transition-colors"
                    >
                      <Edit2 className="w-3.5 h-3.5" />
                    </button>
                    <button
                      onClick={(e) => handleDelete(suite.id, e)}
                      className="p-1 text-gray-500 hover:text-red-400 hover:bg-gray-800 rounded transition-colors"
                    >
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>
                  </div>
                </div>
                <p className="text-gray-500 text-xs mb-4">{suite.description || 'No description'}</p>
                <div className="flex items-center justify-between text-xs">
                  <span className="text-gray-500">{suite.test_cases?.length ?? 0} test case{(suite.test_cases?.length ?? 0) !== 1 ? 's' : ''}</span>
                  <span className="text-gray-600">{new Date(suite.created_at).toLocaleDateString()}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {showModal && (
        <Modal title={editSuite ? 'Edit Suite' : 'New Test Suite'} onClose={() => setShowModal(false)}>
          <div className="space-y-4">
            {error && (
              <div className="flex items-center gap-2 px-3 py-2 bg-red-900/30 border border-red-800/50 rounded-lg text-red-400 text-sm">
                <AlertCircle className="w-4 h-4 flex-shrink-0" />
                {error}
              </div>
            )}
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Name <span className="text-red-400">*</span></label>
              <input
                autoFocus
                type="text"
                value={form.name}
                onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
                onKeyDown={(e) => { if (e.key === 'Enter') handleSave(); }}
                placeholder="e.g., Basic functionality tests"
                className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 text-sm"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1.5">Description</label>
              <textarea
                value={form.description}
                onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
                placeholder="What does this suite test?"
                rows={3}
                className="w-full px-3 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500 text-sm resize-none"
              />
            </div>
            <div className="flex justify-end gap-2 pt-1">
              <button
                onClick={() => setShowModal(false)}
                className="px-4 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-lg text-sm transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleSave}
                disabled={saving}
                className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-500 disabled:opacity-50 text-white rounded-lg text-sm font-medium transition-colors"
              >
                {saving ? <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" /> : <Check className="w-4 h-4" />}
                {editSuite ? 'Save' : 'Create'}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}

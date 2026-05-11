import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import { ArrowLeft, Plus, FlaskConical } from 'lucide-react';
import { createTestSuite, deleteTestSuite, getPrompt, getTestSuitesByPrompt } from '../lib/api';
import type { Prompt, TestSuite } from '../types';
import { Modal } from '../components/Modal';

export function TestSuitesPage() {
  const { promptId } = useParams();
  const qc = useQueryClient();
  const [prompt, setPrompt] = useState<Prompt | null>(null);
  const [suites, setSuites] = useState<TestSuite[]>([]);
  const [loading, setLoading] = useState(true);
  const [modal, setModal] = useState(false);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');

  async function load() {
    if (!promptId) return;
    const [p, s] = await Promise.all([getPrompt(promptId), getTestSuitesByPrompt(promptId)]);
    setPrompt(p);
    setSuites(s);
    setLoading(false);
  }

  useEffect(() => {
    void load();
  }, [promptId]);

  async function handleCreate() {
    if (!promptId || !name.trim()) return;
    await createTestSuite(promptId, name.trim(), description || null);
    setModal(false);
    setName('');
    setDescription('');
    await load();
    await qc.invalidateQueries({ queryKey: ['testSuites'] });
  }

  async function handleDelete(id: string) {
    if (!confirm('¿Eliminar suite?')) return;
    await deleteTestSuite(id);
    await load();
  }

  if (loading || !promptId) {
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
          <Link to={`/prompts/${promptId}`} className="text-gray-400 hover:text-white p-1.5 hover:bg-gray-800 rounded-lg">
            <ArrowLeft className="w-4 h-4" />
          </Link>
          <div>
            <h2 className="text-xl font-semibold text-white">Test Suites</h2>
            <p className="text-gray-400 text-sm">{prompt?.title}</p>
          </div>
          <button
            type="button"
            onClick={() => setModal(true)}
            className="ml-auto flex items-center gap-2 px-3 py-2 bg-blue-600 hover:bg-blue-500 rounded-lg text-sm"
          >
            <Plus className="w-4 h-4" />
            Nueva suite
          </button>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto px-8 py-6">
        <div className="grid gap-4 md:grid-cols-2">
          {suites.map((s) => (
            <div key={s.id} className="bg-gray-900 border border-gray-800 rounded-xl p-5 flex justify-between items-start">
              <Link to={`/test-suites/${s.id}`} className="flex-1 min-w-0">
                <div className="flex items-center gap-2 text-white font-medium">
                  <FlaskConical className="w-4 h-4 text-blue-400" />
                  {s.name}
                </div>
                <p className="text-gray-500 text-sm mt-1">{s.description || 'Sin descripción'}</p>
              </Link>
              <button type="button" onClick={() => handleDelete(s.id)} className="text-xs text-red-400 hover:underline shrink-0">
                Eliminar
              </button>
            </div>
          ))}
        </div>
        {suites.length === 0 && <p className="text-gray-500 text-center py-12">No hay suites. Creá una.</p>}
      </div>

      <Modal isOpen={modal} onClose={() => setModal(false)} title="Nueva test suite">
        <div className="space-y-3">
          <input
            placeholder="Nombre"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm"
          />
          <textarea
            placeholder="Descripción"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm"
            rows={3}
          />
          <button type="button" onClick={handleCreate} className="w-full py-2 bg-blue-600 rounded-lg text-sm font-medium">
            Crear
          </button>
        </div>
      </Modal>
    </div>
  );
}

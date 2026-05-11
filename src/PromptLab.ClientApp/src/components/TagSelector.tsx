import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Plus, X, Search } from 'lucide-react';
import { createTag, getTags } from '../lib/api';
import { queryKeys } from '../lib/queryKeys';
interface TagSelectorProps {
  selectedIds: string[];
  onChange: (ids: string[]) => void;
}

export function TagSelector({ selectedIds, onChange }: TagSelectorProps) {
  const [q, setQ] = useState('');
  const [newName, setNewName] = useState('');
  const { data: allTags = [] } = useQuery({
    queryKey: queryKeys.tags(q),
    queryFn: () => getTags(q || undefined),
  });

  const selectedSet = new Set(selectedIds);
  const selectedSummaries = allTags.filter((t) => selectedSet.has(t.id));

  function toggle(id: string) {
    if (selectedSet.has(id)) onChange(selectedIds.filter((x) => x !== id));
    else onChange([...selectedIds, id]);
  }

  async function handleCreate() {
    const name = newName.trim();
    if (!name) return;
    const id = await createTag(name);
    setNewName('');
    onChange([...selectedIds, id]);
  }

  return (
    <div className="space-y-3">
      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-500" />
        <input
          type="text"
          placeholder="Buscar tags..."
          value={q}
          onChange={(e) => setQ(e.target.value)}
          className="w-full pl-9 pr-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-gray-200"
        />
      </div>
      <div className="flex flex-wrap gap-2">
        {selectedSummaries.map((t) => (
          <button
            key={t.id}
            type="button"
            onClick={() => toggle(t.id)}
            className="inline-flex items-center gap-1 px-2 py-1 bg-blue-600/30 text-blue-300 text-xs rounded border border-blue-600/40"
          >
            {t.name}
            <X className="w-3 h-3" />
          </button>
        ))}
      </div>
      <div className="max-h-40 overflow-y-auto border border-gray-800 rounded-lg divide-y divide-gray-800">
        {allTags
          .filter((t) => !selectedSet.has(t.id))
          .map((t) => (
            <button
              key={t.id}
              type="button"
              onClick={() => toggle(t.id)}
              className="w-full text-left px-3 py-2 text-sm text-gray-300 hover:bg-gray-800"
            >
              {t.name}
            </button>
          ))}
      </div>
      <div className="flex gap-2">
        <input
          type="text"
          placeholder="Nuevo tag"
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
          className="flex-1 px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm"
        />
        <button type="button" onClick={handleCreate} className="px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-gray-200 hover:bg-gray-700">
          <Plus className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
}

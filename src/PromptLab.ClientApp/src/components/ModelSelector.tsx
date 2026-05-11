import { useQuery } from '@tanstack/react-query';
import { getAiModels } from '../lib/api';
import { queryKeys } from '../lib/queryKeys';

interface ModelSelectorProps {
  value: string;
  onChange: (modelId: string) => void;
}

export function ModelSelector({ value, onChange }: ModelSelectorProps) {
  const { data: models = [], isLoading } = useQuery({
    queryKey: queryKeys.aiModels,
    queryFn: getAiModels,
  });

  if (isLoading) {
    return <div className="h-10 bg-gray-800 rounded-lg animate-pulse" />;
  }

  const enabled = models.filter((m) => m.enabled);

  return (
    <select
      value={value}
      onChange={(e) => onChange(e.target.value)}
      className="w-full px-3 py-2 bg-gray-800 border border-gray-700 rounded-lg text-sm text-gray-200"
    >
      <option value="">Seleccionar modelo</option>
      {enabled.map((m) => (
        <option key={m.id} value={m.id}>
          {m.displayName} ({m.provider})
        </option>
      ))}
    </select>
  );
}

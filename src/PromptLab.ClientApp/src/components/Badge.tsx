type Variant = 'default' | 'success' | 'error' | 'warning' | 'info' | 'neutral';

const variants: Record<Variant, string> = {
  default: 'bg-blue-600/20 text-blue-400 border-blue-600/30',
  success: 'bg-emerald-600/20 text-emerald-400 border-emerald-600/30',
  error: 'bg-red-600/20 text-red-400 border-red-600/30',
  warning: 'bg-amber-600/20 text-amber-400 border-amber-600/30',
  info: 'bg-sky-600/20 text-sky-400 border-sky-600/30',
  neutral: 'bg-gray-700 text-gray-300 border-gray-600',
};

export function Badge({ children, variant = 'default' }: { children: React.ReactNode; variant?: Variant }) {
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium border ${variants[variant]}`}>
      {children}
    </span>
  );
}

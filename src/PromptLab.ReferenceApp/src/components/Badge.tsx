interface BadgeProps {
    children: React.ReactNode;
    variant?: 'default' | 'success' | 'error' | 'warning' | 'info' | 'neutral';
    size?: 'sm' | 'md';
  }
  
  export function Badge({ children, variant = 'default', size = 'sm' }: BadgeProps) {
    const variantClass = {
      default: 'bg-gray-800 text-gray-300',
      success: 'bg-emerald-900/60 text-emerald-400 border border-emerald-800/50',
      error: 'bg-red-900/60 text-red-400 border border-red-800/50',
      warning: 'bg-amber-900/60 text-amber-400 border border-amber-800/50',
      info: 'bg-blue-900/60 text-blue-400 border border-blue-800/50',
      neutral: 'bg-gray-800 text-gray-400 border border-gray-700',
    }[variant];
  
    const sizeClass = size === 'sm' ? 'px-2 py-0.5 text-xs' : 'px-2.5 py-1 text-sm';
  
    return (
      <span className={`inline-flex items-center rounded-full font-medium ${variantClass} ${sizeClass}`}>
        {children}
      </span>
    );
  }
  
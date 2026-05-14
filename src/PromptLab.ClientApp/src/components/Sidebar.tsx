import { Link, useLocation } from 'react-router-dom';
import { FileText, BarChart3, ChevronRight, X } from 'lucide-react';

type SidebarProps = {
  mobileOpen?: boolean;
  onMobileClose?: () => void;
};

export function Sidebar({ mobileOpen = false, onMobileClose }: SidebarProps) {
  const loc = useLocation();
  const promptsActive = loc.pathname.startsWith('/prompts') || loc.pathname === '/';
  const runsActive = loc.pathname.startsWith('/test-runs');

  function handleNavClick() {
    onMobileClose?.();
  }

  return (
    <aside
      id="app-sidebar"
      className={[
        'flex w-64 shrink-0 flex-col border-r border-gray-800 bg-gray-950',
        'fixed bottom-0 left-0 top-14 z-[60] min-h-0 shadow-2xl transition-transform duration-200 ease-out md:static md:top-auto md:z-auto md:min-h-screen md:translate-x-0 md:shadow-none',
        mobileOpen ? 'translate-x-0' : '-translate-x-full',
      ].join(' ')}
    >
      <div className="flex items-center justify-between gap-3 border-b border-gray-800 px-4 py-4 md:px-6 md:py-5">
        <div className="flex min-w-0 items-center gap-3">
          <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-blue-600">
            <FileText className="h-4 w-4 text-white" />
          </div>
          <div className="min-w-0">
            <h1 className="text-sm font-semibold tracking-wide text-white">PromptLab</h1>
            <p className="text-xs text-gray-500">Prompt Management</p>
          </div>
        </div>
        <button
          type="button"
          aria-label="Cerrar menú"
          className="inline-flex h-9 w-9 shrink-0 items-center justify-center rounded-lg text-gray-400 hover:bg-gray-800 hover:text-white md:hidden"
          onClick={onMobileClose}
        >
          <X className="h-5 w-5" aria-hidden />
        </button>
      </div>

      <nav className="flex-1 px-3 py-4 space-y-1">
        <Link
          to="/prompts"
          onClick={handleNavClick}
          className={`group flex w-full items-center justify-between rounded-lg px-3 py-2.5 text-left transition-all duration-150 ${
            promptsActive ? 'bg-blue-600 text-white' : 'text-gray-400 hover:bg-gray-800 hover:text-gray-100'
          }`}
        >
          <div className="flex items-center gap-3">
            <FileText className="w-4 h-4 flex-shrink-0" />
            <div>
              <div className="text-sm font-medium">Prompts</div>
              <div className={`text-xs mt-0.5 ${promptsActive ? 'text-blue-200' : 'text-gray-600'}`}>Gestionar prompts</div>
            </div>
          </div>
          {promptsActive && <ChevronRight className="w-3.5 h-3.5 opacity-60" />}
        </Link>

        <p className="px-3 text-xs text-gray-600 pt-2">Test Suites: icono en cada tarjeta de prompt</p>

        <Link
          to="/test-runs"
          onClick={handleNavClick}
          className={`group flex w-full items-center justify-between rounded-lg px-3 py-2.5 text-left transition-all duration-150 ${
            runsActive ? 'bg-blue-600 text-white' : 'text-gray-400 hover:bg-gray-800 hover:text-gray-100'
          }`}
        >
          <div className="flex items-center gap-3">
            <BarChart3 className="w-4 h-4 flex-shrink-0" />
            <div>
              <div className="text-sm font-medium">Test Analysis</div>
              <div className={`text-xs mt-0.5 ${runsActive ? 'text-blue-200' : 'text-gray-600'}`}>Ejecuciones</div>
            </div>
          </div>
        </Link>
      </nav>

      <div className="px-4 py-4 border-t border-gray-800">
        <div className="text-xs text-gray-600 text-center">ClientApp v1.0</div>
      </div>
    </aside>
  );
}

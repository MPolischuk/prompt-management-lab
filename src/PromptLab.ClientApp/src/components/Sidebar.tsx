import { Link, useLocation } from 'react-router-dom';
import { FileText, BarChart3, ChevronRight } from 'lucide-react';

export function Sidebar() {
  const loc = useLocation();
  const promptsActive = loc.pathname.startsWith('/prompts') || loc.pathname === '/';
  const runsActive = loc.pathname.startsWith('/test-runs');

  return (
    <aside className="w-64 bg-gray-950 border-r border-gray-800 flex flex-col min-h-screen">
      <div className="px-6 py-5 border-b border-gray-800">
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center">
            <FileText className="w-4 h-4 text-white" />
          </div>
          <div>
            <h1 className="text-white font-semibold text-sm tracking-wide">PromptLab</h1>
            <p className="text-gray-500 text-xs">Prompt Management</p>
          </div>
        </div>
      </div>

      <nav className="flex-1 px-3 py-4 space-y-1">
        <Link
          to="/prompts"
          className={`w-full flex items-center justify-between px-3 py-2.5 rounded-lg text-left transition-all duration-150 group ${
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
          className={`w-full flex items-center justify-between px-3 py-2.5 rounded-lg text-left transition-all duration-150 group ${
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

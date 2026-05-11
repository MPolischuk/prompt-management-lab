import { FileText, FlaskConical, BarChart3, ChevronRight } from 'lucide-react';
import type { View } from '../types';

interface SidebarProps {
  currentView: View;
  onNavigate: (view: View) => void;
}

const navItems = [
  { id: 'prompts' as View, label: 'Prompts', icon: FileText, description: 'Manage your prompts' },
  { id: 'test-suites' as View, label: 'Test Suites', icon: FlaskConical, description: 'Create & run tests' },
  { id: 'test-runs' as View, label: 'Test Analysis', icon: BarChart3, description: 'Analyze test runs' },
];

export function Sidebar({ currentView, onNavigate }: SidebarProps) {
  const activeBase = currentView.split('-')[0];

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
        {navItems.map((item) => {
          const Icon = item.icon;
          const isActive = currentView === item.id || activeBase === item.id.split('-')[0];
          const exactActive = currentView === item.id;

          return (
            <button
              key={item.id}
              onClick={() => onNavigate(item.id)}
              className={`w-full flex items-center justify-between px-3 py-2.5 rounded-lg text-left transition-all duration-150 group ${
                exactActive
                  ? 'bg-blue-600 text-white'
                  : isActive
                  ? 'bg-gray-800 text-gray-100'
                  : 'text-gray-400 hover:bg-gray-800 hover:text-gray-100'
              }`}
            >
              <div className="flex items-center gap-3">
                <Icon className={`w-4 h-4 flex-shrink-0 ${exactActive ? 'text-white' : isActive ? 'text-blue-400' : 'text-gray-500 group-hover:text-gray-300'}`} />
                <div>
                  <div className={`text-sm font-medium ${exactActive ? 'text-white' : ''}`}>{item.label}</div>
                  <div className={`text-xs mt-0.5 ${exactActive ? 'text-blue-200' : 'text-gray-600'}`}>{item.description}</div>
                </div>
              </div>
              {isActive && <ChevronRight className="w-3.5 h-3.5 opacity-60" />}
            </button>
          );
        })}
      </nav>

      <div className="px-4 py-4 border-t border-gray-800">
        <div className="text-xs text-gray-600 text-center">v1.0.0</div>
      </div>
    </aside>
  );
}

import { useState } from 'react';
import { Sidebar } from './components/Sidebar';
import { PromptsPage } from './pages/PromptsPage';
import { PromptFormPage } from './pages/PromptFormPage';
import { PromptDetailPage } from './pages/PromptDetailPage';
import { TestSuitesPage } from './pages/TestSuitesPage';
import { TestSuiteDetailPage } from './pages/TestSuiteDetailPage';
import { TestRunDetailPage } from './pages/TestRunDetailPage';
import { TestRunsPage } from './pages/TestRunsPage';
import type { View } from './types';

interface NavState {
  view: View;
  id?: string;
}

function App() {
  const [nav, setNav] = useState<NavState>({ view: 'prompts' });

  function navigate(view: View, id?: string) {
    setNav({ view, id });
  }

  function renderContent() {
    switch (nav.view) {
      case 'prompts':
        return <PromptsPage onNavigate={navigate} />;
      case 'prompt-create':
        return <PromptFormPage onNavigate={navigate} />;
      case 'prompt-edit':
        return <PromptFormPage promptId={nav.id} onNavigate={navigate} />;
      case 'prompt-detail':
        return nav.id ? <PromptDetailPage promptId={nav.id} onNavigate={navigate} /> : <PromptsPage onNavigate={navigate} />;
      case 'test-suites':
        return nav.id ? <TestSuitesPage promptId={nav.id} onNavigate={navigate} /> : <PromptsPage onNavigate={navigate} />;
      case 'test-suite-detail':
        return nav.id ? <TestSuiteDetailPage suiteId={nav.id} onNavigate={navigate} /> : <PromptsPage onNavigate={navigate} />;
      case 'test-runs':
        return <TestRunsPage onNavigate={navigate} />;
      case 'test-run-detail':
        return nav.id ? <TestRunDetailPage runId={nav.id} onNavigate={navigate} /> : <TestRunsPage onNavigate={navigate} />;
      default:
        return <PromptsPage onNavigate={navigate} />;
    }
  }

  return (
    <div className="flex min-h-screen bg-gray-950 text-white">
      <Sidebar currentView={nav.view} onNavigate={(view) => navigate(view)} />
      {renderContent()}
    </div>
  );
}

export default App;

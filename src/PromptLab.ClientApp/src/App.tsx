import { Navigate, Route, Routes } from 'react-router-dom';
import { AppLayout } from './components/AppLayout';
import { PromptsPage } from './pages/PromptsPage';
import { PromptFormPage } from './pages/PromptFormPage';
import { PromptDetailPage } from './pages/PromptDetailPage';
import { TestSuitesPage } from './pages/TestSuitesPage';
import { TestSuiteDetailPage } from './pages/TestSuiteDetailPage';
import { TestRunsPage } from './pages/TestRunsPage';
import { TestRunDetailPage } from './pages/TestRunDetailPage';

export default function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/" element={<Navigate to="/prompts" replace />} />
        <Route path="/prompts" element={<PromptsPage />} />
        <Route path="/prompts/new" element={<PromptFormPage />} />
        <Route path="/prompts/:id" element={<PromptDetailPage />} />
        <Route path="/prompts/:id/edit" element={<PromptFormPage />} />
        <Route path="/prompts/:promptId/test-suites" element={<TestSuitesPage />} />
        <Route path="/test-suites/:suiteId" element={<TestSuiteDetailPage />} />
        <Route path="/test-runs" element={<TestRunsPage />} />
        <Route path="/test-runs/:runId" element={<TestRunDetailPage />} />
      </Route>
    </Routes>
  );
}

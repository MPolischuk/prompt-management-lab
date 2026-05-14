import { render } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import type { ReactElement } from 'react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { PromptFormPage } from '../pages/PromptFormPage';

function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: { retry: false, staleTime: 0 },
      mutations: { retry: false },
    },
  });
}

export interface RenderWithProvidersOptions {
  /** Rutas iniciales del MemoryRouter (por defecto `['/']`). */
  initialEntries?: string[];
}

export function renderWithProviders(ui: ReactElement, options: RenderWithProvidersOptions = {}) {
  const client = createTestQueryClient();
  const initialEntries = options.initialEntries ?? ['/'];

  return render(
    <QueryClientProvider client={client}>
      <MemoryRouter initialEntries={initialEntries}>{ui}</MemoryRouter>
    </QueryClientProvider>
  );
}

/** PromptFormPage necesita `useParams` + rutas. */
export function renderPromptFormAt(path: string) {
  const client = createTestQueryClient();
  return render(
    <QueryClientProvider client={client}>
      <MemoryRouter initialEntries={[path]}>
        <Routes>
          <Route path="/prompts/new" element={<PromptFormPage />} />
          <Route path="/prompts/:id/edit" element={<PromptFormPage />} />
        </Routes>
      </MemoryRouter>
    </QueryClientProvider>
  );
}

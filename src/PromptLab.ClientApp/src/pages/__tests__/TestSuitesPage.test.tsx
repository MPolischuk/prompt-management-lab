import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import * as api from '../../lib/api';
import { TestSuitesPage } from '../TestSuitesPage';
import { resetAllStores, setSuitesStateForTest } from '../../test/msw/handlers';
import { server } from '../../test/msw/server';

function renderPage(promptId: string) {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false, staleTime: 0 } } });
  return render(
    <QueryClientProvider client={client}>
      <MemoryRouter initialEntries={[`/prompts/${promptId}/test-suites`]}>
        <Routes>
          <Route path="/prompts/:promptId/test-suites" element={<TestSuitesPage />} />
        </Routes>
      </MemoryRouter>
    </QueryClientProvider>
  );
}

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => {
  server.resetHandlers();
  resetAllStores();
  vi.restoreAllMocks();
});
afterAll(() => server.close());

describe('TestSuitesPage', () => {
  it('lista suites con enlace al detalle', async () => {
    renderPage('prompt-1');
    expect(await screen.findByText('Suite A')).toBeInTheDocument();
    const link = screen.getByRole('link', { name: /Suite A/i });
    expect(link).toHaveAttribute('href', '/test-suites/suite-1');
  });

  it('estado vacío', async () => {
    setSuitesStateForTest([]);
    renderPage('prompt-1');
    await screen.findByText(/No hay suites/);
  });

  it('crear suite con nombre vacío no llama API', async () => {
    const user = userEvent.setup();
    const spy = vi.spyOn(api, 'createTestSuite');
    renderPage('prompt-1');
    await screen.findByText('Suite A');
    await user.click(screen.getByRole('button', { name: /Nueva suite/i }));
    await user.click(screen.getByRole('button', { name: /^Crear$/i }));
    expect(spy).not.toHaveBeenCalled();
  });

  it('crear suite válida cierra modal y refresca', async () => {
    const user = userEvent.setup();
    setSuitesStateForTest([]);
    renderPage('prompt-1');
    await screen.findByText(/No hay suites/);
    await user.click(screen.getByRole('button', { name: /Nueva suite/i }));
    await user.type(screen.getByPlaceholderText('Nombre'), 'Mi suite');
    await user.click(screen.getByRole('button', { name: /^Crear$/i }));
    await waitFor(() => {
      expect(screen.queryByPlaceholderText('Nombre')).not.toBeInTheDocument();
    });
    expect(await screen.findByText('Mi suite')).toBeInTheDocument();
  });

  it('eliminar con confirm', async () => {
    const user = userEvent.setup();
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const spy = vi.spyOn(api, 'deleteTestSuite').mockResolvedValue();
    renderPage('prompt-1');
    await screen.findByText('Suite A');
    await user.click(screen.getByRole('button', { name: /^Eliminar$/i }));
    expect(spy).toHaveBeenCalledWith('suite-1');
  });
});

import { afterAll, afterEach, beforeAll, describe, expect, it } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { TestRunDetailPage } from '../TestRunDetailPage';
import { resetAllStores } from '../../test/msw/handlers';
import { server } from '../../test/msw/server';

function renderRun(runId: string) {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false, staleTime: 0 } } });
  return render(
    <QueryClientProvider client={client}>
      <MemoryRouter initialEntries={[`/test-runs/${runId}`]}>
        <Routes>
          <Route path="/test-runs/:runId" element={<TestRunDetailPage />} />
        </Routes>
      </MemoryRouter>
    </QueryClientProvider>
  );
}

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => {
  server.resetHandlers();
  resetAllStores();
});
afterAll(() => server.close());

describe('TestRunDetailPage', () => {
  it('spinner y métricas para run-1', async () => {
    renderRun('run-1');
    expect(document.querySelector('.animate-spin')).toBeTruthy();
    await waitFor(() => expect(screen.getByText('50%')).toBeInTheDocument());
    expect(screen.getByText(/150 ms/)).toBeInTheDocument();
    expect(screen.getByText('65.0')).toBeInTheDocument();
  });

  it('PASS y FAIL en resultados', async () => {
    renderRun('run-1');
    await screen.findByText('PASS');
    expect(screen.getByText('FAIL')).toBeInTheDocument();
  });

  it('muestra error cuando el resultado tiene error', async () => {
    renderRun('run-2');
    await screen.findByText('Error de aserción');
  });

  it('Run no encontrado', async () => {
    renderRun('run-inexistente');
    expect(await screen.findByText('Run no encontrado')).toBeInTheDocument();
  });
});

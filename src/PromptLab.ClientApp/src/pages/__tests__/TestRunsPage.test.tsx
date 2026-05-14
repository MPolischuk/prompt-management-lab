import { afterAll, afterEach, beforeAll, describe, expect, it } from 'vitest';
import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { TestRunsPage } from '../TestRunsPage';
import { API_BASE, resetAllStores } from '../../test/msw/handlers';
import { server } from '../../test/msw/server';
import { http, HttpResponse } from 'msw';

function renderRuns() {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false, staleTime: 0 } } });
  return render(
    <QueryClientProvider client={client}>
      <MemoryRouter initialEntries={['/test-runs']}>
        <Routes>
          <Route path="/test-runs" element={<TestRunsPage />} />
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

describe('TestRunsPage', () => {
  it('muestra KPIs correctos', async () => {
    renderRuns();
    await screen.findByText('Test Runs');
    await waitFor(() => {
      expect(screen.getByText('Total').parentElement?.firstElementChild?.textContent).toBe('4');
    });
    expect(screen.getByText('Completados').parentElement?.firstElementChild?.textContent).toBe('1');
    expect(screen.getByText('Fallidos').parentElement?.firstElementChild?.textContent).toBe('1');
    expect(screen.getByText('Éxito').parentElement?.firstElementChild?.textContent).toBe('25%');
  });

  it('filtra por estado completed', async () => {
    const user = userEvent.setup();
    renderRuns();
    await screen.findByText('Test Runs');
    await user.click(screen.getByRole('button', { name: 'completed' }));
    await waitFor(() => {
      const runLinks = screen.getAllByRole('link').filter((l) => l.getAttribute('href')?.startsWith('/test-runs/'));
      expect(runLinks).toHaveLength(1);
    });
  });

  it('isLoading muestra spinner hasta resolver', async () => {
    let resolve!: (r: Response) => void;
    server.use(
      http.get(`${API_BASE}/api/TestRuns`, () => new Promise<Response>((res) => { resolve = res; }))
    );
    renderRuns();
    await waitFor(() => expect(document.querySelector('.animate-spin')).toBeTruthy());
    resolve!(HttpResponse.json([]));
    await waitFor(() => expect(document.querySelector('.animate-spin')).toBeFalsy());
  });

  it('badges de estado en lista', async () => {
    renderRuns();
    await screen.findByText('Test Runs');
    await waitFor(() => {
      const runLink = screen.getAllByRole('link').find((l) => l.getAttribute('href') === '/test-runs/run-1');
      expect(runLink).toBeTruthy();
    });
    const runLink = screen.getAllByRole('link').find((l) => l.getAttribute('href') === '/test-runs/run-1');
    const status = within(runLink!).getByText('completed');
    expect(status.className).toMatch(/emerald|success/);
  });
});

import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import * as api from '../../lib/api';
import { TestSuiteDetailPage } from '../TestSuiteDetailPage';
import { resetAllStores } from '../../test/msw/handlers';
import { server } from '../../test/msw/server';

function renderSuite(suiteId: string) {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false, staleTime: 0 } } });
  return render(
    <QueryClientProvider client={client}>
      <MemoryRouter initialEntries={[`/test-suites/${suiteId}`]}>
        <Routes>
          <Route path="/test-suites/:suiteId" element={<TestSuiteDetailPage />} />
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

describe('TestSuiteDetailPage', () => {
  it('Suite no encontrada', async () => {
    renderSuite('suite-inexistente');
    expect(await screen.findByText('Suite no encontrada')).toBeInTheDocument();
  });

  it('lista casos con JSON de variables', async () => {
    renderSuite('suite-1');
    await screen.findByRole('heading', { name: 'Suite A', level: 2 });
    expect(screen.getByText('Case uno')).toBeInTheDocument();
    expect(screen.getByText(/"nombre"/)).toBeInTheDocument();
  });

  it('modal crear: validación nombre obligatorio', async () => {
    const user = userEvent.setup();
    renderSuite('suite-1');
    await screen.findByRole('heading', { name: 'Suite A', level: 2 });
    await user.click(screen.getByRole('button', { name: /Caso/i }));
    await user.click(screen.getByRole('button', { name: /^Guardar$/i }));
    expect(await screen.findByText('Nombre obligatorio')).toBeInTheDocument();
  });

  it('modal crear: guarda caso nuevo', async () => {
    const user = userEvent.setup();
    const spy = vi.spyOn(api, 'createTestCase');
    renderSuite('suite-1');
    await screen.findByRole('heading', { name: 'Suite A', level: 2 });
    await user.click(screen.getByRole('button', { name: /Caso/i }));
    await user.type(screen.getByPlaceholderText('Nombre del caso'), 'Caso nuevo');
    const label = screen.getByText('{{nombre}}');
    const varInput = label.parentElement?.querySelector('input');
    expect(varInput).toBeTruthy();
    await user.type(varInput!, 'valor');
    await user.click(screen.getByRole('button', { name: /^Guardar$/i }));
    await waitFor(() => expect(spy).toHaveBeenCalled());
    await waitFor(() => expect(screen.queryByRole('dialog')).not.toBeInTheDocument());
  });

  it('eliminar caso con confirm', async () => {
    const user = userEvent.setup();
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const spy = vi.spyOn(api, 'deleteTestCase').mockResolvedValue();
    renderSuite('suite-1');
    await screen.findByText('Case uno');
    const card = screen.getByText('Case uno').closest('.bg-gray-900');
    expect(card).toBeTruthy();
    const buttons = within(card as HTMLElement).getAllByRole('button');
    expect(buttons.length).toBeGreaterThanOrEqual(2);
    await user.click(buttons[1]!);
    expect(spy).toHaveBeenCalledWith('case-1');
  });
});

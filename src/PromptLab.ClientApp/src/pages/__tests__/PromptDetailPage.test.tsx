import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import * as api from '../../lib/api';
import { PromptDetailPage } from '../PromptDetailPage';
import { resetAllStores } from '../../test/msw/handlers';
import { server } from '../../test/msw/server';

function renderDetail(initialPath: string) {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false, staleTime: 0 } } });
  return render(
    <QueryClientProvider client={client}>
      <MemoryRouter initialEntries={[initialPath]}>
        <Routes>
          <Route path="/prompts/:id" element={<PromptDetailPage />} />
          <Route path="/prompts" element={<div data-testid="list-page">list</div>} />
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
  vi.unstubAllGlobals();
});
afterAll(() => server.close());

describe('PromptDetailPage', () => {
  it('muestra spinner y luego el detalle', async () => {
    renderDetail('/prompts/prompt-1');
    expect(document.querySelector('.animate-spin')).toBeTruthy();
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Prompt uno' })).toBeInTheDocument();
    });
    expect(screen.getByText(/Contenido/i)).toBeInTheDocument();
  });

  it('muestra variables del contenido', async () => {
    renderDetail('/prompts/prompt-1');
    await screen.findByRole('heading', { name: 'Prompt uno' });
    expect(screen.getByText('{{nombre}}')).toBeInTheDocument();
  });

  it('copiar muestra Copiado al pulsar el botón', async () => {
    const user = userEvent.setup();
    renderDetail('/prompts/prompt-1');
    await screen.findByRole('heading', { name: 'Prompt uno' });
    await user.click(screen.getByRole('button', { name: /Copiar/i }));
    expect(screen.getByText('Copiado')).toBeInTheDocument();
  });

  it('toggle historial de versiones', async () => {
    const user = userEvent.setup();
    renderDetail('/prompts/prompt-1');
    await screen.findByRole('heading', { name: 'Prompt uno' });
    const btn = screen.getByRole('button', { name: /Historial de versiones/i });
    await user.click(btn);
    const list = screen.getByRole('list');
    expect(within(list).getByText(/v1/)).toBeInTheDocument();
    await user.click(btn);
    expect(screen.queryByRole('list')).not.toBeInTheDocument();
  });

  it('eliminar con confirm navega a /prompts', async () => {
    const user = userEvent.setup();
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const del = vi.spyOn(api, 'deletePrompt').mockResolvedValue();
    renderDetail('/prompts/prompt-1');
    await screen.findByRole('heading', { name: 'Prompt uno' });
    const trash = screen.getAllByRole('button').find((b) => b.querySelector('.lucide-trash2'));
    expect(trash).toBeTruthy();
    await user.click(trash!);
    expect(del).toHaveBeenCalledWith('prompt-1');
    await waitFor(() => expect(screen.getByTestId('list-page')).toBeInTheDocument());
  });

  it('No encontrado cuando el prompt no existe', async () => {
    renderDetail('/prompts/no-existe');
    expect(await screen.findByText('No encontrado')).toBeInTheDocument();
  });
});

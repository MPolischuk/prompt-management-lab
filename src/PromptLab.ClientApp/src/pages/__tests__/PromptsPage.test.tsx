import { http, HttpResponse } from 'msw';
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from 'vitest';
import { screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Route, Routes } from 'react-router-dom';
import * as api from '../../lib/api';
import { PromptsPage } from '../PromptsPage';
import { resetAllStores, setPromptsStateForTest, API_BASE } from '../../test/msw/handlers';
import { renderWithProviders } from '../../test/helpers';
import { server } from '../../test/msw/server';

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => {
  server.resetHandlers();
  resetAllStores();
  vi.restoreAllMocks();
});
afterAll(() => server.close());

function renderPromptsRoute() {
  return renderWithProviders(
    <Routes>
      <Route path="/prompts" element={<PromptsPage />} />
    </Routes>,
    { initialEntries: ['/prompts'] }
  );
}

describe('PromptsPage', () => {
  it('muestra skeleton y luego las tarjetas de prompts', async () => {
    renderPromptsRoute();

    expect(document.querySelector('.animate-pulse')).toBeTruthy();

    await waitFor(() => {
      expect(screen.getByText('Prompt uno')).toBeInTheDocument();
    });
    expect(screen.getByText('Prompt dos')).toBeInTheDocument();
  });

  it('muestra estado vacío cuando no hay prompts', async () => {
    setPromptsStateForTest([]);
    renderPromptsRoute();

    await waitFor(() => {
      expect(screen.getByText('No hay prompts')).toBeInTheDocument();
    });
    expect(screen.getByText('Crear el primero')).toBeInTheDocument();
  });

  it('la búsqueda filtra por texto', async () => {
    const user = userEvent.setup();
    renderPromptsRoute();

    await screen.findByText('Prompt uno');

    const searchInput = screen.getByPlaceholderText('Buscar...');
    await user.type(searchInput, 'dos');

    await waitFor(() => {
      expect(screen.queryByText('Prompt uno')).not.toBeInTheDocument();
    });
    expect(screen.getByText('Prompt dos')).toBeInTheDocument();
  });

  it('el filtro por tag activa el estilo del botón seleccionado', async () => {
    const user = userEvent.setup();
    renderPromptsRoute();

    const alpha = await screen.findByRole('button', { name: 'Alpha' });
    expect(alpha.className).not.toContain('bg-blue-600 text-white');

    await user.click(alpha);
    expect(alpha.className).toContain('bg-blue-600');
  });

  it('eliminar pide confirmación y llama deletePrompt', async () => {
    const user = userEvent.setup();
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const deleteSpy = vi.spyOn(api, 'deletePrompt').mockResolvedValue();

    renderPromptsRoute();

    const card = await screen.findByTestId('prompt-card-prompt-1');

    await user.hover(card);
    const deleteBtn = within(card).getByRole('button');
    await user.click(deleteBtn);

    expect(window.confirm).toHaveBeenCalled();
    expect(deleteSpy).toHaveBeenCalledWith('prompt-1');
  });

  it('paginación: totalRows reflejado en el subtítulo', async () => {
    server.use(
      http.get(`${API_BASE}/api/Prompts`, ({ request }) => {
        const url = new URL(request.url);
        const pageNumber = Number(url.searchParams.get('pageNumber') ?? '1');
        const pageSize = Number(url.searchParams.get('pageSize') ?? '12');
        const items = Array.from({ length: pageSize }, (_, i) => ({
          id: `p-${pageNumber}-${i}`,
          title: `Item ${pageNumber}-${i}`,
          description: null,
          content: 'c',
          category: null,
          language: null,
          modelHint: null,
          targetModelId: null,
          temperature: null,
          maxTokens: null,
          topP: null,
          version: 1,
          isActive: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
          tags: [],
          tagSummaries: [] as { id: string; name: string; slug: string }[],
        }));
        return HttpResponse.json({
          items,
          pageNumber,
          pageSize,
          totalRows: 30,
        });
      })
    );

    renderPromptsRoute();

    await waitFor(() => {
      expect(screen.getByText(/30 prompts/)).toBeInTheDocument();
    });
  });
});

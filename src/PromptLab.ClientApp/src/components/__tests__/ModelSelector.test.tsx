import { http, HttpResponse } from 'msw';
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ModelSelector } from '../ModelSelector';
import { server } from '../../test/msw/server';
import { API_BASE, defaultAiModels, resetAllStores } from '../../test/msw/handlers';

function renderModelSelector(onChange = vi.fn()) {
  const client = new QueryClient({
    defaultOptions: { queries: { retry: false, staleTime: 0 }, mutations: { retry: false } },
  });
  return render(
    <QueryClientProvider client={client}>
      <ModelSelector value="" onChange={onChange} />
    </QueryClientProvider>
  );
}

describe('ModelSelector', () => {
  beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
  afterEach(() => {
    server.resetHandlers();
    resetAllStores();
  });
  afterAll(() => server.close());

  it('muestra skeleton mientras carga modelos', async () => {
    server.use(
      http.get(`${API_BASE}/api/ai-models`, async () => {
        await new Promise((r) => setTimeout(r, 300));
        return HttpResponse.json(defaultAiModels);
      })
    );
    renderModelSelector();
    expect(document.querySelector('.animate-pulse')).toBeTruthy();
    await waitFor(() => expect(screen.getByRole('combobox')).toBeInTheDocument(), { timeout: 5000 });
  });

  it('lista modelos habilitados y llama onChange', async () => {
    const user = userEvent.setup();
    const onChange = vi.fn();
    renderModelSelector(onChange);
    await screen.findByRole('combobox');
    await user.selectOptions(screen.getByRole('combobox'), 'model-1');
    expect(onChange).toHaveBeenCalledWith('model-1');
    expect(screen.queryByText(/Disabled/)).not.toBeInTheDocument();
  });
});

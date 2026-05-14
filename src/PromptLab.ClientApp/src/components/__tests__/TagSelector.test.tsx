import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { TagSelector } from '../TagSelector';
import { server } from '../../test/msw/server';
import { resetAllStores } from '../../test/msw/handlers';

function renderTagSelector(initial: string[] = []) {
  const onChange = vi.fn();
  const client = new QueryClient({
    defaultOptions: { queries: { retry: false, staleTime: 0 }, mutations: { retry: false } },
  });
  const r = render(
    <QueryClientProvider client={client}>
      <TagSelector selectedIds={initial} onChange={onChange} />
    </QueryClientProvider>
  );
  return { ...r, onChange };
}

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => {
  server.resetHandlers();
  resetAllStores();
});
afterAll(() => server.close());

describe('TagSelector', () => {
  it('muestra tags disponibles', async () => {
    renderTagSelector();
    expect(await screen.findByRole('button', { name: 'Alpha' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Beta' })).toBeInTheDocument();
  });

  it('al seleccionar un tag llama onChange con el id agregado', async () => {
    const user = userEvent.setup();
    const { onChange } = renderTagSelector([]);
    await screen.findByRole('button', { name: 'Beta' });
    await user.click(screen.getByRole('button', { name: 'Beta' }));
    expect(onChange).toHaveBeenCalledWith(['tag-2']);
  });

  it('al quitar chip seleccionado llama onChange sin ese id', async () => {
    const user = userEvent.setup();
    const { onChange } = renderTagSelector(['tag-1']);
    await screen.findByRole('button', { name: 'Alpha' });
    await user.click(screen.getByRole('button', { name: 'Alpha' }));
    expect(onChange).toHaveBeenCalledWith([]);
  });

  it('filtra por búsqueda', async () => {
    const user = userEvent.setup();
    renderTagSelector();
    const search = await screen.findByPlaceholderText('Buscar tags...');
    await user.type(search, 'bet');
    await waitFor(() => {
      expect(screen.queryByRole('button', { name: 'Alpha' })).not.toBeInTheDocument();
    });
    expect(screen.getByRole('button', { name: 'Beta' })).toBeInTheDocument();
  });

  it('crear tag vacío no llama onChange', async () => {
    const user = userEvent.setup();
    const { onChange } = renderTagSelector([]);
    await screen.findByPlaceholderText('Nuevo tag');
    const row = screen.getByPlaceholderText('Nuevo tag').parentElement;
    const plusBtn = row?.querySelector('button[type="button"]');
    expect(plusBtn).toBeTruthy();
    await user.click(plusBtn!);
    expect(onChange).not.toHaveBeenCalled();
  });

  it('crear tag con nombre llama onChange con nuevo id', async () => {
    const user = userEvent.setup();
    const { onChange } = renderTagSelector([]);
    await screen.findByPlaceholderText('Nuevo tag');
    await user.type(screen.getByPlaceholderText('Nuevo tag'), 'Gamma');
    const row = screen.getByPlaceholderText('Nuevo tag').parentElement;
    const plusBtn = row?.querySelector('button[type="button"]');
    await user.click(plusBtn!);
    await waitFor(() => {
      expect(onChange).toHaveBeenCalled();
    });
    const arg = onChange.mock.calls[0]![0] as string[];
    expect(arg.some((id) => id.startsWith('tag-new-'))).toBe(true);
  });
});

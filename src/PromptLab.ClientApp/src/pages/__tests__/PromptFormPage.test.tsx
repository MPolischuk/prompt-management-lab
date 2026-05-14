import { useParams } from 'react-router-dom';
import { afterAll, afterEach, beforeAll, describe, expect, it, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { PromptFormPage } from '../PromptFormPage';
import { resetPromptsState } from '../../test/msw/handlers';
import { server } from '../../test/msw/server';

function fieldTitle() {
  return screen.getAllByRole('textbox')[0]!;
}

function fieldContent() {
  return screen.getAllByRole('textbox')[2]!;
}

function createClient() {
  return new QueryClient({
    defaultOptions: {
      queries: { retry: false, staleTime: 0 },
      mutations: { retry: false },
    },
  });
}

function PromptIdEcho() {
  const { id } = useParams();
  return <div data-testid="after-save">prompt:{id}</div>;
}

function renderFormAt(initialPath: string) {
  const client = createClient();
  return render(
    <QueryClientProvider client={client}>
      <MemoryRouter initialEntries={[initialPath]}>
        <Routes>
          <Route path="/prompts/new" element={<PromptFormPage />} />
          <Route path="/prompts/:id/edit" element={<PromptFormPage />} />
          <Route path="/prompts/:id" element={<PromptIdEcho />} />
        </Routes>
      </MemoryRouter>
    </QueryClientProvider>
  );
}

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => {
  server.resetHandlers();
  resetPromptsState();
  vi.restoreAllMocks();
});
afterAll(() => server.close());

describe('PromptFormPage', () => {
  it('modo nuevo: título Nuevo prompt y campos vacíos', async () => {
    renderFormAt('/prompts/new');

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Nuevo prompt' })).toBeInTheDocument();
    });
    expect(fieldTitle()).toHaveValue('');
    expect(fieldContent()).toHaveValue('');
  });

  it('modo editar: precarga datos del prompt', async () => {
    renderFormAt('/prompts/prompt-1/edit');

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Editar prompt' })).toBeInTheDocument();
    });
    expect(await screen.findByDisplayValue('Prompt uno')).toBeInTheDocument();
    expect(screen.getByDisplayValue('Contenido del prompt')).toBeInTheDocument();
  });

  it('validación: título obligatorio', async () => {
    const user = userEvent.setup();
    renderFormAt('/prompts/new');

    await screen.findByRole('heading', { name: 'Nuevo prompt' });

    await user.click(screen.getByRole('button', { name: /Guardar/i }));
    expect(await screen.findByText('El título es obligatorio')).toBeInTheDocument();
  });

  it('validación: contenido obligatorio', async () => {
    const user = userEvent.setup();
    renderFormAt('/prompts/new');

    await screen.findByRole('heading', { name: 'Nuevo prompt' });

    await user.type(fieldTitle(), 'Solo título');
    await user.click(screen.getByRole('button', { name: /Guardar/i }));

    expect(await screen.findByText('El contenido es obligatorio')).toBeInTheDocument();
  });

  it('crear: guarda y navega al detalle del prompt', async () => {
    const user = userEvent.setup();
    renderFormAt('/prompts/new');

    await screen.findByRole('heading', { name: 'Nuevo prompt' });

    await user.type(fieldTitle(), 'Mi nuevo');
    await user.type(fieldContent(), 'Contenido largo');

    await user.click(screen.getByRole('button', { name: /^Guardar$/i }));

    await waitFor(() => {
      expect(screen.getByTestId('after-save')).toHaveTextContent(/^prompt:/);
    });
    expect(screen.getByTestId('after-save').textContent).toMatch(/^prompt:prompt-/);
  });

  it('editar: guarda y navega al detalle', async () => {
    const user = userEvent.setup();
    renderFormAt('/prompts/prompt-1/edit');

    await screen.findByDisplayValue('Prompt uno');

    await user.clear(fieldTitle());
    await user.type(fieldTitle(), 'Título actualizado');
    await user.click(screen.getByRole('button', { name: /^Guardar$/i }));

    await waitFor(() => {
      expect(screen.getByTestId('after-save')).toHaveTextContent('prompt:prompt-1');
    });
  });
});

import { describe, expect, it } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { AppLayout } from '../AppLayout';

function ChildPrompts() {
  return <div data-testid="child-prompts">prompts</div>;
}
function ChildRuns() {
  return <div data-testid="child-runs">runs</div>;
}

function renderLayout(initialPath: string) {
  return render(
    <MemoryRouter initialEntries={[initialPath]}>
      <Routes>
        <Route element={<AppLayout />}>
          <Route path="/prompts" element={<ChildPrompts />} />
          <Route path="/test-runs" element={<ChildRuns />} />
        </Route>
      </Routes>
    </MemoryRouter>
  );
}

describe('AppLayout', () => {
  it('renderiza Outlet con la ruta hija', () => {
    renderLayout('/prompts');
    expect(screen.getByTestId('child-prompts')).toBeInTheDocument();
  });

  it('abre y cierra el menú móvil con el botón hamburguesa y el overlay', async () => {
    const user = userEvent.setup();
    renderLayout('/prompts');

    const hamburger = screen.getByRole('button', { expanded: false });
    await user.click(hamburger);
    expect(hamburger).toHaveAttribute('aria-expanded', 'true');

    const overlays = screen.getAllByRole('button', { name: 'Cerrar menú' });
    expect(overlays.length).toBeGreaterThanOrEqual(1);
    await user.click(overlays[0]!);
    expect(hamburger).toHaveAttribute('aria-expanded', 'false');
  });

  it('cierra el menú al navegar a otra ruta', async () => {
    const user = userEvent.setup();
    renderLayout('/prompts');

    const hamburger = screen.getByRole('button', { expanded: false });
    await user.click(hamburger);
    expect(hamburger).toHaveAttribute('aria-expanded', 'true');

    await user.click(screen.getByRole('link', { name: /Test Analysis/i }));
    expect(screen.getByTestId('child-runs')).toBeInTheDocument();
    await waitFor(() => expect(hamburger).toHaveAttribute('aria-expanded', 'false'));
  });
});

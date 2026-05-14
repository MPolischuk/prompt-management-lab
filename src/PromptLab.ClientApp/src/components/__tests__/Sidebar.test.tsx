import { describe, expect, it, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { Sidebar } from '../Sidebar';

describe('Sidebar', () => {
  it('resalta Prompts en /prompts', () => {
    render(
      <MemoryRouter initialEntries={['/prompts']}>
        <Routes>
          <Route path="/prompts" element={<Sidebar />} />
        </Routes>
      </MemoryRouter>
    );
    const link = screen.getByRole('link', { name: /Prompts/i });
    expect(link.className).toContain('bg-blue-600');
  });

  it('resalta Test Analysis en /test-runs', () => {
    render(
      <MemoryRouter initialEntries={['/test-runs']}>
        <Routes>
          <Route path="/test-runs" element={<Sidebar />} />
        </Routes>
      </MemoryRouter>
    );
    const link = screen.getByRole('link', { name: /Test Analysis/i });
    expect(link.className).toContain('bg-blue-600');
  });

  it('mobileOpen aplica translate-x-0 al aside', () => {
    const { container } = render(
      <MemoryRouter>
        <Sidebar mobileOpen />
      </MemoryRouter>
    );
    const aside = container.querySelector('aside');
    expect(aside?.className).toContain('translate-x-0');
  });

  it('onMobileClose al pulsar el botón cerrar (móvil)', async () => {
    const user = userEvent.setup();
    const onMobileClose = vi.fn();
    render(
      <MemoryRouter>
        <Sidebar mobileOpen onMobileClose={onMobileClose} />
      </MemoryRouter>
    );
    await user.click(screen.getByRole('button', { name: 'Cerrar menú' }));
    expect(onMobileClose).toHaveBeenCalled();
  });
});

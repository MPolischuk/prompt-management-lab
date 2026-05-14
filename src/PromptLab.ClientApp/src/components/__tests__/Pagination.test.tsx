import { describe, expect, it, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Pagination } from '../Pagination';

describe('Pagination', () => {
  it('retorna null cuando hay una sola página', () => {
    const { container } = render(
      <Pagination pageNumber={1} pageSize={12} totalRows={10} onPageChange={() => {}} />
    );
    expect(container.firstChild).toBeNull();
  });

  it('muestra página actual y total', () => {
    render(<Pagination pageNumber={2} pageSize={10} totalRows={25} onPageChange={() => {}} />);
    expect(screen.getByText('Página 2 de 3')).toBeInTheDocument();
  });

  it('deshabilita Anterior en la primera página', () => {
    render(<Pagination pageNumber={1} pageSize={10} totalRows={30} onPageChange={() => {}} />);
    expect(screen.getByRole('button', { name: 'Anterior' })).toBeDisabled();
    expect(screen.getByRole('button', { name: 'Siguiente' })).not.toBeDisabled();
  });

  it('deshabilita Siguiente en la última página', () => {
    render(<Pagination pageNumber={3} pageSize={10} totalRows={30} onPageChange={() => {}} />);
    expect(screen.getByRole('button', { name: 'Siguiente' })).toBeDisabled();
    expect(screen.getByRole('button', { name: 'Anterior' })).not.toBeDisabled();
  });

  it('llama onPageChange al navegar', async () => {
    const user = userEvent.setup();
    const onPageChange = vi.fn();
    render(<Pagination pageNumber={2} pageSize={10} totalRows={30} onPageChange={onPageChange} />);

    await user.click(screen.getByRole('button', { name: 'Anterior' }));
    expect(onPageChange).toHaveBeenCalledWith(1);

    await user.click(screen.getByRole('button', { name: 'Siguiente' }));
    expect(onPageChange).toHaveBeenCalledWith(3);
  });
});

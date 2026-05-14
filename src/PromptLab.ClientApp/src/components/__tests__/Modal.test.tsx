import { describe, expect, it, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Modal } from '../Modal';

describe('Modal', () => {
  it('no renderiza contenido cuando está cerrado', () => {
    const { container } = render(
      <Modal isOpen={false} onClose={() => {}} title="T">
        <p>Contenido</p>
      </Modal>
    );
    expect(container.querySelector('[role="dialog"]')).toBeNull();
  });

  it('renderiza children y título cuando está abierto', () => {
    render(
      <Modal isOpen onClose={() => {}} title="Mi modal">
        <p>Hola modal</p>
      </Modal>
    );
    expect(screen.getByRole('dialog')).toBeInTheDocument();
    expect(screen.getByText('Mi modal')).toBeInTheDocument();
    expect(screen.getByText('Hola modal')).toBeInTheDocument();
  });

  it('llama onClose al pulsar el backdrop', async () => {
    const user = userEvent.setup();
    const onClose = vi.fn();
    render(
      <Modal isOpen onClose={onClose} title="T">
        x
      </Modal>
    );

    await user.click(screen.getByRole('button', { name: 'Cerrar' }));
    expect(onClose).toHaveBeenCalledTimes(1);
  });
});

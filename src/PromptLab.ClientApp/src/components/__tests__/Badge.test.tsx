import { describe, expect, it } from 'vitest';
import { render, screen } from '@testing-library/react';
import { Badge } from '../Badge';

describe('Badge', () => {
  it('renderiza el texto hijo', () => {
    render(<Badge>Hola</Badge>);
    expect(screen.getByText('Hola')).toBeInTheDocument();
  });

  it.each([
    ['success', 'emerald'],
    ['error', 'red'],
    ['warning', 'amber'],
    ['info', 'sky'],
    ['neutral', 'gray-700'],
    ['default', 'blue'],
  ] as const)('variante %s incluye clase esperada', (variant, cls) => {
    const { container } = render(<Badge variant={variant}>x</Badge>);
    const span = container.querySelector('span');
    expect(span?.className).toContain(cls);
  });
});

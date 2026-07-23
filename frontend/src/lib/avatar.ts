import { createAvatar } from '@dicebear/core';
import { bottts } from '@dicebear/collection';

/** Gera o data URI (SVG) do avatar a partir de um seed. Offline. */
export function avatarDataUri(seed: string): string {
  return createAvatar(bottts, {
    seed,
    radius: 50,
    backgroundColor: ['transparent'],
  }).toDataUri();
}

/** Conjunto de seeds oferecidos na grade de escolha. */
export const SEEDS_AVATAR: string[] = [
  'Aneka', 'Bandit', 'Bella', 'Boots', 'Callie', 'Chester',
  'Cleo', 'Coco', 'Felix', 'Gizmo', 'Jasper', 'Loki',
  'Milo', 'Nala', 'Oscar', 'Pepper', 'Rocky', 'Simba',
];

/** Iniciais para o fallback quando não há avatar. */
export function iniciais(nome?: string): string {
  if (!nome) return '?';
  const partes = nome.trim().split(/\s+/);
  const primeira = partes[0]?.[0] ?? '';
  const ultima = partes.length > 1 ? partes[partes.length - 1][0] : '';
  return (primeira + ultima).toUpperCase();
}

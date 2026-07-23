import { avatarDataUri, iniciais } from '@/lib/avatar';
import { cn } from '@/lib/utils';

interface AvatarProps {
  seed?: string | null;
  nome?: string;
  className?: string;
}

/** Mostra o avatar escolhido (seed) ou as iniciais do nome como fallback. */
export function Avatar({ seed, nome, className }: AvatarProps) {
  if (seed) {
    return (
      <img
        src={avatarDataUri(seed)}
        alt={nome ?? 'Avatar'}
        className={cn('size-9 rounded-full bg-primary/10 object-cover', className)}
      />
    );
  }

  return (
    <span
      className={cn(
        'grid size-9 place-items-center rounded-full bg-primary/15 text-primary text-sm font-medium',
        className,
      )}
    >
      {iniciais(nome)}
    </span>
  );
}

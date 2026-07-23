import { cn } from '@/lib/utils';

/** Bloco de carregamento (placeholder animado) enquanto os dados chegam. */
function Skeleton({ className, ...props }: React.ComponentProps<'div'>) {
  return (
    <div
      data-slot="skeleton"
      className={cn('animate-pulse rounded-md bg-muted', className)}
      {...props}
    />
  );
}

export { Skeleton };

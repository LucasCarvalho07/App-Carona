import { cn } from '@/lib/utils';
import type { StatusPagamento } from '@/types';

const ESTILOS: Record<StatusPagamento, { label: string; classe: string }> = {
  Pendente: { label: 'Pendente', classe: 'bg-muted text-muted-foreground' },
  Informado: { label: 'Informado', classe: 'bg-amber-500/15 text-amber-600 dark:text-amber-400' },
  Confirmado: { label: 'Confirmado', classe: 'bg-emerald-500/15 text-emerald-600 dark:text-emerald-400' },
  Rejeitado: { label: 'Rejeitado', classe: 'bg-destructive/15 text-destructive' },
};

export function StatusPagamentoBadge({ status }: { status: StatusPagamento }) {
  const { label, classe } = ESTILOS[status];
  return (
    <span className={cn('rounded-full px-2 py-0.5 text-xs font-medium', classe)}>{label}</span>
  );
}

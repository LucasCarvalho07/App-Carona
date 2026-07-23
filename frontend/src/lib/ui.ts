export const inputClass =
  'w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground outline-none transition-colors hover:border-ring/60 focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50';

export const labelClass = 'text-xs font-medium text-muted-foreground';

export function anoMesAtual(): number {
  const d = new Date();
  return d.getFullYear() * 100 + (d.getMonth() + 1);
}

export function rotuloAnoMes(anoMes: number): string {
  const ano = Math.floor(anoMes / 100);
  const mes = anoMes % 100;
  return `${String(mes).padStart(2, '0')}/${ano}`;
}

export function formatarReal(valor: number): string {
  return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

import { useCallback, useEffect, useMemo, useState } from 'react';
import { CalendarCheck, TrendingUp, Users, Wallet } from 'lucide-react';
import type { LucideIcon } from 'lucide-react';
import { useAuth } from '@/lib/auth';
import { api } from '@/lib/api';
import { anoMesAtual, formatarReal, rotuloAnoMes } from '@/lib/ui';
import { StatusPagamentoBadge } from '@/components/StatusPagamentoBadge';
import { Skeleton } from '@/components/ui/skeleton';
import { StatusPagamento, type Marcacao, type PagamentoResumo } from '@/types';

interface Cartao {
  rotulo: string;
  valor: string;
  icon: LucideIcon;
}

export function Dashboard() {
  const { usuario, token } = useAuth();
  const t = token ?? undefined;

  const [marcacoes, setMarcacoes] = useState<Marcacao[]>([]);
  const [resumo, setResumo] = useState<PagamentoResumo[]>([]);
  const [carregando, setCarregando] = useState(true);

  const anoMes = anoMesAtual();

  const carregar = useCallback(async () => {
    setCarregando(true);
    try {
      const [marcs, res] = await Promise.all([
        api.get<Marcacao[]>(`/marcacao?anoMes=${anoMes}`, t),
        api.get<PagamentoResumo[]>(`/pagamentos/resumo?anoMes=${anoMes}`, t),
      ]);
      setMarcacoes(marcs);
      setResumo(res);
    } finally {
      setCarregando(false);
    }
  }, [t, anoMes]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const dias = useMemo(
    () => new Set(marcacoes.map((m) => m.data.slice(0, 10))).size,
    [marcacoes],
  );
  const totalMes = useMemo(
    () => marcacoes.reduce((s, m) => s + m.valorPorPessoa, 0),
    [marcacoes],
  );
  const pendente = useMemo(
    () => resumo.filter((r) => r.status !== StatusPagamento.Confirmado).reduce((s, r) => s + r.total, 0),
    [resumo],
  );
  const qtdMotoristas = useMemo(
    () => new Set(marcacoes.map((m) => m.motoristaId)).size,
    [marcacoes],
  );

  const menores: Cartao[] = [
    { rotulo: 'Dias no mês', valor: String(dias), icon: CalendarCheck },
    { rotulo: 'Pendente', valor: formatarReal(pendente), icon: Wallet },
    { rotulo: 'Motoristas', valor: String(qtdMotoristas), icon: Users },
  ];

  const semDados = !carregando && marcacoes.length === 0;

  return (
    <div className="space-y-6 animate-in fade-in duration-300">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Olá, {usuario?.nome}</h1>
        <p className="text-muted-foreground">Resumo de {rotuloAnoMes(anoMes)}.</p>
      </div>

      {/* Bento: tile de destaque (total) + tiles menores */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="col-span-2 relative overflow-hidden rounded-2xl bg-gradient-to-br from-primary to-primary/70 p-5 text-primary-foreground shadow-lg animate-in fade-in slide-in-from-bottom-2">
          <div className="pointer-events-none absolute -right-8 -top-10 size-40 rounded-full bg-white/10 blur-2xl" />
          <div className="flex items-center justify-between">
            <span className="text-sm font-medium opacity-90">Total no mês</span>
            <span className="rounded-xl bg-white/20 p-2">
              <TrendingUp className="size-4" />
            </span>
          </div>
          {carregando ? (
            <Skeleton className="mt-3 h-9 w-40 bg-white/30" />
          ) : (
            <p className="mt-3 text-3xl font-bold tabular-nums">{formatarReal(totalMes)}</p>
          )}
          <p className="mt-1 text-sm opacity-80">O que você deve somando todas as caronas.</p>
        </div>

        {menores.map((c) => (
          <div
            key={c.rotulo}
            className="rounded-2xl border bg-card p-4 space-y-2 shadow-sm transition-all duration-200 hover:shadow-md hover:-translate-y-0.5 animate-in fade-in slide-in-from-bottom-2"
          >
            <div className="flex items-center justify-between">
              <span className="text-sm text-muted-foreground">{c.rotulo}</span>
              <span className="rounded-xl bg-primary/10 text-primary p-2">
                <c.icon className="size-4" />
              </span>
            </div>
            {carregando ? (
              <Skeleton className="h-8 w-16" />
            ) : (
              <p className="text-2xl font-semibold tabular-nums">{c.valor}</p>
            )}
          </div>
        ))}
      </div>

      {carregando ? (
        <div className="rounded-2xl border bg-card p-4 sm:p-5 shadow-sm space-y-3">
          <Skeleton className="h-5 w-32" />
          {[0, 1, 2].map((i) => (
            <div key={i} className="flex items-center justify-between py-1">
              <div className="space-y-2">
                <Skeleton className="h-4 w-32" />
                <Skeleton className="h-3 w-20" />
              </div>
              <Skeleton className="h-5 w-16" />
            </div>
          ))}
        </div>
      ) : semDados ? (
        <div className="flex flex-col items-center gap-3 rounded-2xl border border-dashed bg-card/50 p-10 text-center shadow-sm">
          <span className="rounded-2xl bg-primary/10 text-primary p-3">
            <CalendarCheck className="size-6" />
          </span>
          <p className="font-medium">Nenhuma carona em {rotuloAnoMes(anoMes)}</p>
          <p className="text-sm text-muted-foreground">Marque sua presença na aba Marcação para começar.</p>
        </div>
      ) : (
        <div className="rounded-2xl border bg-card p-4 sm:p-5 shadow-sm space-y-1">
          <h2 className="font-semibold mb-2">Por motorista</h2>
          {resumo.length === 0 ? (
            <p className="text-sm text-muted-foreground py-2">
              Suas caronas ainda não geraram valores a pagar.
            </p>
          ) : (
            <ul className="divide-y divide-border/60">
              {resumo.map((r) => (
                <li key={r.motoristaId} className="flex items-center justify-between gap-3 py-3">
                  <div className="text-sm min-w-0">
                    <p className="font-medium truncate">{r.motoristaNome}</p>
                    <p className="text-muted-foreground">{r.qtdDias} dia(s)</p>
                  </div>
                  <div className="flex items-center gap-3 shrink-0">
                    <span className="font-semibold tabular-nums">{formatarReal(r.total)}</span>
                    <StatusPagamentoBadge status={r.status} />
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}
    </div>
  );
}

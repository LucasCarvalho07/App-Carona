import { useCallback, useEffect, useMemo, useState } from 'react';
import { BarChart3, CalendarDays, Car, Users } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { Avatar } from '@/components/Avatar';
import { EChart, coresGrafico, paletaGrafico } from '@/components/EChart';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { anoMesAtual, formatarReal, rotuloAnoMes } from '@/lib/ui';
import { useTheme } from '@/lib/theme';
import type { ResumoMensalMotorista } from '@/types';

export function Resumo() {
  const { token } = useAuth();
  const t = token ?? undefined;
  const { tema } = useTheme();

  const [anoMes, setAnoMes] = useState(anoMesAtual());
  const [dados, setDados] = useState<ResumoMensalMotorista[]>([]);
  const [carregando, setCarregando] = useState(true);

  const carregar = useCallback(async () => {
    setCarregando(true);
    try {
      setDados(await api.get<ResumoMensalMotorista[]>(`/pagamentos/resumo-mensal?anoMes=${anoMes}`, t));
    } finally {
      setCarregando(false);
    }
  }, [t, anoMes]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  function mudarMes(delta: number) {
    const ano = Math.floor(anoMes / 100);
    const mes = anoMes % 100;
    const d = new Date(ano, mes - 1 + delta, 1);
    setAnoMes(d.getFullYear() * 100 + (d.getMonth() + 1));
  }

  const totais = useMemo(() => {
    const totalMes = dados.reduce((s, m) => s + m.totalValor, 0);
    const viagens = dados.reduce((s, m) => s + m.qtdViagens, 0);
    const passageiros = new Set<number>();
    dados.forEach((m) => m.passageiros.forEach((p) => passageiros.add(p.passageiroId)));
    return { totalMes, viagens, motoristas: dados.length, passageiros: passageiros.size };
  }, [dados]);

  // Gráfico: total recebido por motorista (barra horizontal, maior no topo).
  const opcaoMotoristas = useMemo(() => {
    const c = coresGrafico();
    const ordenado = [...dados].sort((a, b) => b.totalValor - a.totalValor);
    return {
      color: paletaGrafico,
      grid: { left: 8, right: 12, top: 16, bottom: 8, containLabel: true },
      tooltip: {
        trigger: 'axis',
        axisPointer: { type: 'shadow' },
        valueFormatter: (v: number) => formatarReal(Number(v)),
      },
      xAxis: {
        type: 'category',
        data: ordenado.map((m) => m.motoristaNome),
        axisLabel: { color: c.texto, interval: 0, hideOverlap: true },
        axisLine: { lineStyle: { color: c.linha } },
        axisTick: { show: false },
      },
      yAxis: {
        type: 'value',
        axisLabel: { color: c.texto, formatter: (v: number) => formatarReal(Number(v)) },
        splitLine: { lineStyle: { color: c.linha } },
      },
      series: [
        {
          type: 'bar',
          colorBy: 'data',
          data: ordenado.map((m) => Number(m.totalValor.toFixed(2))),
          itemStyle: { borderRadius: [6, 6, 0, 0] },
          barMaxWidth: 48,
        },
      ],
    };
  }, [dados, tema]);

  // Gráfico pizza: valor gasto por passageiro no mês (viagens no tooltip).
  const opcaoPassageiros = useMemo(() => {
    const c = coresGrafico();
    const porPassageiro = new Map<number, { nome: string; valor: number; viagens: number }>();
    dados.forEach((m) =>
      m.passageiros.forEach((p) => {
        const atual = porPassageiro.get(p.passageiroId) ?? { nome: p.nome, valor: 0, viagens: 0 };
        atual.valor += p.valor;
        atual.viagens += p.qtdViagens;
        porPassageiro.set(p.passageiroId, atual);
      }),
    );
    const arr = [...porPassageiro.values()].sort((a, b) => b.valor - a.valor);
    return {
      color: paletaGrafico,
      tooltip: {
        trigger: 'item',
        formatter: (p: { name: string; value: number; percent: number; data: { viagens: number } }) =>
          `${p.name}<br/>${formatarReal(p.value)} · ${p.data.viagens} viagem(ns) (${p.percent}%)`,
      },
      legend: {
        type: 'scroll',
        bottom: 0,
        textStyle: { color: c.texto },
        pageTextStyle: { color: c.texto },
      },
      series: [
        {
          type: 'pie',
          radius: ['42%', '70%'],
          center: ['50%', '44%'],
          avoidLabelOverlap: true,
          itemStyle: { borderColor: c.fundoCard, borderWidth: 2, borderRadius: 4 },
          label: { color: c.texto, formatter: '{d}%' },
          data: arr.map((p) => ({ name: p.nome, value: Number(p.valor.toFixed(2)), viagens: p.viagens })),
        },
      ],
    };
  }, [dados, tema]);

  const vazio = !carregando && dados.length === 0;

  return (
    <div className="max-w-5xl mx-auto space-y-6 animate-in fade-in duration-300">
      <div className="flex items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Resumo do mês</h1>
          <p className="text-muted-foreground text-sm">Caronas por motorista, passageiros e valores.</p>
        </div>
        <div className="flex items-center gap-1 rounded-full border bg-card p-1 shadow-sm">
          <Button variant="ghost" size="icon" className="rounded-full" onClick={() => mudarMes(-1)}>‹</Button>
          <span className="text-sm font-medium w-16 text-center tabular-nums">{rotuloAnoMes(anoMes)}</span>
          <Button variant="ghost" size="icon" className="rounded-full" onClick={() => mudarMes(1)}>›</Button>
        </div>
      </div>

      {/* KPIs */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-3">
        <Kpi rotulo="Total no mês" valor={formatarReal(totais.totalMes)} destaque carregando={carregando} icone={<BarChart3 className="size-5" />} />
        <Kpi rotulo="Viagens" valor={String(totais.viagens)} carregando={carregando} icone={<CalendarDays className="size-5" />} />
        <Kpi rotulo="Motoristas" valor={String(totais.motoristas)} carregando={carregando} icone={<Car className="size-5" />} />
        <Kpi rotulo="Passageiros" valor={String(totais.passageiros)} carregando={carregando} icone={<Users className="size-5" />} />
      </div>

      {carregando && (
        <div className="grid gap-4 lg:grid-cols-2">
          {[0, 1].map((i) => (
            <div key={i} className="rounded-2xl border bg-card p-4 shadow-sm space-y-3">
              <Skeleton className="h-5 w-40" />
              <Skeleton className="h-72 w-full rounded-lg" />
            </div>
          ))}
        </div>
      )}

      {vazio && (
        <div className="rounded-2xl border bg-card p-10 text-center shadow-sm">
          <BarChart3 className="mx-auto size-8 text-muted-foreground/50" />
          <p className="mt-3 font-medium">Nenhuma carona neste mês</p>
          <p className="text-sm text-muted-foreground">Escolha outro mês ou registre marcações.</p>
        </div>
      )}

      {!vazio && !carregando && (
        <>
          {/* Gráficos */}
          <div className="grid gap-4 lg:grid-cols-2">
            <div className="rounded-2xl border bg-card p-4 shadow-sm">
              <h2 className="font-semibold mb-2">Total por motorista</h2>
              <EChart option={opcaoMotoristas} className="h-72 w-full" />
            </div>
            <div className="rounded-2xl border bg-card p-4 shadow-sm">
              <h2 className="font-semibold mb-2">Gasto por passageiro</h2>
              <EChart option={opcaoPassageiros} className="h-72 w-full" />
            </div>
          </div>

          {/* Detalhe por motorista */}
          <div className="space-y-4">
            {dados.map((m) => (
              <CardMotorista key={m.motoristaId} motorista={m} />
            ))}
          </div>
        </>
      )}
    </div>
  );
}

function Kpi({
  rotulo,
  valor,
  icone,
  destaque,
  carregando,
}: {
  rotulo: string;
  valor: string;
  icone: React.ReactNode;
  destaque?: boolean;
  carregando?: boolean;
}) {
  if (destaque) {
    return (
      <div className="rounded-2xl border border-primary/20 bg-gradient-to-br from-primary to-primary/70 p-4 text-primary-foreground shadow-sm">
        <div className="flex items-center justify-between">
          <span className="text-sm/none opacity-90">{rotulo}</span>
          {icone}
        </div>
        {carregando ? (
          <Skeleton className="mt-3 h-8 w-28 bg-white/30" />
        ) : (
          <p className="mt-3 text-2xl font-bold tabular-nums">{valor}</p>
        )}
      </div>
    );
  }
  return (
    <div className="rounded-2xl border bg-card p-4 shadow-sm">
      <div className="flex items-center justify-between text-muted-foreground">
        <span className="text-sm">{rotulo}</span>
        {icone}
      </div>
      {carregando ? (
        <Skeleton className="mt-3 h-8 w-16" />
      ) : (
        <p className="mt-3 text-2xl font-bold tabular-nums">{valor}</p>
      )}
    </div>
  );
}

function CardMotorista({ motorista }: { motorista: ResumoMensalMotorista }) {
  return (
    <div className="rounded-2xl border bg-card shadow-sm overflow-hidden">
      <div className="flex items-center justify-between gap-3 border-b bg-muted/30 p-4">
        <div className="flex items-center gap-3 min-w-0">
          <Avatar seed={motorista.avatar} nome={motorista.motoristaNome} className="size-11 ring-2 ring-primary/20" />
          <div className="min-w-0">
            <p className="font-semibold truncate">{motorista.motoristaNome}</p>
            <p className="text-xs text-muted-foreground">
              {motorista.qtdDiasDirigiu} dia(s) · {motorista.qtdViagens} viagem(ns)
            </p>
          </div>
        </div>
        <div className="text-right">
          <p className="text-xs text-muted-foreground">Total</p>
          <p className="text-lg font-bold text-primary tabular-nums">{formatarReal(motorista.totalValor)}</p>
        </div>
      </div>

      <ul className="divide-y">
        {motorista.passageiros.map((p) => (
          <li key={p.passageiroId} className="flex items-center gap-3 p-3 sm:px-4">
            <Avatar seed={p.avatar} nome={p.nome} className="size-8" />
            <span className="flex-1 min-w-0 truncate text-sm font-medium">{p.nome}</span>
            <span className="hidden sm:inline text-xs text-muted-foreground tabular-nums w-24 text-right">
              {p.qtdDias} dia(s)
            </span>
            <span className="text-xs text-muted-foreground tabular-nums w-24 text-right">
              {p.qtdViagens} viagem(ns)
            </span>
            <span className="w-24 text-right font-semibold tabular-nums">{formatarReal(p.valor)}</span>
          </li>
        ))}
        {motorista.passageiros.length === 0 && (
          <li className="p-4 text-sm text-muted-foreground">Nenhum passageiro neste mês.</li>
        )}
      </ul>
    </div>
  );
}

import { useCallback, useEffect, useState } from 'react';
import { sileo } from 'sileo';
import { Button } from '@/components/ui/button';
import { PixDialog } from '@/components/PixDialog';
import { StatusPagamentoBadge } from '@/components/StatusPagamentoBadge';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { anoMesAtual, formatarReal, rotuloAnoMes } from '@/lib/ui';
import { StatusPagamento, type PagamentoResumo } from '@/types';

export function Pagamentos() {
  const { token } = useAuth();
  const t = token ?? undefined;

  const [anoMes, setAnoMes] = useState(anoMesAtual());
  const [resumo, setResumo] = useState<PagamentoResumo[]>([]);

  const carregar = useCallback(async () => {
    setResumo(await api.get<PagamentoResumo[]>(`/pagamentos/resumo?anoMes=${anoMes}`, t));
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

  async function informar(motoristaId: number) {
    try {
      await api.post('/pagamentos/informar', { motoristaId, anoMes }, t);
      sileo.success({ title: 'Pagamento informado ao motorista.' });
      await carregar();
    } catch (erro) {
      sileo.error({ title: erro instanceof Error ? erro.message : 'Falha ao informar o pagamento.' });
    }
  }

  const totalGeral = resumo.reduce((s, r) => s + r.total, 0);

  return (
    <div className="max-w-2xl mx-auto space-y-6 animate-in fade-in duration-300">
      <div className="flex items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Pagamentos</h1>
          <p className="text-muted-foreground text-sm">O que você deve a cada motorista.</p>
        </div>
        <div className="flex items-center gap-1 rounded-full border bg-card p-1 shadow-sm">
          <Button variant="ghost" size="icon" className="rounded-full" onClick={() => mudarMes(-1)}>‹</Button>
          <span className="text-sm font-medium w-16 text-center tabular-nums">{rotuloAnoMes(anoMes)}</span>
          <Button variant="ghost" size="icon" className="rounded-full" onClick={() => mudarMes(1)}>›</Button>
        </div>
      </div>

      <ul className="space-y-3">
        {resumo.map((r) => (
          <li key={r.motoristaId} className="rounded-2xl border bg-card p-4 shadow-sm transition-all hover:shadow-md hover:-translate-y-0.5 space-y-3 animate-in fade-in">
            <div className="flex items-center justify-between gap-2">
              <span className="font-medium">{r.motoristaNome}</span>
              <div className="flex items-center gap-2">
                <StatusPagamentoBadge status={r.status} />
                <span className="font-semibold tabular-nums">{formatarReal(r.total)}</span>
              </div>
            </div>
            <div className="text-sm text-muted-foreground">
              {r.qtdDias} dia(s){r.chavePix ? ` · PIX (${r.tipoChave}): ${r.chavePix}` : ''}
            </div>
            <div className="flex flex-wrap gap-2">
              {r.chavePix && (
                <PixDialog chave={r.chavePix} nome={r.motoristaNome} valor={r.total} />
              )}
              {r.status !== StatusPagamento.Confirmado && (
                <Button size="sm" onClick={() => informar(r.motoristaId)}>
                  {r.status === StatusPagamento.Informado ? 'Reenviar aviso' : 'Já paguei'}
                </Button>
              )}
            </div>
          </li>
        ))}
        {resumo.length === 0 && <p className="text-muted-foreground text-sm">Nada a pagar neste mês.</p>}
      </ul>

      {resumo.length > 0 && (
        <div className="rounded-2xl border bg-muted/40 px-4 py-3 flex justify-between font-semibold">
          <span>Total do mês</span>
          <span className="tabular-nums">{formatarReal(totalGeral)}</span>
        </div>
      )}
    </div>
  );
}

import { useCallback, useEffect, useState } from 'react';
import { sileo } from 'sileo';
import { Button } from '@/components/ui/button';
import { StatusPagamentoBadge } from '@/components/StatusPagamentoBadge';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { anoMesAtual, formatarReal, rotuloAnoMes } from '@/lib/ui';
import { StatusPagamento, type Recebimento } from '@/types';

export function Recebimentos() {
  const { token } = useAuth();
  const t = token ?? undefined;

  const [anoMes, setAnoMes] = useState(anoMesAtual());
  const [itens, setItens] = useState<Recebimento[]>([]);

  const carregar = useCallback(async () => {
    setItens(await api.get<Recebimento[]>(`/pagamentos/recebimentos?anoMes=${anoMes}`, t));
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

  async function acao(pagamentoId: number, tipo: 'confirmar' | 'rejeitar') {
    try {
      await api.put(`/pagamentos/${pagamentoId}/${tipo}`, {}, t);
      sileo.success({ title: tipo === 'confirmar' ? 'Recebimento confirmado.' : 'Pagamento rejeitado.' });
      await carregar();
    } catch (erro) {
      sileo.error({ title: erro instanceof Error ? erro.message : 'Falha ao atualizar o recebimento.' });
    }
  }

  const pagaram = itens.filter((i) => i.status === StatusPagamento.Confirmado);
  const faltaPagar = itens.filter((i) => i.status !== StatusPagamento.Confirmado);
  const totalReceber = faltaPagar.reduce((s, i) => s + i.valor, 0);
  const totalRecebido = pagaram.reduce((s, i) => s + i.valor, 0);

  return (
    <div className="max-w-2xl mx-auto space-y-6 animate-in fade-in duration-300">
      <div className="flex items-end justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Recebimentos</h1>
          <p className="text-muted-foreground text-sm">O que os passageiros te devem.</p>
        </div>
        <div className="flex items-center gap-1 rounded-full border bg-card p-1 shadow-sm">
          <Button variant="ghost" size="icon" className="rounded-full" onClick={() => mudarMes(-1)}>‹</Button>
          <span className="text-sm font-medium w-16 text-center tabular-nums">{rotuloAnoMes(anoMes)}</span>
          <Button variant="ghost" size="icon" className="rounded-full" onClick={() => mudarMes(1)}>›</Button>
        </div>
      </div>

      <Secao titulo="Falta pagar" total={totalReceber} vazio="Ninguém pendente.">
        {faltaPagar.map((i) => (
          <LinhaRecebimento key={i.passageiroId} item={i} onAcao={acao} />
        ))}
      </Secao>

      <Secao titulo="Já pagaram" total={totalRecebido} vazio="Nenhum pagamento confirmado.">
        {pagaram.map((i) => (
          <LinhaRecebimento key={i.passageiroId} item={i} onAcao={acao} />
        ))}
      </Secao>
    </div>
  );
}

function Secao({
  titulo,
  total,
  vazio,
  children,
}: {
  titulo: string;
  total: number;
  vazio: string;
  children: React.ReactNode[];
}) {
  return (
    <section className="space-y-2">
      <div className="flex items-center justify-between">
        <h2 className="font-semibold">{titulo}</h2>
        <span className="text-sm text-muted-foreground tabular-nums">{formatarReal(total)}</span>
      </div>
      <ul className="space-y-2">
        {children.length > 0 ? children : <p className="text-muted-foreground text-sm">{vazio}</p>}
      </ul>
    </section>
  );
}

function LinhaRecebimento({
  item,
  onAcao,
}: {
  item: Recebimento;
  onAcao: (id: number, tipo: 'confirmar' | 'rejeitar') => void;
}) {
  return (
    <li className="rounded-2xl border bg-card p-4 shadow-sm transition-all hover:shadow-md hover:-translate-y-0.5 space-y-3 animate-in fade-in">
      <div className="flex items-center justify-between gap-2">
        <span className="font-medium">{item.passageiroNome}</span>
        <div className="flex items-center gap-2">
          <StatusPagamentoBadge status={item.status} />
          <span className="font-semibold tabular-nums">{formatarReal(item.valor)}</span>
        </div>
      </div>
      {item.pagamentoId && item.status !== StatusPagamento.Confirmado && (
        <div className="flex gap-2">
          <Button size="sm" onClick={() => onAcao(item.pagamentoId!, 'confirmar')}>
            Confirmar recebimento
          </Button>
          <Button variant="outline" size="sm" onClick={() => onAcao(item.pagamentoId!, 'rejeitar')}>
            Rejeitar
          </Button>
        </div>
      )}
      {item.pagamentoId && item.status === StatusPagamento.Confirmado && (
        <Button variant="outline" size="sm" onClick={() => onAcao(item.pagamentoId!, 'rejeitar')}>
          Desfazer
        </Button>
      )}
    </li>
  );
}

import { useState } from 'react';
import { Info } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { formatarReal } from '@/lib/ui';
import type { DetalheMotorista } from '@/types';

export function DetalhesMotorista({ motoristaId, anoMes }: { motoristaId: number; anoMes: number }) {
  const { token } = useAuth();
  const [detalhe, setDetalhe] = useState<DetalheMotorista | null>(null);

  async function carregar() {
    setDetalhe(null);
    const d = await api.get<DetalheMotorista>(
      `/motoristas/${motoristaId}/detalhes?anoMes=${anoMes}`,
      token ?? undefined,
    );
    setDetalhe(d);
  }

  return (
    <Dialog onOpenChange={(aberto) => aberto && carregar()}>
      <DialogTrigger
        render={
          <Button variant="outline" size="sm">
            <Info className="size-4" /> Detalhes
          </Button>
        }
      />
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{detalhe?.motoristaNome ?? 'Detalhes'}</DialogTitle>
          <DialogDescription>O que este motorista repassa por viagem.</DialogDescription>
        </DialogHeader>

        {!detalhe && <p className="text-sm text-muted-foreground">Carregando…</p>}

        {detalhe && (
          <div className="space-y-3 text-sm">
            {!detalhe.configurado && (
              <p className="rounded-lg bg-amber-500/15 text-amber-600 dark:text-amber-400 px-3 py-2">
                Motorista ainda não informou o consumo do veículo.
              </p>
            )}
            {!detalhe.temConfigMes && (
              <p className="rounded-lg bg-amber-500/15 text-amber-600 dark:text-amber-400 px-3 py-2">
                Sem preço de combustível configurado para este mês.
              </p>
            )}

            <Linha rotulo="Veículo" valor={detalhe.veiculoNome ?? '—'} />
            <Linha rotulo="Consumo" valor={`${detalhe.consumoKmLitro} km/l`} />
            <Linha rotulo="Distância" valor={`${detalhe.kmPorViagem} km`} />
            <Linha rotulo="Preço do litro" valor={formatarReal(detalhe.precoLitro)} />
            <Linha rotulo="Manutenção/km" valor={formatarReal(detalhe.custoKmManutencao)} />
            <div className="border-t pt-2 space-y-2">
              <Linha rotulo="Combustível" valor={formatarReal(detalhe.custoCombustivel)} />
              <Linha rotulo="Custo total da viagem" valor={formatarReal(detalhe.custoTotal)} destaque />
            </div>
            <p className="text-xs text-muted-foreground">
              O custo total é dividido entre os passageiros + o motorista.
            </p>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}

function Linha({ rotulo, valor, destaque }: { rotulo: string; valor: string; destaque?: boolean }) {
  return (
    <div className="flex justify-between">
      <span className="text-muted-foreground">{rotulo}</span>
      <span className={destaque ? 'font-semibold' : 'font-medium'}>{valor}</span>
    </div>
  );
}

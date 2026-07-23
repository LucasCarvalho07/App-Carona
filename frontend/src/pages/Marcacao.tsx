import { useCallback, useEffect, useMemo, useState } from 'react';
import { sileo } from 'sileo';
import { DayPicker, type DayButtonProps } from 'react-day-picker';
import { ptBR } from 'date-fns/locale';
import { format } from 'date-fns';
import 'react-day-picker/style.css';
import { Button } from '@/components/ui/button';
import { DetalhesMotorista } from '@/components/DetalhesMotorista';
import { cn } from '@/lib/utils';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { formatarReal } from '@/lib/ui';
import { avatarDataUri } from '@/lib/avatar';
import { Papel, Sentido, type EscalaCarro, type Ocupante } from '@/types';

const iso = (d: Date) => format(d, 'yyyy-MM-dd');

/** Dia do calendário com pontos de ida (verde) / volta (azul), lidos dos modifiers. */
function DiaBotao({ day, modifiers, ...props }: DayButtonProps) {
  const temIda = Boolean(modifiers.ida);
  const temVolta = Boolean(modifiers.volta);
  return (
    <button {...props} className={cn(props.className, 'relative')}>
      {day.date.getDate()}
      {(temIda || temVolta) && (
        <span className="absolute inset-x-0 -bottom-0.5 flex justify-center gap-0.5">
          {temIda && <span className="size-1.5 rounded-full bg-emerald-500" />}
          {temVolta && <span className="size-1.5 rounded-full bg-sky-500" />}
        </span>
      )}
    </button>
  );
}

export function Marcacao() {
  const { token, usuario, isMaster } = useAuth();
  const t = token ?? undefined;
  const ehMotorista = (usuario?.papeis?.includes(Papel.Motorista) ?? false) || isMaster;

  const [mes, setMes] = useState(new Date());
  const [diaSel, setDiaSel] = useState<Date | undefined>(new Date());
  const [carros, setCarros] = useState<EscalaCarro[]>([]);

  const anoMes = mes.getFullYear() * 100 + (mes.getMonth() + 1);

  const carregar = useCallback(async () => {
    setCarros(await api.get<EscalaCarro[]>(`/escala?anoMes=${anoMes}`, t));
  }, [t, anoMes]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  // Pontos do calendário = só os dias em que EU participo (dirijo ou estou no carro).
  const meuEnvolvimento = (c: EscalaCarro) => c.souMotorista || c.estouNesteCarro;
  const diasComIda = useMemo(
    () => carros.filter((c) => c.sentido === Sentido.Ida && meuEnvolvimento(c)).map((c) => new Date(c.data)),
    [carros],
  );
  const diasComVolta = useMemo(
    () => carros.filter((c) => c.sentido === Sentido.Volta && meuEnvolvimento(c)).map((c) => new Date(c.data)),
    [carros],
  );

  const carrosDoDia = useCallback(
    (sentido: Sentido) =>
      diaSel
        ? carros.filter((c) => c.data.slice(0, 10) === iso(diaSel) && c.sentido === sentido)
        : [],
    [carros, diaSel],
  );

  async function escalar(sentido: Sentido) {
    if (!diaSel) return;
    try {
      await api.post('/escala', { data: iso(diaSel), sentido }, t);
      sileo.success({ title: `Você dirige na ${sentido.toLowerCase()}.` });
      await carregar();
    } catch (e) {
      sileo.error({ title: e instanceof Error ? e.message : 'Falha ao se escalar.' });
    }
  }

  async function desescalar(sentido: Sentido) {
    if (!diaSel) return;
    try {
      await api.del('/escala', { data: iso(diaSel), sentido }, t);
      sileo.success({ title: 'Você saiu da escala.' });
      await carregar();
    } catch (e) {
      sileo.error({ title: e instanceof Error ? e.message : 'Falha ao sair da escala.' });
    }
  }

  async function entrar(carro: EscalaCarro) {
    try {
      await api.post('/marcacao', { data: carro.data, motoristaId: carro.motoristaId, sentido: carro.sentido }, t);
      sileo.success({ title: `${carro.sentido} marcada com ${carro.motoristaNome}.` });
      await carregar();
    } catch (e) {
      sileo.error({ title: e instanceof Error ? e.message : 'Falha ao marcar.' });
    }
  }

  async function sair(carro: EscalaCarro) {
    try {
      await api.del('/marcacao', { data: carro.data, motoristaId: carro.motoristaId, sentido: carro.sentido }, t);
      sileo.success({ title: `${carro.sentido} removida.` });
      await carregar();
    } catch (e) {
      sileo.error({ title: e instanceof Error ? e.message : 'Falha ao remover.' });
    }
  }

  async function escalarSemana() {
    const base = diaSel ?? mes;
    try {
      await api.post('/escala/semana', { dataNaSemana: iso(base) }, t);
      sileo.success({ title: 'Semana escalada (seg–sex, ida e volta).' });
      await carregar();
    } catch (e) {
      sileo.error({ title: e instanceof Error ? e.message : 'Falha ao escalar a semana.' });
    }
  }

  return (
    <div className="max-w-3xl mx-auto space-y-6 animate-in fade-in duration-300">
      <h1 className="text-2xl font-semibold tracking-tight">Marcação de presença</h1>

      <div className="grid md:grid-cols-[auto_1fr] gap-6">
        <div className="space-y-3">
          <div className="rounded-2xl border bg-card p-3 shadow-sm">
            <DayPicker
              mode="single"
              locale={ptBR}
              month={mes}
              onMonthChange={setMes}
              selected={diaSel}
              onSelect={setDiaSel}
              modifiers={{ ida: diasComIda, volta: diasComVolta }}
              components={{ DayButton: DiaBotao }}
            />
            <div className="flex gap-4 justify-center pt-2 text-xs text-muted-foreground">
              <span className="flex items-center gap-1"><span className="size-2 rounded-full bg-emerald-500" /> Ida</span>
              <span className="flex items-center gap-1"><span className="size-2 rounded-full bg-sky-500" /> Volta</span>
            </div>
          </div>
          {ehMotorista && (
            <Button variant="outline" className="w-full" onClick={escalarSemana}>
              Vou dirigir esta semana
            </Button>
          )}
        </div>

        <div className="space-y-4">
          <h2 className="font-medium">
            {diaSel ? format(diaSel, "EEEE, dd 'de' MMMM", { locale: ptBR }) : 'Selecione um dia'}
          </h2>

          <BlocoSentido
            titulo="Ida" cor="emerald" sentido={Sentido.Ida}
            carros={carrosDoDia(Sentido.Ida)} ehMotorista={ehMotorista}
            onEntrar={entrar} onSair={sair} onEscalar={escalar} onDesescalar={desescalar}
          />
          <BlocoSentido
            titulo="Volta" cor="sky" sentido={Sentido.Volta}
            carros={carrosDoDia(Sentido.Volta)} ehMotorista={ehMotorista}
            onEntrar={entrar} onSair={sair} onEscalar={escalar} onDesescalar={desescalar}
          />
        </div>
      </div>
    </div>
  );
}

function BlocoSentido({
  titulo, cor, sentido, carros, ehMotorista, onEntrar, onSair, onEscalar, onDesescalar,
}: {
  titulo: string;
  cor: 'emerald' | 'sky';
  sentido: Sentido;
  carros: EscalaCarro[];
  ehMotorista: boolean;
  onEntrar: (c: EscalaCarro) => void;
  onSair: (c: EscalaCarro) => void;
  onEscalar: (s: Sentido) => void;
  onDesescalar: (s: Sentido) => void;
}) {
  const barra = cor === 'emerald' ? 'bg-emerald-500' : 'bg-sky-500';
  const anoMes = carros[0] ? Number(carros[0].data.slice(0, 4)) * 100 + Number(carros[0].data.slice(5, 7)) : 0;
  const souMotoristaNoTrecho = carros.some((c) => c.souMotorista);

  return (
    <div className="rounded-2xl border bg-card p-4 shadow-sm transition-shadow hover:shadow-md space-y-3">
      <div className="flex items-center gap-2">
        <span className={cn('size-2.5 rounded-full', barra)} />
        <span className="font-medium">{titulo}</span>
      </div>

      {carros.length === 0 ? (
        <p className="text-sm text-muted-foreground">Ninguém escalado nesse dia.</p>
      ) : (
        <ul className="space-y-2">
          {carros.map((c) => (
            <li key={c.viagemId} className="rounded-lg border bg-background p-3 space-y-2 animate-in fade-in duration-200">
              <div className="flex items-center justify-between gap-3">
                <div className="flex items-center gap-2 min-w-0">
                  <img src={avatarDataUri(c.avatar || c.motoristaNome)} alt={c.motoristaNome} className="size-8 rounded-full bg-muted" />
                  <div className="min-w-0">
                    <p className="font-medium truncate">
                      {c.motoristaNome}
                      {c.souMotorista && <span className="ml-2 text-xs text-primary">Você dirige</span>}
                    </p>
                    <p className="text-xs text-muted-foreground">
                      {c.qtdPassageiros} passageiro(s) · {formatarReal(c.valorPorPessoa)}/pessoa
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-2 shrink-0">
                  {anoMes > 0 && <DetalhesMotorista motoristaId={c.motoristaId} anoMes={anoMes} />}
                  {c.souMotorista ? (
                    <Button variant="outline" size="sm" onClick={() => onDesescalar(sentido)}>
                      Não vou dirigir
                    </Button>
                  ) : c.estouNesteCarro ? (
                    <Button variant="outline" size="sm" onClick={() => onSair(c)}>Sair</Button>
                  ) : (
                    <Button size="sm" onClick={() => onEntrar(c)}>Vou nesse carro</Button>
                  )}
                </div>
              </div>
              <Ocupantes ocupantes={c.ocupantes} />
            </li>
          ))}
        </ul>
      )}

      {ehMotorista && !souMotoristaNoTrecho && (
        <Button variant="outline" size="sm" onClick={() => onEscalar(sentido)}>
          Vou dirigir ({titulo.toLowerCase()})
        </Button>
      )}
    </div>
  );
}

/** Ocupantes do carro nessa viagem (avatares + nomes). */
function Ocupantes({ ocupantes }: { ocupantes: Ocupante[] }) {
  if (ocupantes.length === 0) return null;
  return (
    <div className="border-t pt-2">
      <ul className="flex flex-wrap gap-2">
        {ocupantes.map((o) => (
          <li key={o.id} className="flex items-center gap-1.5 rounded-full border bg-card py-0.5 pl-0.5 pr-2.5 text-xs">
            <img src={avatarDataUri(o.avatar || o.nome)} alt={o.nome} className="size-5 rounded-full bg-muted" />
            <span className="font-medium">{o.nome}</span>
          </li>
        ))}
      </ul>
    </div>
  );
}

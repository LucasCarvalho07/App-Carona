import { useCallback, useEffect, useState, type FormEvent } from 'react';
import { sileo } from 'sileo';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { formatarReal, inputClass, labelClass } from '@/lib/ui';
import type { ParametroCusto } from '@/types';

function CampoReal({ valor, onChange, placeholder }: {
  valor: string;
  onChange: (v: string) => void;
  placeholder: string;
}) {
  return (
    <div className="relative">
      <span className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-sm text-muted-foreground">
        R$
      </span>
      <input
        className={cn(inputClass, 'pl-9')}
        type="number"
        step="0.01"
        min="0"
        placeholder={placeholder}
        value={valor}
        onChange={(e) => onChange(e.target.value)}
        required
      />
    </div>
  );
}

function hojeIso(): string {
  const d = new Date();
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function rotuloData(iso: string): string {
  const [ano, mes, dia] = iso.slice(0, 10).split('-');
  return `${dia}/${mes}/${ano}`;
}

export function ConfigCalculo() {
  const { token } = useAuth();
  const t = token ?? undefined;

  const [parametros, setParametros] = useState<ParametroCusto[]>([]);
  const [form, setForm] = useState({
    vigenteDe: hojeIso(),
    precoLitro: '',
    custoKmManutencao: '0.16',
  });

  const carregar = useCallback(async () => {
    setParametros(await api.get<ParametroCusto[]>('/parametros-custo', t));
  }, [t]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  async function salvar(e: FormEvent) {
    e.preventDefault();
    try {
      await api.post('/parametros-custo', {
        vigenteDe: form.vigenteDe,
        precoLitro: Number(form.precoLitro),
        custoKmManutencao: Number(form.custoKmManutencao),
      }, t);
      sileo.success({ title: 'Parâmetro salvo.' });
      await carregar();
    } catch (erro) {
      sileo.error({ title: erro instanceof Error ? erro.message : 'Falha ao salvar o parâmetro.' });
    }
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6 animate-in fade-in duration-300">
      <h1 className="text-2xl font-semibold tracking-tight">Parâmetros de cálculo</h1>

      <form onSubmit={salvar} className="rounded-2xl border bg-card p-4 shadow-sm space-y-3">
        <p className="text-sm text-muted-foreground">
          O preço passa a valer a partir da data informada. As caronas anteriores mantêm o preço vigente na época.
        </p>
        <div className="grid sm:grid-cols-3 gap-3">
          <div className="space-y-1">
            <label className={labelClass}>A partir de</label>
            <input className={inputClass} type="date" value={form.vigenteDe}
              onChange={(e) => setForm({ ...form, vigenteDe: e.target.value })} required />
          </div>
          <div className="space-y-1">
            <label className={labelClass}>Preço do litro</label>
            <CampoReal valor={form.precoLitro} placeholder="6,14"
              onChange={(v) => setForm({ ...form, precoLitro: v })} />
          </div>
          <div className="space-y-1">
            <label className={labelClass}>Manutenção por km</label>
            <CampoReal valor={form.custoKmManutencao} placeholder="0,16"
              onChange={(v) => setForm({ ...form, custoKmManutencao: v })} />
          </div>
        </div>
        <Button type="submit">Salvar parâmetro</Button>
      </form>

      <ul className="space-y-2">
        {parametros.map((p) => (
          <li key={p.id} className="rounded-2xl border bg-card p-3 shadow-sm transition-shadow hover:shadow-md flex justify-between text-sm">
            <span className="font-medium">a partir de {rotuloData(p.vigenteDe)}</span>
            <span className="text-muted-foreground">
              litro {formatarReal(p.precoLitro)} · manut. {formatarReal(p.custoKmManutencao)}/km
            </span>
          </li>
        ))}
        {parametros.length === 0 && <p className="text-muted-foreground text-sm">Nenhum parâmetro cadastrado.</p>}
      </ul>
    </div>
  );
}

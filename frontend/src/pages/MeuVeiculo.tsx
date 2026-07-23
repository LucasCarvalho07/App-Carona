import { useCallback, useEffect, useState, type FormEvent } from 'react';
import { sileo } from 'sileo';
import { Button } from '@/components/ui/button';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { inputClass, labelClass } from '@/lib/ui';
import type { MotoristaConfig, TipoChavePix, TipoCombustivel, Veiculo } from '@/types';

const COMBUSTIVEIS: TipoCombustivel[] = ['Gasolina', 'Etanol', 'Diesel', 'Flex', 'Gnv'];
const TIPOS_CHAVE: TipoChavePix[] = ['Cpf', 'Cnpj', 'Email', 'Telefone', 'Aleatoria'];

export function MeuVeiculo() {
  const { token } = useAuth();
  const t = token ?? undefined;

  const [veiculoId, setVeiculoId] = useState<number | null>(null);
  const [form, setForm] = useState({
    nome: '',
    modelo: '',
    consumoKmLitro: '',
    kmPorViagem: '',
    combustivel: 'Gasolina' as TipoCombustivel,
  });
  const [pix, setPix] = useState<MotoristaConfig>({
    chavePix: '',
    tipoChave: 'Cpf',
    titular: '',
    emailComprovante: '',
  });

  const carregar = useCallback(async () => {
    const [lista, config] = await Promise.all([
      api.get<Veiculo[]>('/veiculos', t),
      api.get<MotoristaConfig | undefined>('/motorista/config', t),
    ]);
    const veiculo = lista[0];
    if (veiculo) {
      setVeiculoId(veiculo.id);
      setForm({
        nome: veiculo.nome,
        modelo: veiculo.modelo ?? '',
        consumoKmLitro: String(veiculo.consumoKmLitro),
        kmPorViagem: String(veiculo.kmPorViagem),
        combustivel: veiculo.combustivel,
      });
    }
    if (config) setPix(config);
  }, [t]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  const veiculoValido = Number(form.consumoKmLitro) > 0 && Number(form.kmPorViagem) > 0;

  async function salvarVeiculo(e: FormEvent) {
    e.preventDefault();
    if (!veiculoValido) return;
    // Um único veículo por usuário; sempre padrão (usado no cálculo da carona).
    const dados = {
      nome: form.nome,
      modelo: form.modelo || null,
      consumoKmLitro: Number(form.consumoKmLitro),
      kmPorViagem: Number(form.kmPorViagem),
      combustivel: form.combustivel,
      padrao: true,
    };
    try {
      if (veiculoId) {
        await api.put(`/veiculos/${veiculoId}`, dados, t);
      } else {
        await api.post('/veiculos', dados, t);
      }
      sileo.success({ title: 'Veículo salvo.' });
      await carregar();
    } catch (erro) {
      sileo.error({ title: erro instanceof Error ? erro.message : 'Falha ao salvar o veículo.' });
    }
  }

  async function salvarPix(e: FormEvent) {
    e.preventDefault();
    try {
      await api.put('/motorista/config', pix, t);
      sileo.success({ title: 'Dados de recebimento salvos.' });
    } catch (erro) {
      sileo.error({ title: erro instanceof Error ? erro.message : 'Falha ao salvar o recebimento.' });
    }
  }

  return (
    <div className="max-w-2xl mx-auto space-y-8 animate-in fade-in duration-300">
      <h1 className="text-2xl font-semibold tracking-tight">Meu veículo</h1>

      <section className="space-y-3">
        <h2 className="font-medium">Veículo</h2>
        <p className="text-sm text-muted-foreground">
          Usado para calcular o custo da carona. Você pode editar quando quiser.
        </p>

        <form onSubmit={salvarVeiculo} className="rounded-2xl border bg-card p-4 shadow-sm space-y-3">
          <div className="space-y-1">
            <label className={labelClass}>Nome</label>
            <input className={inputClass} placeholder="Ex: Meu carro" value={form.nome}
              onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
          </div>
          <div className="space-y-1">
            <label className={labelClass}>Modelo (opcional)</label>
            <input className={inputClass} placeholder="Ex: Onix 1.0" value={form.modelo}
              onChange={(e) => setForm({ ...form, modelo: e.target.value })} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-1">
              <label className={labelClass}>Consumo (km/l)</label>
              <input className={inputClass} type="number" step="0.1" min="0.1" placeholder="Ex: 12"
                value={form.consumoKmLitro} onChange={(e) => setForm({ ...form, consumoKmLitro: e.target.value })} required />
            </div>
            <div className="space-y-1">
              <label className={labelClass}>Km por viagem</label>
              <input className={inputClass} type="number" step="0.1" min="0.1" placeholder="Ex: 30"
                value={form.kmPorViagem} onChange={(e) => setForm({ ...form, kmPorViagem: e.target.value })} required />
            </div>
          </div>
          <div className="space-y-1">
            <label className={labelClass}>Combustível</label>
            <Select value={form.combustivel}
              onValueChange={(v) => setForm({ ...form, combustivel: v as TipoCombustivel })}>
              <SelectTrigger className="w-full"><SelectValue /></SelectTrigger>
              <SelectContent>
                {COMBUSTIVEIS.map((c) => <SelectItem key={c} value={c}>{c}</SelectItem>)}
              </SelectContent>
            </Select>
          </div>
          <Button type="submit" disabled={!veiculoValido}>
            {veiculoId ? 'Salvar alterações' : 'Salvar veículo'}
          </Button>
          {!veiculoValido && (
            <p className="text-xs text-muted-foreground">
              Informe consumo (km/l) e km por viagem maiores que zero.
            </p>
          )}
        </form>
      </section>

      <section className="space-y-3">
        <h2 className="font-medium">Recebimento (PIX)</h2>
        <form onSubmit={salvarPix} className="rounded-2xl border bg-card p-4 shadow-sm space-y-3">
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-1">
              <label className={labelClass}>Tipo da chave</label>
              <Select value={pix.tipoChave}
                onValueChange={(v) => setPix({ ...pix, tipoChave: v as TipoChavePix })}>
                <SelectTrigger className="w-full"><SelectValue /></SelectTrigger>
                <SelectContent>
                  {TIPOS_CHAVE.map((tc) => <SelectItem key={tc} value={tc}>{tc}</SelectItem>)}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-1">
              <label className={labelClass}>Chave PIX</label>
              <input className={inputClass} placeholder="Sua chave" value={pix.chavePix}
                onChange={(e) => setPix({ ...pix, chavePix: e.target.value })} required />
            </div>
          </div>
          <div className="space-y-1">
            <label className={labelClass}>Titular da chave</label>
            <input className={inputClass} placeholder="Nome do titular" value={pix.titular}
              onChange={(e) => setPix({ ...pix, titular: e.target.value })} required />
          </div>
          <div className="space-y-1">
            <label className={labelClass}>E-mail p/ comprovantes (opcional)</label>
            <input className={inputClass} type="email" placeholder="email@exemplo.com"
              value={pix.emailComprovante ?? ''} onChange={(e) => setPix({ ...pix, emailComprovante: e.target.value })} />
          </div>
          <Button type="submit">Salvar recebimento</Button>
        </form>
      </section>
    </div>
  );
}

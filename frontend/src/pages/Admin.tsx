import { useCallback, useEffect, useState } from 'react';
import { sileo } from 'sileo';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { Papel, StatusUsuario, type Usuario } from '@/types';

export function Admin() {
  const { token, usuario } = useAuth();
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const souPrincipal = usuario?.ehMasterPrincipal ?? false;

  const carregar = useCallback(async () => {
    setCarregando(true);
    setErro(null);
    try {
      const lista = await api.get<Usuario[]>('/usuarios', token ?? undefined);
      setUsuarios(lista);
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Falha ao carregar usuários.');
    } finally {
      setCarregando(false);
    }
  }, [token]);

  useEffect(() => {
    void carregar();
  }, [carregar]);

  async function aprovar(id: number, papeis: Papel[]) {
    try {
      await api.put(`/usuarios/${id}/aprovar`, { papeis }, token ?? undefined);
      sileo.success({ title: 'Usuário atualizado.' });
      await carregar();
    } catch (e) {
      sileo.error({ title: e instanceof Error ? e.message : 'Falha ao atualizar o usuário.' });
    }
  }

  async function definirMaster(id: number, tornarMaster: boolean) {
    try {
      await api.put(`/usuarios/${id}/master`, { tornarMaster }, token ?? undefined);
      sileo.success({ title: tornarMaster ? 'Usuário agora é master.' : 'Master removido.' });
      await carregar();
    } catch (e) {
      sileo.error({ title: e instanceof Error ? e.message : 'Falha ao alterar master.' });
    }
  }

  return (
    <div className="max-w-3xl mx-auto space-y-6 animate-in fade-in duration-300">
      <h1 className="text-2xl font-semibold tracking-tight">Usuários</h1>

      {erro && <p className="text-destructive text-sm">{erro}</p>}

      {carregando && (
        <ul className="space-y-3">
          {[0, 1, 2].map((i) => (
            <li key={i} className="rounded-2xl border bg-card p-4 shadow-sm space-y-3">
              <div className="flex items-center justify-between">
                <div className="space-y-2">
                  <Skeleton className="h-4 w-32" />
                  <Skeleton className="h-3 w-44" />
                </div>
                <Skeleton className="h-6 w-24 rounded-full" />
              </div>
              <Skeleton className="h-8 w-full" />
            </li>
          ))}
        </ul>
      )}

      <ul className="space-y-3">
        {usuarios.map((u) => (
          <LinhaUsuario
            key={u.id}
            usuario={u}
            souPrincipal={souPrincipal}
            onAprovar={aprovar}
            onDefinirMaster={definirMaster}
          />
        ))}
      </ul>
    </div>
  );
}

function LinhaUsuario({
  usuario,
  souPrincipal,
  onAprovar,
  onDefinirMaster,
}: {
  usuario: Usuario;
  souPrincipal: boolean;
  onAprovar: (id: number, papeis: Papel[]) => Promise<void>;
  onDefinirMaster: (id: number, tornarMaster: boolean) => Promise<void>;
}) {
  const [ehMotorista, setEhMotorista] = useState(usuario.papeis.includes(Papel.Motorista));
  const [salvando, setSalvando] = useState(false);

  async function confirmar() {
    setSalvando(true);
    try {
      // Passageiro é base (backend adiciona). Aqui só decide se é também motorista.
      await onAprovar(usuario.id, ehMotorista ? [Papel.Motorista] : []);
    } finally {
      setSalvando(false);
    }
  }

  const ehMaster = usuario.papeis.includes(Papel.Master);
  // Só o master principal gerencia masters, e nunca o próprio principal.
  const podeGerenciarMaster = souPrincipal && !usuario.ehMasterPrincipal;

  return (
    <li className="rounded-2xl border bg-card p-4 shadow-sm transition-shadow hover:shadow-md flex flex-col gap-3">
      <div className="flex items-center justify-between gap-4">
        <div>
          <p className="font-medium">{usuario.nome}</p>
          <p className="text-sm text-muted-foreground">{usuario.email}</p>
        </div>
        <div className="flex items-center gap-2">
          {usuario.ehMasterPrincipal && (
            <span className="text-xs rounded-full border border-primary/40 bg-primary/10 text-primary px-2 py-1">
              Master principal
            </span>
          )}
          <span className="text-xs rounded-full border px-2 py-1">{usuario.status}</span>
        </div>
      </div>

      {!ehMaster && (
        <div className="space-y-2">
          <div className="flex flex-wrap items-center gap-4">
            <label className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                checked={ehMotorista}
                onChange={() => setEhMotorista((v) => !v)}
              />
              Também é motorista
            </label>
            <Button size="sm" onClick={confirmar} disabled={salvando}>
              {usuario.status === StatusUsuario.Ativo ? 'Atualizar' : 'Aprovar'}
            </Button>
          </div>
          <p className="text-xs text-muted-foreground">
            Todo usuário aprovado entra como passageiro.
          </p>
        </div>
      )}

      {podeGerenciarMaster && (
        <label className="flex items-center gap-2 text-sm border-t pt-3">
          <input
            type="checkbox"
            checked={ehMaster}
            onChange={() => onDefinirMaster(usuario.id, !ehMaster)}
          />
          É master (acesso total)
        </label>
      )}
    </li>
  );
}

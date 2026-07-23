import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { sileo } from 'sileo';
import { Button } from '@/components/ui/button';
import { Avatar } from '@/components/Avatar';
import { api } from '@/lib/api';
import { useAuth } from '@/lib/auth';
import { avatarDataUri, SEEDS_AVATAR } from '@/lib/avatar';
import { cn } from '@/lib/utils';
import { useTheme, type Tema } from '@/lib/theme';
import type { Usuario } from '@/types';

const OPCOES_TEMA: { valor: Tema; label: string }[] = [
  { valor: 'light', label: 'Claro' },
  { valor: 'dark', label: 'Escuro' },
  { valor: 'system', label: 'Sistema' },
];

export function Perfil() {
  const { usuario, token, atualizarUsuario, logout } = useAuth();
  const { tema, setTema } = useTheme();
  const navigate = useNavigate();
  const [salvando, setSalvando] = useState<string | null>(null);

  function sair() {
    logout();
    navigate('/login', { replace: true });
  }

  async function escolherAvatar(seed: string) {
    setSalvando(seed);
    try {
      const atualizado = await api.put<Usuario>('/perfil/avatar', { avatar: seed }, token ?? undefined);
      atualizarUsuario(atualizado);
      sileo.success({ title: 'Avatar atualizado.' });
    } catch (erro) {
      sileo.error({ title: erro instanceof Error ? erro.message : 'Falha ao atualizar o avatar.' });
    } finally {
      setSalvando(null);
    }
  }

  return (
    <div className="max-w-lg mx-auto space-y-6 animate-in fade-in duration-300">
      <h1 className="text-2xl font-semibold tracking-tight">Perfil</h1>

      <div className="rounded-2xl border bg-card p-4 shadow-sm flex items-center gap-4">
        <Avatar seed={usuario?.avatar} nome={usuario?.nome} className="size-16" />
        <div className="min-w-0">
          <p className="font-medium truncate">{usuario?.nome}</p>
          <p className="text-sm text-muted-foreground truncate">{usuario?.email}</p>
        </div>
      </div>

      <div className="rounded-2xl border bg-card p-4 shadow-sm space-y-3">
        <p className="text-sm font-medium">Escolha seu avatar</p>
        <div className="grid grid-cols-6 gap-2">
          {SEEDS_AVATAR.map((seed) => {
            const selecionado = usuario?.avatar === seed;
            return (
              <button
                key={seed}
                type="button"
                onClick={() => escolherAvatar(seed)}
                disabled={salvando !== null}
                className={cn(
                  'aspect-square rounded-xl border p-1 cursor-pointer transition-all hover:-translate-y-0.5 hover:shadow-md',
                  selecionado ? 'border-primary ring-2 ring-primary/40' : 'border-border',
                  salvando === seed && 'animate-pulse',
                )}
              >
                <img src={avatarDataUri(seed)} alt={seed} className="size-full" />
              </button>
            );
          })}
        </div>
      </div>

      <div className="rounded-2xl border bg-card p-4 shadow-sm space-y-2">
        <Campo rotulo="Status" valor={usuario?.status} />
        <Campo rotulo="Papéis" valor={usuario?.papeis?.join(', ') || '—'} />
      </div>

      <div className="rounded-2xl border bg-card p-4 shadow-sm space-y-3">
        <p className="text-sm font-medium">Tema</p>
        <div className="flex gap-2">
          {OPCOES_TEMA.map((opcao) => (
            <Button
              key={opcao.valor}
              variant={tema === opcao.valor ? 'default' : 'outline'}
              size="sm"
              onClick={() => setTema(opcao.valor)}
            >
              {opcao.label}
            </Button>
          ))}
        </div>
      </div>

      <Button variant="outline" onClick={sair}>
        Sair
      </Button>
    </div>
  );
}

function Campo({ rotulo, valor }: { rotulo: string; valor?: string }) {
  return (
    <div className="flex justify-between text-sm">
      <span className="text-muted-foreground">{rotulo}</span>
      <span className="font-medium">{valor}</span>
    </div>
  );
}

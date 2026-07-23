import { createContext, useContext, useMemo, useState, type ReactNode } from 'react';
import { api } from '@/lib/api';
import { Papel, type AuthResponse, type Usuario } from '@/types';

interface AuthContextValue {
  usuario: Usuario | null;
  token: string | null;
  isMaster: boolean;
  loginComGoogle: (idToken: string) => Promise<void>;
  loginLocal: (email: string, senha: string) => Promise<AuthResponse>;
  registrar: (nome: string, email: string, telefone: string, senha: string) => Promise<AuthResponse>;
  solicitarRecuperacao: (email: string, canal: CanalRecuperacao) => Promise<void>;
  verificarCodigo: (email: string, codigo: string) => Promise<string>;
  redefinirSenha: (resetToken: string, novaSenha: string) => Promise<void>;
  verificarEmail: (email: string, codigo: string) => Promise<void>;
  reenviarVerificacao: (email: string) => Promise<void>;
  atualizarUsuario: (usuario: Usuario) => void;
  logout: () => void;
}

export const CanalRecuperacao = {
  Email: 'Email',
  WhatsApp: 'WhatsApp',
} as const;
export type CanalRecuperacao = (typeof CanalRecuperacao)[keyof typeof CanalRecuperacao];

const AuthContext = createContext<AuthContextValue | null>(null);

const CHAVE_STORAGE = 'app-carona-auth';

function carregarSessao(): AuthResponse | null {
  const bruto = localStorage.getItem(CHAVE_STORAGE);
  if (!bruto) return null;
  const sessao = JSON.parse(bruto) as AuthResponse;
  // Compatibilidade com sessões antigas (sem papeis).
  if (sessao.usuario && !Array.isArray(sessao.usuario.papeis)) {
    sessao.usuario.papeis = [];
  }
  return sessao;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const sessaoInicial = carregarSessao();
  const [usuario, setUsuario] = useState<Usuario | null>(sessaoInicial?.usuario ?? null);
  const [token, setToken] = useState<string | null>(sessaoInicial?.token ?? null);

  function aplicarSessao(resposta: AuthResponse): void {
    localStorage.setItem(CHAVE_STORAGE, JSON.stringify(resposta));
    setUsuario(resposta.usuario);
    setToken(resposta.token);
  }

  async function loginComGoogle(idToken: string): Promise<void> {
    aplicarSessao(await api.post<AuthResponse>('/auth/google', { idToken }));
  }

  async function loginLocal(email: string, senha: string): Promise<AuthResponse> {
    const resp = await api.post<AuthResponse>('/auth/login', { email, senha });
    aplicarSessao(resp);
    return resp;
  }

  async function registrar(nome: string, email: string, telefone: string, senha: string): Promise<AuthResponse> {
    const resp = await api.post<AuthResponse>('/auth/registrar', { nome, email, telefone, senha });
    aplicarSessao(resp);
    return resp;
  }

  async function verificarEmail(email: string, codigo: string): Promise<void> {
    aplicarSessao(await api.post<AuthResponse>('/auth/verificar-email', { email, codigo }));
  }

  async function reenviarVerificacao(email: string): Promise<void> {
    await api.post('/auth/reenviar-verificacao', { email });
  }

  async function solicitarRecuperacao(email: string, canal: CanalRecuperacao): Promise<void> {
    await api.post('/auth/esqueci-senha', { email, canal });
  }

  async function verificarCodigo(email: string, codigo: string): Promise<string> {
    const resposta = await api.post<{ resetToken: string }>('/auth/verificar-codigo', { email, codigo });
    return resposta.resetToken;
  }

  async function redefinirSenha(resetToken: string, novaSenha: string): Promise<void> {
    await api.post('/auth/redefinir-senha', { resetToken, novaSenha });
  }

  function atualizarUsuario(novo: Usuario): void {
    setUsuario(novo);
    if (token) {
      localStorage.setItem(CHAVE_STORAGE, JSON.stringify({ token, usuario: novo }));
    }
  }

  function logout(): void {
    localStorage.removeItem(CHAVE_STORAGE);
    setUsuario(null);
    setToken(null);
  }

  const isMaster = usuario?.papeis?.includes(Papel.Master) ?? false;

  const valor = useMemo<AuthContextValue>(
    () => ({
      usuario,
      token,
      isMaster,
      loginComGoogle,
      loginLocal,
      registrar,
      solicitarRecuperacao,
      verificarCodigo,
      redefinirSenha,
      verificarEmail,
      reenviarVerificacao,
      atualizarUsuario,
      logout,
    }),
    [usuario, token, isMaster],
  );

  return <AuthContext.Provider value={valor}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const contexto = useContext(AuthContext);
  if (!contexto) {
    throw new Error('useAuth precisa estar dentro de AuthProvider.');
  }
  return contexto;
}

import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { GoogleLogin } from '@react-oauth/google';
import { sileo } from 'sileo';
import { Eye, EyeOff } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/lib/auth';
import type { AuthResponse } from '@/types';
import { inputClass, labelClass } from '@/lib/ui';
import logo from '@/assets/logo.png';

type Modo = 'login' | 'cadastro' | 'verificar';

const REGRAS_SENHA = 'Mínimo 6 caracteres, 1 letra maiúscula e 1 caractere especial.';

export function Login() {
  const { loginComGoogle, loginLocal, registrar, verificarEmail, reenviarVerificacao } = useAuth();
  const navigate = useNavigate();

  const [modo, setModo] = useState<Modo>('login');
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [telefone, setTelefone] = useState('');
  const [senha, setSenha] = useState('');
  const [codigo, setCodigo] = useState('');
  const [mostrarSenha, setMostrarSenha] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);

  // Após login/cadastro: master principal com e-mail não verificado vai para a verificação.
  function encaminhar(resp: AuthResponse) {
    const precisaVerificar = resp.usuario.ehMasterPrincipal && !resp.usuario.emailVerificado;
    if (precisaVerificar) {
      setCodigo('');
      setModo('verificar');
    } else {
      navigate('/', { replace: true });
    }
  }

  async function executar(acao: () => Promise<void>) {
    setErro(null);
    setEnviando(true);
    try {
      await acao();
      navigate('/', { replace: true });
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Falha na autenticação.');
    } finally {
      setEnviando(false);
    }
  }

  async function submeter(evento: FormEvent) {
    evento.preventDefault();
    setErro(null);
    setEnviando(true);
    try {
      const resp =
        modo === 'login'
          ? await loginLocal(email, senha)
          : await registrar(nome, email, telefone, senha);
      encaminhar(resp);
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Falha na autenticação.');
    } finally {
      setEnviando(false);
    }
  }

  async function submeterVerificacao(evento: FormEvent) {
    evento.preventDefault();
    setErro(null);
    setEnviando(true);
    try {
      await verificarEmail(email, codigo);
      navigate('/', { replace: true });
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Código inválido.');
    } finally {
      setEnviando(false);
    }
  }

  async function reenviar() {
    try {
      await reenviarVerificacao(email);
      sileo.success({ title: 'Código reenviado.' });
    } catch {
      sileo.error({ title: 'Falha ao reenviar o código.' });
    }
  }

  return (
    <div className="relative min-h-svh flex flex-col items-center justify-center gap-6 overflow-hidden p-6 bg-gradient-to-b from-primary/10 via-background to-background">
      {/* brilho decorativo */}
      <div className="pointer-events-none absolute -top-32 -right-24 size-80 rounded-full bg-primary/20 blur-3xl" />
      <div className="pointer-events-none absolute -bottom-32 -left-24 size-80 rounded-full bg-primary/10 blur-3xl" />

      <div className="relative w-full max-w-sm flex flex-col items-center gap-6 rounded-3xl border border-border/60 bg-card/80 p-8 shadow-xl backdrop-blur-sm animate-in fade-in zoom-in-95 duration-300">
        <div className="flex flex-col items-center gap-3 text-center">
          <div className="rounded-2xl bg-primary/10 p-3 ring-1 ring-primary/20">
            <img src={logo} alt="Controle de Caronas" className="h-16 w-auto object-contain" />
          </div>
          <p className="text-muted-foreground">
            {modo === 'login' && 'Entre para continuar.'}
            {modo === 'cadastro' && 'Crie sua conta.'}
            {modo === 'verificar' && 'Confirme seu e-mail para continuar.'}
          </p>
        </div>

        {modo === 'verificar' ? (
          <form onSubmit={submeterVerificacao} className="w-full space-y-3">
            <p className="text-sm text-muted-foreground">
              Enviamos um código de 6 dígitos para <span className="font-medium text-foreground">{email}</span>.
            </p>
            <div className="space-y-1">
              <label className={labelClass} htmlFor="codigo">Código</label>
              <input
                id="codigo"
                className={`${inputClass} text-center text-lg tracking-[0.5em] tabular-nums`}
                type="text"
                inputMode="numeric"
                autoComplete="one-time-code"
                maxLength={6}
                placeholder="000000"
                value={codigo}
                onChange={(e) => setCodigo(e.target.value.replace(/\D/g, ''))}
                required
              />
            </div>
            <Button type="submit" className="w-full" loading={enviando} disabled={codigo.length < 6}>
              Confirmar e-mail
            </Button>
            <button type="button" className="w-full text-xs text-muted-foreground underline hover:text-foreground" onClick={reenviar}>
              Reenviar código
            </button>
          </form>
        ) : (
          <>
            <form onSubmit={submeter} className="w-full space-y-3">
              {modo === 'cadastro' && (
                <div className="space-y-1">
                  <label className={labelClass} htmlFor="nome">Nome</label>
                  <input
                    id="nome"
                    className={inputClass}
                    type="text"
                    placeholder="Seu nome"
                    value={nome}
                    onChange={(e) => setNome(e.target.value)}
                    required
                  />
                </div>
              )}

              <div className="space-y-1">
                <label className={labelClass} htmlFor="email">E-mail</label>
                <input
                  id="email"
                  className={inputClass}
                  type="email"
                  placeholder="email@exemplo.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>

              {modo === 'cadastro' && (
                <div className="space-y-1">
                  <label className={labelClass} htmlFor="telefone">Telefone</label>
                  <input
                    id="telefone"
                    className={inputClass}
                    type="tel"
                    placeholder="(00) 00000-0000"
                    value={telefone}
                    onChange={(e) => setTelefone(e.target.value)}
                    required
                  />
                </div>
              )}

              <div className="space-y-1">
                <label className={labelClass} htmlFor="senha">Senha</label>
                <div className="relative">
                  <input
                    id="senha"
                    className={`${inputClass} pr-10`}
                    type={mostrarSenha ? 'text' : 'password'}
                    placeholder="Sua senha"
                    value={senha}
                    onChange={(e) => setSenha(e.target.value)}
                    required
                    minLength={6}
                  />
                  <button
                    type="button"
                    onClick={() => setMostrarSenha((v) => !v)}
                    aria-label={mostrarSenha ? 'Ocultar senha' : 'Mostrar senha'}
                    className="absolute inset-y-0 right-0 flex items-center px-3 text-muted-foreground transition-colors hover:text-foreground"
                  >
                    {mostrarSenha ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
                  </button>
                </div>
                {modo === 'cadastro' && (
                  <p className="text-xs text-muted-foreground">{REGRAS_SENHA}</p>
                )}
              </div>

              {modo === 'login' && (
                <div className="flex justify-end">
                  <Link to="/esqueci-senha" className="text-xs text-muted-foreground underline hover:text-foreground">
                    Esqueci minha senha
                  </Link>
                </div>
              )}

              <Button type="submit" className="w-full" loading={enviando}>
                {modo === 'login' ? 'Entrar' : 'Cadastrar'}
              </Button>
            </form>

            <button
              type="button"
              className="text-sm text-muted-foreground underline"
              onClick={() => {
                setErro(null);
                setModo((m) => (m === 'login' ? 'cadastro' : 'login'));
              }}
            >
              {modo === 'login' ? 'Não tem conta? Cadastre-se' : 'Já tem conta? Entrar'}
            </button>

            <div className="flex items-center gap-3 w-full text-muted-foreground text-xs">
              <div className="h-px flex-1 bg-border" />
              ou
              <div className="h-px flex-1 bg-border" />
            </div>

            <GoogleLogin
              onSuccess={(credential) => {
                if (credential.credential) {
                  void executar(() => loginComGoogle(credential.credential!));
                }
              }}
              onError={() => setErro('Falha no login com Google.')}
            />
          </>
        )}

        {erro && (
          <p className="text-sm text-destructive text-center" role="alert">
            {erro}
          </p>
        )}
      </div>
    </div>
  );
}

import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { sileo } from 'sileo';
import { ArrowLeft, Eye, EyeOff, Mail, MessageCircle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useAuth, CanalRecuperacao } from '@/lib/auth';
import { inputClass, labelClass } from '@/lib/ui';
import logo from '@/assets/logo.png';

type Passo = 'solicitar' | 'codigo' | 'senha';

export function EsqueciSenha() {
  const { solicitarRecuperacao, verificarCodigo, redefinirSenha } = useAuth();
  const navigate = useNavigate();

  const [passo, setPasso] = useState<Passo>('solicitar');
  const [email, setEmail] = useState('');
  const [canal, setCanal] = useState<CanalRecuperacao>(CanalRecuperacao.Email);
  const [codigo, setCodigo] = useState('');
  const [resetToken, setResetToken] = useState('');
  const [novaSenha, setNovaSenha] = useState('');
  const [confirmarSenha, setConfirmarSenha] = useState('');
  const [mostrarSenha, setMostrarSenha] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);

  async function executar(acao: () => Promise<void>) {
    setErro(null);
    setEnviando(true);
    try {
      await acao();
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Ocorreu um erro. Tente novamente.');
    } finally {
      setEnviando(false);
    }
  }

  function enviarCodigo(evento: FormEvent) {
    evento.preventDefault();
    void executar(async () => {
      await solicitarRecuperacao(email, canal);
      sileo.success({ title: 'Se a conta existir, enviamos o código.' });
      setPasso('codigo');
    });
  }

  function conferirCodigo(evento: FormEvent) {
    evento.preventDefault();
    void executar(async () => {
      const token = await verificarCodigo(email, codigo);
      setResetToken(token);
      setPasso('senha');
    });
  }

  function salvarSenha(evento: FormEvent) {
    evento.preventDefault();
    if (novaSenha !== confirmarSenha) {
      setErro('As senhas não conferem.');
      return;
    }
    void executar(async () => {
      await redefinirSenha(resetToken, novaSenha);
      sileo.success({ title: 'Senha redefinida com sucesso.' });
      navigate('/login', { replace: true });
    });
  }

  return (
    <div className="relative min-h-svh flex flex-col items-center justify-center gap-6 overflow-hidden p-6 bg-gradient-to-b from-primary/10 via-background to-background">
      <div className="pointer-events-none absolute -top-32 -right-24 size-80 rounded-full bg-primary/20 blur-3xl" />
      <div className="pointer-events-none absolute -bottom-32 -left-24 size-80 rounded-full bg-primary/10 blur-3xl" />

      <div className="relative w-full max-w-sm flex flex-col items-center gap-6 rounded-3xl border border-border/60 bg-card/80 p-8 shadow-xl backdrop-blur-sm animate-in fade-in zoom-in-95 duration-300">
        <div className="flex flex-col items-center gap-3 text-center">
          <div className="rounded-2xl bg-primary/10 p-3 ring-1 ring-primary/20">
            <img src={logo} alt="Controle de Caronas" className="h-16 w-auto object-contain" />
          </div>
          <div>
            <h1 className="font-semibold tracking-tight">Recuperar senha</h1>
            <p className="text-sm text-muted-foreground">
              {passo === 'solicitar' && 'Enviaremos um código para você.'}
              {passo === 'codigo' && 'Digite o código que você recebeu.'}
              {passo === 'senha' && 'Escolha uma nova senha.'}
            </p>
          </div>
        </div>

        {passo === 'solicitar' && (
          <form onSubmit={enviarCodigo} className="w-full space-y-3">
            <div className="space-y-1">
              <label className={labelClass} htmlFor="email">E-mail da conta</label>
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

            <div className="space-y-1">
              <span className={labelClass}>Enviar código por</span>
              <div className="grid grid-cols-2 gap-2">
                <button
                  type="button"
                  onClick={() => setCanal(CanalRecuperacao.Email)}
                  className={`flex items-center justify-center gap-2 rounded-lg border px-3 py-2 text-sm transition-colors ${
                    canal === CanalRecuperacao.Email
                      ? 'border-primary bg-primary/10 text-primary font-medium'
                      : 'border-border text-muted-foreground hover:border-ring/60'
                  }`}
                >
                  <Mail className="size-4" /> E-mail
                </button>
                <button
                  type="button"
                  disabled
                  aria-disabled="true"
                  title="Em breve"
                  className="flex items-center justify-center gap-2 rounded-lg border border-border px-3 py-2 text-sm text-muted-foreground/50 cursor-not-allowed"
                >
                  <MessageCircle className="size-4" /> WhatsApp
                </button>
              </div>
              <p className="text-xs text-muted-foreground">WhatsApp em breve.</p>
            </div>

            <Button type="submit" className="w-full" loading={enviando}>
              Enviar código
            </Button>
          </form>
        )}

        {passo === 'codigo' && (
          <form onSubmit={conferirCodigo} className="w-full space-y-3">
            <div className="space-y-1">
              <label className={labelClass} htmlFor="codigo">Código de 6 dígitos</label>
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
              Validar código
            </Button>
            <button
              type="button"
              className="w-full text-xs text-muted-foreground underline hover:text-foreground"
              onClick={() => {
                setErro(null);
                setCodigo('');
                setPasso('solicitar');
              }}
            >
              Reenviar código
            </button>
          </form>
        )}

        {passo === 'senha' && (
          <form onSubmit={salvarSenha} className="w-full space-y-3">
            <div className="space-y-1">
              <label className={labelClass} htmlFor="novaSenha">Nova senha</label>
              <div className="relative">
                <input
                  id="novaSenha"
                  className={`${inputClass} pr-10`}
                  type={mostrarSenha ? 'text' : 'password'}
                  placeholder="Nova senha"
                  value={novaSenha}
                  onChange={(e) => setNovaSenha(e.target.value)}
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
            </div>
            <div className="space-y-1">
              <label className={labelClass} htmlFor="confirmarSenha">Confirmar senha</label>
              <input
                id="confirmarSenha"
                className={inputClass}
                type={mostrarSenha ? 'text' : 'password'}
                placeholder="Repita a nova senha"
                value={confirmarSenha}
                onChange={(e) => setConfirmarSenha(e.target.value)}
                required
                minLength={6}
              />
            </div>
            <p className="text-xs text-muted-foreground">
              Mínimo 6 caracteres, 1 letra maiúscula e 1 caractere especial.
            </p>
            <Button type="submit" className="w-full" loading={enviando}>
              Redefinir senha
            </Button>
          </form>
        )}

        {erro && (
          <p className="text-sm text-destructive text-center" role="alert">
            {erro}
          </p>
        )}

        <Link to="/login" className="flex items-center gap-1 text-sm text-muted-foreground underline hover:text-foreground">
          <ArrowLeft className="size-4" /> Voltar ao login
        </Link>
      </div>
    </div>
  );
}

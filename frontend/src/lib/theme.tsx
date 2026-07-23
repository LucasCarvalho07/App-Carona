import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';

export type Tema = 'light' | 'dark' | 'system';

interface ThemeContextValue {
  tema: Tema;
  setTema: (t: Tema) => void;
}

const ThemeContext = createContext<ThemeContextValue | null>(null);
const CHAVE = 'app-carona-tema';

function aplicarTema(tema: Tema) {
  const prefereDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  const escuro = tema === 'dark' || (tema === 'system' && prefereDark);
  document.documentElement.classList.toggle('dark', escuro);
}

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [tema, setTemaState] = useState<Tema>(
    () => (localStorage.getItem(CHAVE) as Tema) ?? 'system',
  );

  useEffect(() => {
    aplicarTema(tema);
    const mq = window.matchMedia('(prefers-color-scheme: dark)');
    const aoMudar = () => {
      if (tema === 'system') {
        aplicarTema('system');
      }
    };
    mq.addEventListener('change', aoMudar);
    return () => mq.removeEventListener('change', aoMudar);
  }, [tema]);

  function setTema(t: Tema) {
    localStorage.setItem(CHAVE, t);
    setTemaState(t);
  }

  const valor = useMemo(() => ({ tema, setTema }), [tema]);

  return <ThemeContext.Provider value={valor}>{children}</ThemeContext.Provider>;
}

export function useTheme(): ThemeContextValue {
  const ctx = useContext(ThemeContext);
  if (!ctx) {
    throw new Error('useTheme precisa estar dentro de ThemeProvider.');
  }
  return ctx;
}

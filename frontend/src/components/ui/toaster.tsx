import 'sileo/styles.css';
import { Toaster as SileoToaster } from 'sileo';
import { useTheme } from '@/lib/theme';

/** Toaster global (Sileo) no topo-centro, seguindo o tema do sistema. */
export function Toaster() {
  const { tema } = useTheme();
  return <SileoToaster theme={tema} position="top-center" />;
}

import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '@/lib/auth';
import { StatusUsuario } from '@/types';

/** Exige usuário logado e ativo; senão manda pro login/aguardando. */
export function RequireAuth({ children }: { children: ReactNode }) {
  const { usuario } = useAuth();
  if (!usuario) {
    return <Navigate to="/login" replace />;
  }
  if (usuario.status !== StatusUsuario.Ativo) {
    return <Navigate to="/aguardando" replace />;
  }
  return <>{children}</>;
}

/** Exige papel Master; usuário comum vai pro dashboard. */
export function RequireMaster({ children }: { children: ReactNode }) {
  const { usuario, isMaster } = useAuth();
  if (!usuario) {
    return <Navigate to="/login" replace />;
  }
  if (!isMaster) {
    return <Navigate to="/dashboard" replace />;
  }
  return <>{children}</>;
}

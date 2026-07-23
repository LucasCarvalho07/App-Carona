import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/lib/auth';

export function Aguardando() {
  const { usuario, logout } = useAuth();
  const navigate = useNavigate();

  function sair() {
    logout();
    navigate('/login', { replace: true });
  }

  return (
    <div className="min-h-svh flex flex-col items-center justify-center gap-4 p-8 text-center">
      <h1 className="text-2xl font-semibold">Olá, {usuario?.nome}</h1>
      <p className="text-muted-foreground max-w-md">
        Seu cadastro foi criado e está aguardando aprovação do administrador.
      </p>
      <Button variant="outline" onClick={sair}>
        Sair
      </Button>
    </div>
  );
}

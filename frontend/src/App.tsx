import { Navigate, Route, Routes } from 'react-router-dom';
import { RequireAuth, RequireMaster } from '@/components/guards';
import { Toaster } from '@/components/ui/toaster';
import { Layout } from '@/components/Layout';
import { useAuth } from '@/lib/auth';
import { StatusUsuario } from '@/types';
import { Login } from '@/pages/Login';
import { EsqueciSenha } from '@/pages/EsqueciSenha';
import { Aguardando } from '@/pages/Aguardando';
import { Dashboard } from '@/pages/Dashboard';
import { Admin } from '@/pages/Admin';
import { Perfil } from '@/pages/Perfil';
import { MeuVeiculo } from '@/pages/MeuVeiculo';
import { ConfigCalculo } from '@/pages/ConfigCalculo';
import { Marcacao } from '@/pages/Marcacao';
import { Pagamentos } from '@/pages/Pagamentos';
import { Resumo } from '@/pages/Resumo';
import { Recebimentos } from '@/pages/Recebimentos';

/** Rota "/" e desconhecidas: redireciona conforme o estado do usuário. */
function Inicio() {
  const { usuario } = useAuth();
  if (!usuario) {
    return <Navigate to="/login" replace />;
  }
  if (usuario.status !== StatusUsuario.Ativo) {
    return <Navigate to="/aguardando" replace />;
  }
  return <Navigate to="/dashboard" replace />;
}

function App() {
  return (
    <>
      <Toaster />
      <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/esqueci-senha" element={<EsqueciSenha />} />
      <Route path="/aguardando" element={<Aguardando />} />

      <Route
        element={
          <RequireAuth>
            <Layout />
          </RequireAuth>
        }
      >
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/marcacao" element={<Marcacao />} />
        <Route path="/pagamentos" element={<Pagamentos />} />
        <Route path="/resumo" element={<Resumo />} />
        <Route path="/motorista" element={<MeuVeiculo />} />
        <Route path="/recebimentos" element={<Recebimentos />} />
        <Route path="/perfil" element={<Perfil />} />
        <Route
          path="/admin"
          element={
            <RequireMaster>
              <Admin />
            </RequireMaster>
          }
        />
        <Route
          path="/config"
          element={
            <RequireMaster>
              <ConfigCalculo />
            </RequireMaster>
          }
        />
      </Route>

      <Route path="*" element={<Inicio />} />
      </Routes>
    </>
  );
}

export default App;

import { useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import {
  BarChart3,
  Car,
  CalendarCheck,
  HandCoins,
  LayoutDashboard,
  LogOut,
  Moon,
  PanelLeft,
  PanelLeftClose,
  Settings,
  Sun,
  User,
  Users,
  Wallet,
  type LucideIcon,
} from 'lucide-react';
import { cn } from '@/lib/utils';
import { useAuth } from '@/lib/auth';
import { useTheme } from '@/lib/theme';
import { Papel } from '@/types';
import { Avatar } from '@/components/Avatar';
import logo from '@/assets/logo.png';

interface ItemMenu {
  to: string;
  label: string;
  icon: LucideIcon;
  somenteMaster?: boolean;
  somenteMotorista?: boolean;
}

const ITENS: ItemMenu[] = [
  { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/marcacao', label: 'Marcação', icon: CalendarCheck },
  { to: '/pagamentos', label: 'Pagamentos', icon: Wallet },
  { to: '/resumo', label: 'Resumo', icon: BarChart3 },
  { to: '/motorista', label: 'Meu veículo', icon: Car, somenteMotorista: true },
  { to: '/recebimentos', label: 'Recebimentos', icon: HandCoins, somenteMotorista: true },
  { to: '/admin', label: 'Usuários', icon: Users, somenteMaster: true },
  { to: '/config', label: 'Cálculo', icon: Settings, somenteMaster: true },
  { to: '/perfil', label: 'Perfil', icon: User },
];

function ThemeToggle() {
  const { tema, setTema } = useTheme();
  const escuro =
    tema === 'dark' ||
    (tema === 'system' && window.matchMedia('(prefers-color-scheme: dark)').matches);

  return (
    <button
      type="button"
      aria-label="Alternar tema"
      className="cursor-pointer rounded-lg p-2 text-muted-foreground transition-colors hover:bg-primary/10 hover:text-primary"
      onClick={() => setTema(escuro ? 'light' : 'dark')}
    >
      {escuro ? <Sun className="size-5" /> : <Moon className="size-5" />}
    </button>
  );
}

export function Layout() {
  const { usuario, isMaster, logout } = useAuth();
  const navigate = useNavigate();
  const [sidebarAberta, setSidebarAberta] = useState(true);

  const ehMotorista = usuario?.papeis?.includes(Papel.Motorista) ?? false;
  const itensVisiveis = ITENS.filter((item) => {
    if (isMaster) return true; // master acessa todos os menus
    if (item.somenteMaster) return false;
    if (item.somenteMotorista) return ehMotorista;
    return true;
  });

  function sair() {
    logout();
    navigate('/login', { replace: true });
  }

  return (
    <div className="min-h-svh flex flex-col md:flex-row">
      {/* Sidebar desktop — flutuante */}
      <aside
        className={cn(
          'hidden md:block shrink-0 overflow-hidden transition-[width] duration-300 ease-in-out',
          sidebarAberta ? 'md:w-64' : 'md:w-0',
        )}
      >
        <div className="h-full w-64 p-3">
          <div className="flex h-full flex-col rounded-2xl border bg-card shadow-lg">
            <div className="h-14 flex items-center justify-between gap-2 px-3 border-b">
              <img src={logo} alt="Caronas" className="h-9 w-auto object-contain" />
              <button
                type="button"
                aria-label="Recolher menu"
                className="cursor-pointer rounded-lg p-2 text-muted-foreground transition-colors hover:bg-primary/10 hover:text-primary"
                onClick={() => setSidebarAberta(false)}
              >
                <PanelLeftClose className="size-5" />
              </button>
            </div>
            <nav className="flex-1 p-3 space-y-1 overflow-y-auto">
              {itensVisiveis.map((item) => (
                <ItemNav key={item.to} item={item} />
              ))}
            </nav>
            <div className="p-3 border-t">
              <button
                type="button"
                onClick={sair}
                className="w-full flex items-center gap-2 rounded-lg px-3 py-2 text-sm cursor-pointer transition-colors hover:bg-primary/10 hover:text-primary"
              >
                <LogOut className="size-4" /> Sair
              </button>
            </div>
          </div>
        </div>
      </aside>

      <div className="flex-1 flex flex-col min-w-0">
        {/* Header */}
        <header className="sticky top-0 z-20 h-14 flex items-center justify-between px-4 border-b border-border/60 bg-card/70 backdrop-blur-md supports-[backdrop-filter]:bg-card/60">
          <div className="flex items-center gap-2">
            {!sidebarAberta && (
              <button
                type="button"
                aria-label="Abrir menu"
                className="hidden md:inline-flex cursor-pointer rounded-lg p-2 text-muted-foreground transition-colors hover:bg-primary/10 hover:text-primary animate-in fade-in"
                onClick={() => setSidebarAberta(true)}
              >
                <PanelLeft className="size-5" />
              </button>
            )}
            <img src={logo} alt="Caronas" className="h-7 w-auto object-contain md:hidden" />
          </div>
          <div className="flex items-center gap-2 sm:gap-3">
            <ThemeToggle />
            <button
              type="button"
              onClick={() => navigate('/perfil')}
              className="flex items-center gap-2 rounded-full py-1 pl-1 pr-1 sm:pr-3 cursor-pointer transition-colors hover:bg-muted"
            >
              <Avatar seed={usuario?.avatar} nome={usuario?.nome} className="size-8 ring-2 ring-primary/20" />
              <span className="hidden sm:block text-sm font-medium">{usuario?.nome}</span>
            </button>
          </div>
        </header>

        <main className="flex-1 p-4 pb-24 md:pb-6 md:p-6">
          <Outlet />
        </main>

        {/* Bottom nav mobile */}
        <nav className="md:hidden fixed bottom-0 inset-x-0 z-20 border-t border-border/60 bg-card/85 backdrop-blur-md flex justify-around px-2 pt-2 pb-[calc(0.5rem+env(safe-area-inset-bottom))]">
          {itensVisiveis.slice(0, 5).map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                cn(
                  'flex flex-col items-center gap-1 text-[0.7rem] font-medium px-2 py-1 rounded-xl transition-colors min-w-14',
                  isActive ? 'text-primary' : 'text-muted-foreground hover:text-foreground',
                )
              }
            >
              {({ isActive }) => (
                <>
                  <span
                    className={cn(
                      'flex items-center justify-center rounded-full px-3 py-1 transition-colors',
                      isActive && 'bg-primary/15',
                    )}
                  >
                    <item.icon className="size-5" />
                  </span>
                  {item.label}
                </>
              )}
            </NavLink>
          ))}
        </nav>
      </div>
    </div>
  );
}

function ItemNav({ item }: { item: ItemMenu }) {
  return (
    <NavLink
      to={item.to}
      className={({ isActive }) =>
        cn(
          'relative flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm transition-all',
          isActive
            ? 'bg-gradient-to-r from-primary/20 to-primary/5 text-primary font-semibold'
            : 'text-muted-foreground hover:bg-primary/10 hover:text-primary hover:translate-x-0.5',
        )
      }
    >
      {({ isActive }) => (
        <>
          {isActive && (
            <span className="absolute left-0 top-1/2 h-5 w-1 -translate-y-1/2 rounded-full bg-primary" />
          )}
          <item.icon className="size-4 shrink-0" />
          {item.label}
        </>
      )}
    </NavLink>
  );
}

// Domínio: constantes (valor em runtime) + tipo derivado (mesmo nome).
// Fonte única de verdade — evita literais soltos espalhados pelas telas.
export const StatusUsuario = {
  AguardandoAprovacao: 'AguardandoAprovacao',
  Ativo: 'Ativo',
  Inativo: 'Inativo',
} as const;
export type StatusUsuario = (typeof StatusUsuario)[keyof typeof StatusUsuario];

export const Papel = {
  Master: 'Master',
  Motorista: 'Motorista',
  Passageiro: 'Passageiro',
} as const;
export type Papel = (typeof Papel)[keyof typeof Papel];

export interface Usuario {
  id: number;
  email: string;
  nome: string;
  telefone?: string | null;
  fotoUrl?: string | null;
  avatar?: string | null;
  status: StatusUsuario;
  papeis: Papel[];
  criadoEm: string;
  ehMasterPrincipal?: boolean;
  emailVerificado?: boolean;
}

export interface AuthResponse {
  token: string;
  usuario: Usuario;
}

export interface ResumoMensalPassageiro {
  passageiroId: number;
  nome: string;
  avatar?: string | null;
  qtdDias: number;
  qtdViagens: number;
  valor: number;
}

export interface ResumoMensalMotorista {
  motoristaId: number;
  motoristaNome: string;
  avatar?: string | null;
  totalValor: number;
  qtdDiasDirigiu: number;
  qtdViagens: number;
  passageiros: ResumoMensalPassageiro[];
}

export type TipoCombustivel = 'Gasolina' | 'Etanol' | 'Diesel' | 'Flex' | 'Gnv';
export type TipoChavePix = 'Cpf' | 'Cnpj' | 'Email' | 'Telefone' | 'Aleatoria';

export interface Veiculo {
  id: number;
  nome: string;
  modelo?: string | null;
  consumoKmLitro: number;
  kmPorViagem: number;
  combustivel: TipoCombustivel;
  padrao: boolean;
  ativo: boolean;
}

export interface MotoristaConfig {
  chavePix: string;
  tipoChave: TipoChavePix;
  titular: string;
  emailComprovante?: string | null;
}

export interface ParametroCusto {
  id: number;
  vigenteDe: string;
  precoLitro: number;
  custoKmManutencao: number;
}

export interface MotoristaOpcao {
  id: number;
  nome: string;
  avatar?: string | null;
  configurado: boolean;
}

export interface DetalheMotorista {
  motoristaId: number;
  motoristaNome: string;
  anoMes: number;
  configurado: boolean;
  temConfigMes: boolean;
  veiculoNome?: string | null;
  consumoKmLitro: number;
  kmPorViagem: number;
  precoLitro: number;
  custoKmManutencao: number;
  custoCombustivel: number;
  custoTotal: number;
}

export const Sentido = {
  Ida: 'Ida',
  Volta: 'Volta',
} as const;
export type Sentido = (typeof Sentido)[keyof typeof Sentido];

export interface EscalaCarro {
  viagemId: number;
  data: string;
  sentido: Sentido;
  motoristaId: number;
  motoristaNome: string;
  avatar?: string | null;
  configurado: boolean;
  qtdPassageiros: number;
  souMotorista: boolean;
  estouNesteCarro: boolean;
  valorPorPessoa: number;
  ocupantes: Ocupante[];
}

export interface Ocupante {
  id: number;
  nome: string;
  avatar?: string | null;
}

export interface Marcacao {
  viagemId: number;
  data: string;
  sentido: Sentido;
  motoristaId: number;
  motoristaNome: string;
  valorPorPessoa: number;
  ocupantes: Ocupante[];
}

export const StatusPagamento = {
  Pendente: 'Pendente',
  Informado: 'Informado',
  Confirmado: 'Confirmado',
  Rejeitado: 'Rejeitado',
} as const;
export type StatusPagamento = (typeof StatusPagamento)[keyof typeof StatusPagamento];

export interface PagamentoResumo {
  pagamentoId?: number | null;
  motoristaId: number;
  motoristaNome: string;
  anoMes: number;
  qtdDias: number;
  total: number;
  status: StatusPagamento;
  chavePix?: string | null;
  tipoChave?: string | null;
}

export interface Recebimento {
  pagamentoId?: number | null;
  passageiroId: number;
  passageiroNome: string;
  anoMes: number;
  valor: number;
  status: StatusPagamento;
}

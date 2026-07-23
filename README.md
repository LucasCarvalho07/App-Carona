# App Carona

Aplicativo web para controle de caronas (substitui a planilha). Cada motorista escala suas
viagens, passageiros se marcam nos dias, o sistema calcula o rateio do custo (combustível +
desgaste por km, dividido entre os ocupantes) e gera o PIX para o acerto. Tem controle de
pagamentos/recebimentos, resumo mensal com gráficos e tema claro/escuro.

## Funcionalidades

- **Cadastro e login local** (e-mail + senha) e **login com Google**.
- **Senha forte** obrigatória (mín. 6 caracteres, 1 maiúscula, 1 caractere especial).
- **Recuperação de senha** por código de 6 dígitos enviado por e-mail.
- **Papéis**: Passageiro (base), Motorista e Master. Aprovação de novos usuários pelo master.
- **Master principal** (definido por configuração) é o único que promove/rebaixa outros masters.
- **Escala e marcação**: motorista escala viagens (ida/volta por data); passageiro se marca.
- **Cálculo de rateio** com preço do combustível **por data de vigência** (histórico) e recálculo
  ao editar km/consumo do veículo (mês atual em diante; passado preservado).
- **PIX**: QR Code + copia-e-cola estático gerado no app (sem gateway), já com o valor.
- **Pagamentos / recebimentos**, resumo mensal e gráficos (ECharts).

## Stack

- **Backend**: ASP.NET Core Web API (C#, .NET 9), NHibernate + FluentNHibernate, PostgreSQL (Npgsql),
  autenticação JWT, envio de e-mail via SMTP (MailKit).
- **Frontend**: React 19 + Vite + TypeScript + Tailwind CSS v4 + shadcn/ui (Base UI).
- **Infra dev**: Docker Compose (Postgres + Adminer + API + Frontend).

## Estrutura

```
backend/                     AppCarona.slnx
  src/
    AppCarona.Api             Controllers, Program.cs, auth, DI, exception handler
    AppCarona.Domain          Entidades, Enums, Exceptions, interfaces (IService/IRepository)
    AppCarona.Application     Services (regras de negócio), mappers
    AppCarona.Infrastructure  NHibernate (SessionFactory, Maps), Repositories, Email, JWT
    AppCarona.Contracts       DTOs (requests/responses)
  tests/AppCarona.Tests       xUnit
frontend/                    React + Vite + shadcn/ui
docker-compose.yml           Stack de desenvolvimento
```

Camadas: `Controller → IService → Service → IRepository → Repository`, tudo assíncrono.

## Como rodar

### Opção A — Docker (recomendado)

Pré-requisitos: Docker Desktop.

```bash
cp .env.example .env                 # ajuste os valores
cp frontend/.env.example frontend/.env
docker compose up --build
```

| Serviço  | URL                     |
|----------|-------------------------|
| Frontend | http://localhost:5173   |
| API      | http://localhost:5080   |
| Adminer  | http://localhost:8080   |
| Postgres | localhost:55432         |

O schema do banco é criado automaticamente no primeiro boot (dev). Sem SMTP configurado, o
código de recuperação/verificação é escrito no log: `docker logs carona_api`.

### Opção B — Local (host)

Pré-requisitos: .NET 9 SDK, Node 22+, PostgreSQL.

```bash
# backend
cd backend/src/AppCarona.Api
dotnet run --urls "http://localhost:5080"

# frontend (outro terminal)
cd frontend
npm install
npm run dev
```

Os segredos em dev ficam no **.NET user-secrets** (ver abaixo). O front (Vite) faz proxy de
`/api` para a API.

## Configuração e segredos

**Nunca** versione segredos. Divisão:

| Onde | O que | Observação |
|------|-------|------------|
| `.NET user-secrets` (dev, host) | `ConnectionStrings:Postgres`, `Jwt:Key`, `Smtp:Senha`, `Admin:MasterEmails:0` | `dotnet user-secrets set "chave" "valor"` no projeto `AppCarona.Api` |
| `.env` (raiz, Docker) | `POSTGRES_*`, `MASTER_EMAIL` | lido pelo docker-compose; **gitignored** |
| `frontend/.env` (Vite) | `VITE_GOOGLE_CLIENT_ID` | **público** (vai no bundle) — só valores não-secretos |
| Variáveis de ambiente (produção) | `ConnectionStrings__Postgres`, `Jwt__Key`, `Smtp__*`, `Admin__MasterEmails__0` | injetadas pelo host/secret manager |

`appsettings.json` guarda só configuração não-sensível (Issuer/Audience, portas, etc.) e
**placeholders vazios** para os segredos. Em produção a aplicação **recusa subir** se a `Jwt:Key`
ainda for o placeholder de desenvolvimento.

### Master principal

O e-mail em `Admin:MasterEmails` (via `MASTER_EMAIL` no `.env`/env) é o **master principal**:
ao se cadastrar/logar ele vira master automaticamente (conta local exige **verificar o e-mail**
por código antes; Google já é verificado). Só o principal pode tornar outros usuários master.

## Segurança

- Senhas com **BCrypt**; código de recuperação/verificação guardado **hasheado**, com expiração
  (10 min) e limite de tentativas.
- **Rate limiting** nos endpoints de autenticação (10 req/min por IP).
- Handler global de exceções (sem vazar stack); cabeçalhos de segurança + HSTS em produção.
- Queries via NHibernate (parametrizadas) — sem SQL concatenado.

## Verificação

```bash
# backend
cd backend && dotnet build

# frontend
cd frontend && npx tsc --noEmit && npm run build
```

# Deploy (custo zero): Neon + Render + Cloudflare Pages

Arquitetura em produção:

```
Cloudflare Pages (front estático)  ──/api──▶  Render (API .NET, Docker)  ──▶  Neon (Postgres)
```

> Grátis, sem cartão. Trade-off: Render e Neon "dormem" quando ociosos — o 1º acesso após
> inatividade demora alguns segundos (cold start). Normal para uso interno.

Pré-requisito: o repositório já no **GitHub**.

---

## 1. Banco — Neon

1. Cria conta em https://neon.tech → **New Project** (região mais perto, ex. AWS sa-east-1).
2. Copia a connection string. O Neon dá um formato URL (`postgresql://...`). Converta para o
   formato do Npgsql (chave=valor):
   ```
   Host=ep-xxxx.sa-east-1.aws.neon.tech;Database=neondb;Username=SEU_USER;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true
   ```
   Guarde essa string — é o `ConnectionStrings__Postgres`.

## 2. API — Render

1. Cria conta em https://render.com → **New → Web Service** → conecta o repo do GitHub.
2. Configura:
   - **Runtime**: Docker
   - **Dockerfile Path**: `backend/Dockerfile.prod`
   - **Docker Build Context Directory**: `backend`
   - **Instance Type**: Free
3. Em **Environment** adiciona as variáveis:

   | Variável | Valor |
   |---|---|
   | `ASPNETCORE_ENVIRONMENT` | `Production` |
   | `ConnectionStrings__Postgres` | (a string do Neon do passo 1) |
   | `Jwt__Key` | uma chave forte aleatória (ver abaixo) |
   | `Admin__MasterEmails__0` | seu e-mail de master principal |
   | `Authentication__Google__ClientId` | seu Client ID do Google |
   | `Smtp__Host` | `smtp.gmail.com` |
   | `Smtp__Port` | `587` |
   | `Smtp__Usuario` | `app.caronaabase@gmail.com` |
   | `Smtp__Senha` | app password do Gmail |
   | `Smtp__Remetente` | `app.caronaabase@gmail.com` |
   | `Smtp__RemetenteNome` | `App Carona` |
   | `Cors__Origins__0` | (preencher depois com a URL do Cloudflare Pages) |
   | `NHibernate__ExportarSchema` | `true` (cria o schema no 1º deploy) |

   Gerar a chave JWT (qualquer terminal):
   ```
   openssl rand -base64 48
   ```
4. Faz o deploy. Anota a URL gerada, ex.: `https://app-carona-api.onrender.com`.
5. (Após o 1º boot, o schema é criado. Pode deixar `ExportarSchema=true` — é aditivo — ou mudar
   para `false` depois; se mudar, redeploy.)

## 3. Frontend — Cloudflare Pages

1. Cria conta em https://pages.cloudflare.com → **Create application → Pages → conecta o GitHub**.
2. Build settings:
   - **Root directory**: `frontend`
   - **Build command**: `npm run build`
   - **Build output directory**: `dist`
3. **Environment variables**:
   | Variável | Valor |
   |---|---|
   | `VITE_API_URL` | `https://app-carona-api.onrender.com/api` (URL do Render + `/api`) |
   | `VITE_GOOGLE_CLIENT_ID` | seu Client ID do Google |
4. Deploy. Anota a URL, ex.: `https://app-carona.pages.dev`.

## 4. Fechar o círculo (CORS + OAuth)

1. Volta no **Render → Environment**: põe `Cors__Origins__0 = https://app-carona.pages.dev`
   (a URL do Pages) e **Manual Deploy / Save** (reinicia a API).
2. **Google Cloud Console** → APIs & Services → Credentials → seu OAuth Client:
   - **Authorized JavaScript origins**: adiciona `https://app-carona.pages.dev`
   - Salva (leva alguns minutos pra propagar).

## 5. Primeiro acesso

1. Abre `https://app-carona.pages.dev`.
2. Cadastra com o e-mail definido em `Admin__MasterEmails__0` → recebe o **código de verificação
   por e-mail** (SMTP real em produção) → confirma → vira **master principal**.
3. Demais usuários se cadastram → você aprova/promove pela tela **Usuários**.

---

## Notas

- **Ordem importa**: API primeiro (pega a URL) → Pages (usa a URL) → volta no Render pra setar o
  CORS com a URL do Pages.
- **Cold start**: 1º request após ociosidade demora (Render acorda o container; Neon acorda o DB).
- **Segredos**: nada de segredo vai pro Git — tudo em variáveis de ambiente no Render/Pages.
- **Custo**: zero nos tiers gratuitos. Se um dia precisar "não dormir", o plano pago do Render
  resolve (mas não é necessário para uso interno).

// Dev: '/api' (proxy do Vite). Produção: URL absoluta da API (ex.: https://app-carona-api.onrender.com/api).
const BASE_URL = import.meta.env.VITE_API_URL || '/api';

/**
 * Cliente HTTP simples. Injeta o JWT quando informado e trata respostas de erro.
 */
async function request<T>(
  metodo: string,
  path: string,
  body?: unknown,
  token?: string,
): Promise<T> {
  const headers: Record<string, string> = { 'Content-Type': 'application/json' };
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const resposta = await fetch(`${BASE_URL}${path}`, {
    method: metodo,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (!resposta.ok) {
    // Tenta extrair a mensagem do corpo ({ mensagem: "..." }); senão usa o status.
    const mensagem = await extrairMensagemErro(resposta);
    throw new Error(mensagem);
  }

  if (resposta.status === 204 || resposta.headers.get('content-length') === '0') {
    return undefined as T;
  }

  return (await resposta.json()) as T;
}

async function extrairMensagemErro(resposta: Response): Promise<string> {
  try {
    const corpo = (await resposta.json()) as { mensagem?: string };
    if (corpo.mensagem) {
      return corpo.mensagem;
    }
  } catch {
    // corpo não-JSON: ignora e cai no fallback
  }
  return `Erro ${resposta.status}`;
}

export const api = {
  get: <T>(path: string, token?: string) => request<T>('GET', path, undefined, token),
  post: <T>(path: string, body: unknown, token?: string) =>
    request<T>('POST', path, body, token),
  put: <T>(path: string, body: unknown, token?: string) =>
    request<T>('PUT', path, body, token),
  del: (path: string, body: unknown, token?: string) =>
    request<void>('DELETE', path, body, token),
};

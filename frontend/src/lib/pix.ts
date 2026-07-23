/**
 * Gera o "Pix copia e cola" (BR Code / payload EMV) com valor embutido.
 * A string resultante é a mesma que o app do banco lê no QR Code.
 */

interface DadosPix {
  chave: string;
  nome: string;
  cidade?: string;
  valor?: number;
  txid?: string;
}

/** id + comprimento (2 dígitos) + valor. */
function tlv(id: string, valor: string): string {
  const tamanho = valor.length.toString().padStart(2, '0');
  return `${id}${tamanho}${valor}`;
}

/** Remove acentos e força maiúsculas (EMV recomenda ASCII em nome/cidade). */
function sanitizar(texto: string, max: number): string {
  return texto
    .normalize('NFD') // separa os acentos das letras
    .replace(/[^0-9A-Za-z ]/g, '') // remove acentos e símbolos, sobra ASCII
    .toUpperCase()
    .trim()
    .slice(0, max);
}

/** CRC16/CCITT-FALSE (poly 0x1021, init 0xFFFF) — 4 dígitos hex maiúsculos. */
function crc16(texto: string): string {
  let crc = 0xffff;
  for (let i = 0; i < texto.length; i++) {
    crc ^= texto.charCodeAt(i) << 8;
    for (let bit = 0; bit < 8; bit++) {
      crc = crc & 0x8000 ? (crc << 1) ^ 0x1021 : crc << 1;
      crc &= 0xffff;
    }
  }
  return crc.toString(16).toUpperCase().padStart(4, '0');
}

export function montarPixCopiaECola({ chave, nome, cidade = 'BRASIL', valor, txid = '***' }: DadosPix): string {
  const merchantAccount = tlv('00', 'BR.GOV.BCB.PIX') + tlv('01', chave.trim());

  const partes = [
    tlv('00', '01'), // payload format indicator
    tlv('26', merchantAccount), // merchant account information (Pix)
    tlv('52', '0000'), // merchant category code
    tlv('53', '986'), // moeda BRL
  ];

  if (valor && valor > 0) {
    partes.push(tlv('54', valor.toFixed(2)));
  }

  partes.push(
    tlv('58', 'BR'), // país
    tlv('59', sanitizar(nome, 25) || 'RECEBEDOR'), // nome
    tlv('60', sanitizar(cidade, 15) || 'BRASIL'), // cidade
    tlv('62', tlv('05', txid)), // additional data (txid)
  );

  const semCrc = partes.join('') + '6304';
  return semCrc + crc16(semCrc);
}

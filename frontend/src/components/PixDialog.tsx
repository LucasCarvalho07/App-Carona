import { useState } from 'react';
import QRCode from 'qrcode';
import { QrCode, Copy } from 'lucide-react';
import { sileo } from 'sileo';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { montarPixCopiaECola } from '@/lib/pix';
import { formatarReal } from '@/lib/ui';

export function PixDialog({ chave, nome, valor }: { chave: string; nome: string; valor: number }) {
  const [qr, setQr] = useState<string | null>(null);
  const [codigo, setCodigo] = useState('');

  async function gerar() {
    const payload = montarPixCopiaECola({ chave, nome, valor });
    setCodigo(payload);
    setQr(await QRCode.toDataURL(payload, { margin: 1, width: 240 }));
  }

  function copiar() {
    void navigator.clipboard.writeText(codigo);
    sileo.success({ title: 'Código Pix copiado.' });
  }

  return (
    <Dialog onOpenChange={(aberto) => aberto && void gerar()}>
      <DialogTrigger
        render={
          <Button size="sm">
            <QrCode className="size-4" /> Pagar com PIX
          </Button>
        }
      />
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Pagar {nome}</DialogTitle>
          <DialogDescription>
            {valor > 0 ? `Valor: ${formatarReal(valor)}` : 'Escaneie ou copie o código Pix.'}
          </DialogDescription>
        </DialogHeader>

        <div className="flex flex-col items-center gap-4">
          {qr ? (
            <img src={qr} alt="QR Code Pix" className="size-56 rounded-xl border bg-white p-2" />
          ) : (
            <p className="text-sm text-muted-foreground">Gerando QR Code…</p>
          )}

          {codigo && (
            <div className="w-full space-y-2">
              <p className="rounded-lg border bg-muted/40 p-2 text-xs break-all text-muted-foreground">
                {codigo}
              </p>
              <Button variant="outline" className="w-full" onClick={copiar}>
                <Copy className="size-4" /> Copiar código Pix
              </Button>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}

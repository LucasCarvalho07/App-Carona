import { useEffect, useRef } from 'react';
import * as echarts from 'echarts/core';
import { BarChart, PieChart } from 'echarts/charts';
import { GridComponent, TooltipComponent, LegendComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { useTheme } from '@/lib/theme';

echarts.use([BarChart, PieChart, GridComponent, TooltipComponent, LegendComponent, CanvasRenderer]);

/** Paleta categórica variada (azul, vermelho, âmbar, verde…) — boa em light e dark. */
export const paletaGrafico = [
  '#3b82f6', // azul
  '#ef4444', // vermelho
  '#f59e0b', // âmbar
  '#22c55e', // verde
  '#8b5cf6', // roxo
  '#06b6d4', // ciano
  '#ec4899', // rosa
  '#84cc16', // limão
  '#f97316', // laranja
  '#14b8a6', // teal
];

/** Wrapper React fino sobre o ECharts (init/resize/dispose). */
export function EChart({ option, className }: { option: echarts.EChartsCoreOption; className?: string }) {
  const ref = useRef<HTMLDivElement | null>(null);
  const grafico = useRef<echarts.ECharts | null>(null);
  const { tema } = useTheme();

  useEffect(() => {
    const alvo = ref.current;
    if (!alvo) return;
    const inst = echarts.init(alvo);
    grafico.current = inst;
    const ro = new ResizeObserver(() => inst.resize());
    ro.observe(alvo);
    return () => {
      ro.disconnect();
      inst.dispose();
      grafico.current = null;
    };
  }, []);

  useEffect(() => {
    grafico.current?.setOption(option, true);
  }, [option, tema]);

  return <div ref={ref} className={className} />;
}

/** Cores dos gráficos conforme o tema atual (evita oklch no canvas). */
export function coresGrafico() {
  const escuro = document.documentElement.classList.contains('dark');
  return {
    texto: escuro ? '#a1a1aa' : '#71717a',
    linha: escuro ? '#3f3f46' : '#e4e4e7',
    primaria: '#e69824',
    primariaFraca: escuro ? '#7c4d10' : '#f4c986',
    fundoCard: escuro ? '#18181b' : '#ffffff',
  };
}

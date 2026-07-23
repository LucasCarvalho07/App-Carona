using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class PagamentoMap : ClassMap<Pagamento>
{
    public PagamentoMap()
    {
        Table("pagamento");
        Id(x => x.Id).GeneratedBy.Sequence("pagamento_id_seq").Column("id");
        References(x => x.Passageiro).Column("passageiro_id").Not.Nullable();
        References(x => x.Motorista).Column("motorista_id").Not.Nullable();
        Map(x => x.AnoMes).Column("ano_mes").Not.Nullable();
        Map(x => x.Valor).Column("valor");
        Map(x => x.Status).Column("status").CustomType<int>();
        Map(x => x.InformadoEm).Column("informado_em");
        Map(x => x.ConfirmadoEm).Column("confirmado_em");
        Map(x => x.Observacao).Column("observacao");
    }
}

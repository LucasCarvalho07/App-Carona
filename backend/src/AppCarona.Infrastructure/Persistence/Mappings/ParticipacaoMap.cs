using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class ParticipacaoMap : ClassMap<Participacao>
{
    public ParticipacaoMap()
    {
        Table("participacao");
        Id(x => x.Id).GeneratedBy.Sequence("participacao_id_seq").Column("id");
        References(x => x.Passageiro).Column("passageiro_id").Not.Nullable();
        Map(x => x.Origem).Column("origem").CustomType<int>();
        Map(x => x.ValorDevido).Column("valor_devido");
    }
}

using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class ParametroCustoMap : ClassMap<ParametroCusto>
{
    public ParametroCustoMap()
    {
        Table("parametro_custo");
        Id(x => x.Id).GeneratedBy.Sequence("parametro_custo_id_seq").Column("id");
        Map(x => x.VigenteDe).Column("vigente_de").Not.Nullable().Unique();
        Map(x => x.PrecoLitro).Column("preco_litro");
        Map(x => x.CustoKmManutencao).Column("custo_km_manutencao");
    }
}

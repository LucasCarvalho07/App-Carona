using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class VeiculoMap : ClassMap<Veiculo>
{
    public VeiculoMap()
    {
        Table("veiculo");
        Id(x => x.Id).GeneratedBy.Sequence("veiculo_id_seq").Column("id");
        References(x => x.Motorista).Column("motorista_id").Not.Nullable();
        Map(x => x.Nome).Column("nome").Not.Nullable();
        Map(x => x.Modelo).Column("modelo");
        Map(x => x.ConsumoKmLitro).Column("consumo_km_litro");
        Map(x => x.KmPorViagem).Column("km_por_viagem");
        Map(x => x.Combustivel).Column("combustivel").CustomType<int>();
        Map(x => x.Padrao).Column("padrao");
        Map(x => x.Ativo).Column("ativo");
    }
}

using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class ViagemMap : ClassMap<Viagem>
{
    public ViagemMap()
    {
        Table("viagem");
        Id(x => x.Id).GeneratedBy.Sequence("viagem_id_seq").Column("id");
        Map(x => x.Data).Column("data").Not.Nullable();
        Map(x => x.Sentido).Column("sentido").CustomType<int>();
        References(x => x.Motorista).Column("motorista_id").Not.Nullable();
        References(x => x.Veiculo).Column("veiculo_id");
        Map(x => x.Status).Column("status").CustomType<int>();
        Map(x => x.Observacao).Column("observacao");
        Map(x => x.Origem).Column("origem").CustomType<int>();
        Map(x => x.CriadoPor).Column("criado_por");
        Map(x => x.CriadoEm).Column("criado_em").Not.Nullable();

        Map(x => x.SnapKmPorViagem).Column("snap_km_por_viagem");
        Map(x => x.SnapConsumoKmLitro).Column("snap_consumo_km_litro");
        Map(x => x.SnapPrecoLitro).Column("snap_preco_litro");
        Map(x => x.SnapCustoKmManutencao).Column("snap_custo_km_manutencao");

        Map(x => x.QtdPessoas).Column("qtd_pessoas");
        Map(x => x.CustoCombustivel).Column("custo_combustivel");
        Map(x => x.CustoTotal).Column("custo_total");
        Map(x => x.ValorPorPessoa).Column("valor_por_pessoa");

        HasMany(x => x.Participacoes)
            .KeyColumn("viagem_id")
            .Cascade.AllDeleteOrphan()
            .AsBag();
    }
}

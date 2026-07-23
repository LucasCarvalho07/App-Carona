using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class CodigoRecuperacaoMap : ClassMap<CodigoRecuperacao>
{
    public CodigoRecuperacaoMap()
    {
        Table("codigo_recuperacao");
        Id(x => x.Id).GeneratedBy.Sequence("codigo_recuperacao_id_seq").Column("id");
        Map(x => x.UsuarioId).Column("usuario_id").Not.Nullable();
        Map(x => x.CodigoHash).Column("codigo_hash").Not.Nullable();
        Map(x => x.Canal).Column("canal").CustomType<int>();
        Map(x => x.ExpiraEm).Column("expira_em").Not.Nullable();
        Map(x => x.TentativasRestantes).Column("tentativas_restantes").Not.Nullable();
        Map(x => x.Usado).Column("usado").Not.Nullable();
        Map(x => x.CriadoEm).Column("criado_em").Not.Nullable();
    }
}

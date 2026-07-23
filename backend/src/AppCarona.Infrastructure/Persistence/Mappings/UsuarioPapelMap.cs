using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class UsuarioPapelMap : ClassMap<UsuarioPapel>
{
    public UsuarioPapelMap()
    {
        Table("usuario_papel");
        Id(x => x.Id).GeneratedBy.Sequence("usuario_papel_id_seq").Column("id");
        Map(x => x.Papel).Column("papel").CustomType<int>();
    }
}

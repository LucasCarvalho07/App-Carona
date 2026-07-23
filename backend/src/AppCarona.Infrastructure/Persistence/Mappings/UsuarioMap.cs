using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class UsuarioMap : ClassMap<Usuario>
{
    public UsuarioMap()
    {
        Table("usuario");
        Id(x => x.Id).GeneratedBy.Sequence("usuario_id_seq").Column("id");
        Map(x => x.GoogleSub).Column("google_sub").Unique();
        Map(x => x.Email).Column("email").Not.Nullable().Unique();
        Map(x => x.Nome).Column("nome").Not.Nullable();
        Map(x => x.Telefone).Column("telefone");
        Map(x => x.FotoUrl).Column("foto_url");
        Map(x => x.Avatar).Column("avatar");
        Map(x => x.SenhaHash).Column("senha_hash");
        Map(x => x.EmailVerificado).Column("email_verificado");
        Map(x => x.Status).Column("status").CustomType<int>();
        Map(x => x.CriadoEm).Column("criado_em").Not.Nullable();

        HasMany(x => x.Papeis)
            .KeyColumn("usuario_id")
            .Cascade.AllDeleteOrphan()
            .AsBag();
    }
}

using AppCarona.Domain.Entities;
using FluentNHibernate.Mapping;

namespace AppCarona.Infrastructure.Persistence.Mappings;

public class MotoristaConfigMap : ClassMap<MotoristaConfig>
{
    public MotoristaConfigMap()
    {
        Table("motorista_config");
        Id(x => x.Id).GeneratedBy.Sequence("motorista_config_id_seq").Column("id");
        References(x => x.Motorista).Column("usuario_id").Not.Nullable().Unique();
        Map(x => x.ChavePix).Column("chave_pix");
        Map(x => x.TipoChave).Column("tipo_chave").CustomType<int>();
        Map(x => x.Titular).Column("titular");
        Map(x => x.EmailComprovante).Column("email_comprovante");
    }
}

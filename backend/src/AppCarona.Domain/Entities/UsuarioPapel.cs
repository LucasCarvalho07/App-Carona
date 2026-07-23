using AppCarona.Domain.Enums;

namespace AppCarona.Domain.Entities;

/// <summary>Papel atribuído a um usuário. Coleção pertencente ao Usuario (usuario_papel).</summary>
public class UsuarioPapel
{
    public virtual int Id { get; set; }
    public virtual Papel Papel { get; set; }
}

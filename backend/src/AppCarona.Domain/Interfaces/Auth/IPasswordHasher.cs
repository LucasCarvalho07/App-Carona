namespace AppCarona.Domain.Interfaces.Auth;

public interface IPasswordHasher
{
    string Hash(string senha);
    bool Verificar(string senha, string hash);
}

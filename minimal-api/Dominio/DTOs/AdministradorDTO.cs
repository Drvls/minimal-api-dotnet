using minimal_api.Dominio.Enuns;

namespace minimal_api.Dominio.DTOs;

public class AdministradorDTO{
    public string Email { get; set; } = null!;
    public string  Senha { get; set; } = null!;
    public Perfil? Perfil { get; set; } = null!;
}
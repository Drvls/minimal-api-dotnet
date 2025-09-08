using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;

namespace teste_minimal_api.Mocks;

public class AdministradorServicoMock : IAdministradorServico
{
    private static List<Administrador> administradores = new List<Administrador>()
    {
        new Administrador
        {
            Id = 1,
            Email = "admin@email",
            Senha = "admin123",
            Perfil = "Admin"
        },
        
        new Administrador
        {
        Id = 2,
        Email = "editor@email",
        Senha = "editor123",
        Perfil = "Editor"
        }
    };
    
    public Administrador? Login(LoginDTO login)
    {
        return administradores.Find(admin => 
            admin.Email == login.Email && 
            admin.Senha ==  login.Senha);
    }

    public Administrador Incluir(Administrador administrador)
    {
        administrador.Id = administradores.Count + 1;
        administradores.Add(administrador);

        return administrador;
    }

    public List<Administrador> Todos(int? pagina)
    {
        return administradores;
    }

    public Administrador? BuscarPorID(int id)
    {
        return administradores.Find(admin => admin.Id == id);
    }
}